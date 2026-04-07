import { useEffect, useState, useRef } from "react";

export function useContactSearch(initial = "") {
  const [query, setQuery] = useState(initial);
  const [results, setResults] = useState(null);
  const [loading, setLoading] = useState(false);

  const isFirstRender = useRef(true);

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }

    const trimmed = query?.trim();
    if (!trimmed) {
      setResults(null);
      return;
    }

    setLoading(true);

    fetch(`/api/contacts/search`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(trimmed),
    })
      .then((response) => {
        if (!response.ok) throw new Error("Ошибка поиска контактов");
        return response.json();
      })
      .then((json) => setResults(json))
      .catch((e) => {
        console.error(e);
        setResults(null);
      })
      .finally(() => setLoading(false));
  }, [query]);

  return { query, setQuery, results, loading };
}