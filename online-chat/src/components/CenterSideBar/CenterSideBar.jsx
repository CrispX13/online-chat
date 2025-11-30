import { useState, useEffect, useRef, useContext, useCallback} from "react"
import ChatHeading from "./ChatHeading"
import ChatBody from "./CenterChatBar/ChatSide"
import ChatFooter from "./ChatFooter"
import "./CenterSideBar.css"
import {AuthContext} from "../AuthContext"
import { SignalRContext } from "../SignalRConf/SignalRContext"

export default function CenterSideBar({ setContacts,contact,dialogKey}){
    const [messages,setMessages] = useState([])
    const messagesEndRef = useRef(null)

    const {jwtKey, userId} = useContext(AuthContext)
    const {connection,isConnected} = useContext(SignalRContext)

    // Получаю сообщения из диалога для активного контакта
    useEffect(() => {
        if(dialogKey != null){
            fetch(`/api/chat?DialogKey=${dialogKey}`,{
                method: "GET",
                headers: { "Content-Type": "application/json" , Authorization: `Bearer ${jwtKey}`},
            })
              .then(response => response.json())
              .then(json => setMessages(json))
        }
    }, [dialogKey])

    const AddMessage = useCallback((message) => {
        setMessages(prev => {
            // опционально: дедуп по id, если сервер его шлёт
            // if (prev.some(m => m.id === message.id)) return prev;
            return [...prev, message];
        });
        console.log(messages)
        // отправка уведомления при новом сообщении 
        if(userId !== String(message.userId)){
            setContacts(prev=>
                prev.map(contact=>contact.id === message.userId?{ ...contact, notification: true }:contact)
            )
            // setContacts([])
        }
    }, [setContacts,userId]);

    useEffect(()=>{
        if(isConnected){
            if(!connection) return
    
            connection.on("MessageCreated", AddMessage)
        }
    },[AddMessage,isConnected])

    useEffect(() => {
            messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
    }, [messages])
  
    return (
        <div className="CenterSideBar__container">
            {contact ? <ChatHeading>{contact}</ChatHeading> : null}
            <ChatBody ref={messagesEndRef} messages={messages}></ChatBody>
            <ChatFooter dialogKey={dialogKey} ></ChatFooter>
        </div>
    )
}