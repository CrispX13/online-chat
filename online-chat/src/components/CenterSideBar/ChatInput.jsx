import { useEffect, useRef, useContext } from "react"
import { SignalRContext } from "../SignalRConf/SignalRContext"
import { DialogContext } from "../DialogService/DialogContext"
import { MessagesContext } from "../MessagesService/MessagesContext";

export default function ChatInput({ text, setText })
{
    const {activeUser,connection} = useContext(SignalRContext)
    const { editingMessage, setEditingMessage } = useContext(MessagesContext);
    const taRef = useRef(null)

    const textChange = (event) => {
        const el = event.target
        el.style.height = "40px"
        el.style.height = el.scrollHeight + "px"
        setText(el.value)
    }

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
        // не мешаем IME-композиции (китайский/японский и т.п.)
        if (e.isComposing) return;

        // Enter без шифта — отправка
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault(); // чтобы не вставлялся перенос строки
            submit();
            taRef.current.style.height = "50px"
            }

            // Альтернатива: Ctrl/Cmd+Enter — тоже отправка
        if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
            e.preventDefault();
            submit();
            taRef.current.style.height = "50px"
        }
    }

    const submit = async () => {
        const trimmed = text.trim();
        if (!trimmed) return;

        try {
        if (editingMessage) {
            // режим редактирования
            await connection.invoke(
            "EditMessage",
            editingMessage.id,
            trimmed
            );
            // после успешного редактирования выходим из режима редактирования
            setEditingMessage(null);
        } else {
            // обычная отправка нового сообщения
            console.log(activeUser)
            await connection.invoke(
            "SendMessage",
            trimmed,
            String(activeUser.chatId)
            );
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
            placeholder={isEditing ? "Измените сообщение..." : "Write a massage..."}
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