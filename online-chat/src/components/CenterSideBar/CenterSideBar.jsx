import { useState, useEffect, useRef, useContext, useCallback} from "react"
import ChatHeading from "./ChatHeading"
import ChatBody from "./CenterChatBar/ChatSide"
import ChatFooter from "./ChatFooter"
import "./CenterSideBar.css"
import {AuthContext} from "../AuthContext"
import {MessagesContext} from "../MessagesService/MessagesContext"
import { SignalRContext } from "../SignalRConf/SignalRContext"

export default function CenterSideBar({ setContacts,contact,dialogKey}){
    const {messages, AddMessage,SetAllMessages} = useContext(MessagesContext)
    const messagesEndRef = useRef(null)

    const {jwtKey} = useContext(AuthContext)
    const {connection,isConnected} = useContext(SignalRContext)

    // Получаю сообщения из диалога для активного контакта
    useEffect(() => {
        if(dialogKey != null){
            fetch(`/api/chat?DialogKey=${dialogKey}`,{
                method: "GET",
                headers: { "Content-Type": "application/json" , Authorization: `Bearer ${jwtKey}`},
            })
              .then(response => response.json())
              .then(json => {SetAllMessages(json);}
            );
        }
    }, [dialogKey])

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