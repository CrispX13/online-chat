import { forwardRef, useContext } from "react";
import Message from "./Message"
import { MessagesContext } from "../../MessagesService/MessagesContext";
import { SignalRContext } from "../../SignalRConf/SignalRContext";

const ChatBody =  forwardRef(function ChatBody(props,messagesEndRef){

    const {messages} = useContext(MessagesContext)
    const { activeUser } = useContext(SignalRContext);
    

    let messageCards = []

    console.log(messages)

    if(messages.length > 0){
        if(messages[0].chatId === activeUser.chatId){
            messages.forEach((element,index) => {
                messageCards.push(<Message key={index} info = {element}></Message>)
            });
        }
    }

    console.log(messageCards)


    return(
        <div className="ChatBody_container">
            {messageCards}
            <div ref={messagesEndRef}></div>
        </div>
    ) 
})

export default ChatBody