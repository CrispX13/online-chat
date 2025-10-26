import { useState, useEffect, useRef, useContext, useCallback} from "react"
import ChatHeading from "./ChatHeading"
import ChatBody from "./CenterChatBar/ChatSide"
import ChatFooter from "./ChatFooter"
import "./CenterSideBar.css"
import {AuthContext} from "../AuthContext"

export default function CenterSideBar({connection,connRef,contact,dialogKey}){
    const [messages,setMessages] = useState([])
    const [refetch, setRefetch] = useState(0)
    const messagesEndRef = useRef(null)

    const {jwtKey} = useContext(AuthContext)

    useEffect(() => {
        if(dialogKey != null){
            fetch(`/api/chat?DialogKey=${dialogKey}`,{
                method: "GET",
                headers: { "Content-Type": "application/json" , Authorization: `Bearer ${jwtKey}`},
            })
              .then(response => response.json())
              .then(json => setMessages(json))
        }
    }, [refetch,dialogKey])

    const AddMessage = useCallback((message) => {
    setMessages(prev => {
        // опционально: дедуп по id, если сервер его шлёт
        // if (prev.some(m => m.id === message.id)) return prev;
<<<<<<< HEAD
        console.log([...prev, message])
=======
>>>>>>> newcontent
        return [...prev, message];
    });
    console.log(messages)
    }, []);

    useEffect(()=>{
        if(connection===true){
            const conn = connRef?.current
            if(!conn) return
    
            conn.on("MessageCreated", AddMessage)
        }
    },[connRef, AddMessage,connection])

    useEffect(() => {
            messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
    }, [messages])
  
    return (
        <div className="CenterSideBar__container">
            {contact ? <ChatHeading>{contact}</ChatHeading> : null}
            <ChatBody ref={messagesEndRef} messages={messages}></ChatBody>
            <ChatFooter connRef={connRef} dialogKey={dialogKey} setRefetch={setRefetch}></ChatFooter>
        </div>
    )
}