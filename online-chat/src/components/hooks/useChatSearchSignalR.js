import { useCallback, useEffect, useMemo, useState } from "react";

const initialState = {
  chatId: null,
  query: "",
  results: [],
  selectedIndexes: [],
  visible: false,
  loading: false,
  error: ""
};

export function useChatSearchSignalR(connection, activeChatId) {
  const [searchState, setSearchState] = useState(initialState);

  useEffect(() => {
    if (!connection) return;

    const handleSearchResultsReceived = (payload) => {
      setSearchState({
        chatId: payload.chatId,
        query: payload.query ?? "",
        results: payload.results ?? [],
        selectedIndexes: [],
        visible: true,
        loading: false,
        error: ""
      });
    };

    const handleSearchCleared = (payload) => {
      setSearchState((prev) => {
        if (prev.chatId !== payload.chatId) return prev;
        return initialState;
      });
    };

    const handleSearchError = (payload) => {
      setSearchState((prev) => ({
        ...prev,
        loading: false,
        error: payload?.message ?? "Ошибка поиска"
      }));
    };

    connection.on("SearchResultsReceived", handleSearchResultsReceived);
    connection.on("SearchCleared", handleSearchCleared);
    connection.on("SearchError", handleSearchError);

    return () => {
      connection.off("SearchResultsReceived", handleSearchResultsReceived);
      connection.off("SearchCleared", handleSearchCleared);
      connection.off("SearchError", handleSearchError);
    };
  }, [connection]);

  useEffect(() => {
    setSearchState((prev) => {
      if (!prev.visible) return prev;
      if (activeChatId == null) return initialState;
      if (prev.chatId === Number(activeChatId)) return prev;
      return {
        ...prev,
        selectedIndexes: []
      };
    });
  }, [activeChatId]);

  const isVisibleForActiveChat = useMemo(() => {
    if (!searchState.visible) return false;
    if (activeChatId == null) return false;
    return Number(activeChatId) === Number(searchState.chatId);
  }, [searchState.visible, searchState.chatId, activeChatId]);

  const toggleResult = useCallback((index) => {
    setSearchState((prev) => {
      const exists = prev.selectedIndexes.includes(index);

      return {
        ...prev,
        selectedIndexes: exists
          ? prev.selectedIndexes.filter((x) => x !== index)
          : [...prev.selectedIndexes, index]
      };
    });
  }, []);

  const clearLocalState = useCallback(() => {
    setSearchState(initialState);
  }, []);

  const startSearch = useCallback(async (query, chatId) => {
    if (!connection || !query?.trim() || !chatId) return;

    setSearchState((prev) => ({
      ...prev,
      loading: true,
      error: "",
      chatId: Number(chatId)
    }));

    await connection.invoke("SendMessage", `/googling ${query.trim()}`, chatId.toString());
  }, [connection]);

  const stopSearch = useCallback(async (chatId) => {
    if (!connection || !chatId) return;
    await connection.invoke("SendMessage", "/stop", chatId.toString());
  }, [connection]);

  const sendSelected = useCallback(async (chatId) => {
    if (!connection || !chatId) return;
    if (searchState.selectedIndexes.length === 0) return;

    await connection.invoke("SendSearchSelection", {
      chatId: chatId.toString(),
      selectedIndexes: searchState.selectedIndexes
    });
  }, [connection, searchState.selectedIndexes]);

  return {
    searchState,
    isVisibleForActiveChat,
    toggleResult,
    clearLocalState,
    startSearch,
    stopSearch,
    sendSelected
  };
}