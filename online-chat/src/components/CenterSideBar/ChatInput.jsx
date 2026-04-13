import { useEffect, useRef, useContext } from "react";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import { MessagesContext } from "../MessagesService/MessagesContext";

export default function ChatInput({ text, setText, startSearch, stopSearch }) {
  const { activeUser, connection } = useContext(SignalRContext);
  const { editingMessage, setEditingMessage } = useContext(MessagesContext);
  const taRef = useRef(null);

  const textChange = (event) => {
    const el = event.target;
    el.style.height = "40px";
    el.style.height = el.scrollHeight + "px";
    setText(el.value);
  };

  useEffect(() => {
    if (editingMessage) {
      setText(editingMessage.messageText ?? "");
      if (taRef.current) {
        taRef.current.style.height = "40px";
        taRef.current.style.height = taRef.current.scrollHeight + "px";
      }
    }
  }, [editingMessage, setText]);

  const onKeyDown = (e) => {
    if (e.isComposing) return;

    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      submit();
      if (taRef.current) taRef.current.style.height = "50px";
    }

    if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
      e.preventDefault();
      submit();
      if (taRef.current) taRef.current.style.height = "50px";
    }
  };

  const submit = async () => {
    const trimmed = text.trim();
    if (!trimmed) return;

    const chatId = activeUser?.chatId;
    if (!chatId) return;

    try {
      if (editingMessage) {
        await connection.invoke("EditMessage", editingMessage.id, trimmed);
        setEditingMessage(null);
      } else {
        if (trimmed.startsWith("/googling ")) {
          const query = trimmed.replace("/googling", "").trim();
          await startSearch?.(query, chatId);
        } else if (trimmed === "/stop") {
          await stopSearch?.(chatId);
        } else {
          await connection.invoke(
            "SendMessage",
            trimmed,
            String(chatId)
          );
        }
      }
    } finally {
      setText("");
      if (taRef.current) {
        taRef.current.value = "";
        taRef.current.style.height = "50px";
      }
    }
  };

  const isEmpty = text.trim().length === 0;
  const isEditing = Boolean(editingMessage);

  return (
    <div style={{ width: "100%" }}>
      {isEditing && (
        <div className="ChatInput__edit-banner">
          Редактирование сообщения
          <button
            type="button"
            className="ChatInput__edit-cancel"
            onClick={() => {
              setEditingMessage(null);
              setText("");
              if (taRef.current) {
                taRef.current.value = "";
                taRef.current.style.height = "50px";
              }
            }}
          >
            ✕
          </button>
        </div>
      )}

      <div className="ChatInput__wrapper">
        <textarea
          ref={taRef}
          value={text}
          onKeyDown={onKeyDown}
          onChange={textChange}
          placeholder={
            isEditing
              ? "Измените сообщение..."
              : "Write a message or /googling something..."
          }
          className="ChatInput"
        />
        <button
          type="button"
          className="ChatInput__send-btn"
          onClick={submit}
          disabled={isEmpty}
        >
          {isEditing ? "✔" : "➤"}
        </button>
      </div>
    </div>
  );
}