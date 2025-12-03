import { useState, useRef, useContext } from "react"
import { SignalRContext } from "../SignalRConf/SignalRContext"

export default function ChatInput({dialogKey})
{
    const {activeUser,connection} = useContext(SignalRContext)
    const taRef = useRef(null)
    const [text,setText] = useState("")
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
        taRef.current.style.height = "40px"
        }

        // Альтернатива: Ctrl/Cmd+Enter — тоже отправка
        if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
        e.preventDefault();
        submit();
        taRef.current.style.height = "40px"
        }
    }

    const submit = async () => {
       
        console.log(text.trim(),String(dialogKey), String(activeUser.id))
        await connection.invoke("SendMessage", text.trim(), String(dialogKey), String(activeUser.id));
    }

    return(
        <>
        <textarea ref={taRef} onKeyDown={onKeyDown} onChange={textChange} placeholder="Write a massage..." className="ChatInput" type="text" />
        </>
    )
}