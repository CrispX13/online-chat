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
       
        taRef.current.value = ""
        await connection.invoke("SendMessage", text.trim(), String(dialogKey), String(activeUser.id));
        setText(""); 
    }

    return(
        <>
        <textarea ref={taRef} value={text} onKeyDown={onKeyDown} onChange={textChange} placeholder="Write a massage..." className="ChatInput" type="text" />
        </>
    )
}