import ChatInput from "./ChatInput"

export default function ChatFooter({dialogKey})
{
    return(
        <div className="ChatFooter__container">
            <ChatInput dialogKey= {dialogKey} ></ChatInput>
        </div>
    )
}