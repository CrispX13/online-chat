import ChatInput from "./ChatInput"

export default function ChatFooter({ text, setText })
{
    return(
        <div className="ChatFooter__container">
            <ChatInput text={text} setText={setText}/>
        </div>
    )
}