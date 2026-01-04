import {useEffect, useRef, useContext, useState} from "react"
import ChatHeading from "./ChatHeading"
import ChatBody from "./CenterChatBar/ChatSide"
import ChatFooter from "./ChatFooter"
import "./CenterSideBar.css"
import {AuthContext} from "../AuthContext"
import {MessagesContext} from "../MessagesService/MessagesContext"
import {DialogContext} from "../DialogService/DialogContext"
import { SignalRContext } from "../SignalRConf/SignalRContext"
import EmojiPicker from 'emoji-picker-react';

export default function CenterSideBar(){

    // useState для текста сообщения и эмодзи 
    const [inputText, setInputText] = useState("");

    const handleEmojiClick = (emojiData) => {
        setInputText((prev) => prev + emojiData.emoji);
    };

    const {messages,SetAllMessages} = useContext(MessagesContext)
    const messagesEndRef = useRef(null)
    const {dialogKey} = useContext(DialogContext)
    const {activeUser} = useContext(SignalRContext)

    const {jwtKey,userId} = useContext(AuthContext)

    // Получаю сообщения из диалога для активного контакта
    useEffect(() => {
        if(dialogKey != null){
            fetch(`/api/chat?DialogKey=${dialogKey}&UserId=${userId}`,{
                method: "GET",
                headers: { "Content-Type": "application/json" , Authorization: `Bearer ${jwtKey}`},
            })
              .then(response => response.json())
              .then(json => {SetAllMessages(json);}
            );
        }
    }, [dialogKey])

    useEffect(() => {
            messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
    }, [messages])
  
    return (
        <div className="CenterSideBar__container">
            <div className="Chat__container">
                {activeUser && (<ChatHeading></ChatHeading>)}
                <ChatBody ref={messagesEndRef}></ChatBody>
                <ChatFooter
                    text={inputText}
                    setText={setInputText}
                />
            </div>
            <div className="EmojiSidebar">
                <EmojiPicker
                onEmojiClick={handleEmojiClick}
                height="100%"      
                previewConfig={{
                    showPreview: false
                }}
                />
            </div>
        </div>
    )
}