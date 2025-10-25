import { forwardRef } from "react";
import Message from "./Message"

const ChatBody =  forwardRef(function ChatBody({messages},messagesEndRef){


    let messageCards = []

    messages.forEach((element,index) => {
        messageCards.push(<Message key={index} info = {element}></Message>)
    });


    return(
        <div className="ChatBody_container">
            {messageCards}
            <div ref={messagesEndRef}></div>
        </div>
    ) 
})

export default ChatBody