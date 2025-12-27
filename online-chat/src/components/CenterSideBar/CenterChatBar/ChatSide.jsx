import { forwardRef, useContext } from "react";
import Message from "./Message"
import { MessagesContext } from "../../MessagesService/MessagesContext";
import { DialogContext } from "../../DialogService/DialogContext";

const ChatBody =  forwardRef(function ChatBody(props,messagesEndRef){

    const {dialogKey} = useContext(DialogContext)
    const {messages} = useContext(MessagesContext)

    let messageCards = []

    if(messages.length > 0){
        if(messages[0].dialogId === dialogKey){
            messages.forEach((element,index) => {
                messageCards.push(<Message key={index} info = {element}></Message>)
            });
        }
    }


    return(
        <div className="ChatBody_container">
            {messageCards}
            <div ref={messagesEndRef}></div>
        </div>
    ) 
})

export default ChatBody