import { useState, useRef, useContext } from "react"
import { AuthContext } from "../AuthContext"

export default function ChatInput({connRef,setRefetch,dialogKey})
{
    const {userId} = useContext(AuthContext)
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
       
        const conn = connRef.current;
        await conn.invoke("SendMessage", text.trim(), String(dialogKey), String(userId));

        // const message = {
        //     dialogId: dialogKey,
        //     textMessage: text.trim()
        // }

        // const res = await fetch("/api/chat", {
        // method: "POST",
        // headers: { "Content-Type": "application/json", Authorization: `Bearer ${jwtKey}` },
        // body: JSON.stringify( message )
        // })

        // // console.log(res[[PromiseResult]])
        // if (res.ok){
        //     setText("");
        //     setRefetch(r => r+1)
        //     taRef.current.value = ""
        // }else{
        //     alert("Все хуета, переделывай")
        // }
    }

    return(
        <>
        <textarea ref={taRef} onKeyDown={onKeyDown} onChange={textChange} placeholder="Write a massage..." className="ChatInput" type="text" />
        </>
    )
}