import { useState, useRef, useContext } from "react"
import { SignalRContext } from "../SignalRConf/SignalRContext"
import { DialogContext } from "../DialogService/DialogContext"

export default function ChatInput({ text, setText })
{
    const {dialogKey} = useContext(DialogContext)
    const {activeUser,connection} = useContext(SignalRContext)
    const taRef = useRef(null)

    const textChange = (event) => {
        const el = event.target
        el.style.height = "40px"
        el.style.height = el.scrollHeight + "px"
        setText(el.value)
    }
    
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
        if (!trimmed) return; // защита от пустой отправки

        await connection.invoke(
        "SendMessage",
        trimmed,
        String(dialogKey),
        String(activeUser.id)
        );

        setText("");
        if (taRef.current) {
            taRef.current.value = "";
            taRef.current.style.height = "50px";
        }
    };

    const isEmpty = text.trim().length === 0;

    return (
        <div className="ChatInput__wrapper">
            <textarea
                ref={taRef}
                value={text}
                onKeyDown={onKeyDown}
                onChange={textChange}
                placeholder="Write a massage..."
                className="ChatInput"
            />
            <button
                type="button"
                className="ChatInput__send-btn"
                onClick={submit}
                disabled={isEmpty}
            >
                ➤
            </button>
        </div>
    );
}