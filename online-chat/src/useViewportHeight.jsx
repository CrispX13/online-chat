import { useEffect, useState } from "react";

export function useViewportHeight() {
  const [vh, setVh] = useState(window.innerHeight);

  useEffect(() => {
    const handler = () => {
      setVh(window.innerHeight);
    };

    window.addEventListener("resize", handler);
    window.addEventListener("orientationchange", handler);

    handler(); // первый вызов

    return () => {
      window.removeEventListener("resize", handler);
      window.removeEventListener("orientationchange", handler);
    };
  }, []);

  return vh;
}
