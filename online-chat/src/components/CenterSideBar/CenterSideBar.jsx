import { useEffect, useRef, useContext, useState } from "react";
import ChatHeading from "./ChatHeading";
import ChatBody from "./CenterChatBar/ChatSide";
import ChatFooter from "./ChatFooter";
import "./CenterSideBar.css";
import { MessagesContext } from "../MessagesService/MessagesContext";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import EmojiPicker from "emoji-picker-react";

import ChatSearchPanel from "./ChatSearchPanel";
import { useChatSearchSignalR } from "../hooks/useChatSearchSignalR";

export default function CenterSideBar({ onBack = null }) {
  const [inputText, setInputText] = useState("");
  const [isEmojiOpen, setIsEmojiOpen] = useState(false);

  const isMobile = window.innerWidth < 1050;

  const handleEmojiClick = (emojiData) => {
    setInputText((prev) => prev + emojiData.emoji);
  };

  const { messages, SetAllMessages } = useContext(MessagesContext);
  const messagesEndRef = useRef(null);
  const { activeUser, connection } = useContext(SignalRContext);

  const activeChatId = activeUser?.chatId ?? null;

  const {
    searchState,
    isVisibleForActiveChat,
    toggleResult,
    startSearch,
    stopSearch,
    sendSelected
  } = useChatSearchSignalR(connection, activeChatId);

  useEffect(() => {
    if (activeUser == null) return;

    fetch(`/api/Chat/${activeUser.chatId}/messages`, {
      method: "GET",
      credentials: "include",
    })
      .then(async (response) => {
        if (!response.ok) {
          console.error("Failed to load messages", response.status);
          return [];
        }
        return await response.json();
      })
      .then((json) => {
        SetAllMessages(json);
      })
      .catch((e) => {
        console.error(e);
        SetAllMessages([]);
      });
  }, [activeUser, SetAllMessages]);

  useEffect(() => {
    if (!isMobile && messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({
        behavior: "smooth",
        block: "end",
      });
    }
  }, [messages, isMobile]);

  return (
    <div className="CenterSideBar__container">
      <div className="Chat__container">
        {activeUser && (
          <div className="ChatHeading__container">
            {onBack && (
              <button
                className="ChatHeading__back-btn"
                type="button"
                onClick={onBack}
              >
                ← Назад
              </button>
            )}
            <ChatHeading />
          </div>
        )}

        <ChatBody ref={messagesEndRef} />

        <ChatSearchPanel
          visible={isVisibleForActiveChat}
          query={searchState.query}
          results={searchState.results}
          selectedIndexes={searchState.selectedIndexes}
          error={searchState.error}
          onToggle={toggleResult}
          onStop={() => activeChatId && stopSearch(activeChatId)}
          onSendSelected={() => activeChatId && sendSelected(activeChatId)}
        />

        <ChatFooter
          text={inputText}
          setText={setInputText}
          startSearch={startSearch}
          stopSearch={stopSearch}
          isEmojiOpen={isEmojiOpen}
          setIsEmojiOpen={setIsEmojiOpen}
        />
      </div>

      {!isMobile && (
        <div className="CenterSideBar__RigthSide">
          {isEmojiOpen && (
            <div className="EmojiSidebar">
              <EmojiPicker
                onEmojiClick={handleEmojiClick}
                height="100%"
                previewConfig={{ showPreview: false }}
              />
            </div>
          )}
        </div>
      )}
    </div>
  );
}