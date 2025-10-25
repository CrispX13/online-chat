import ChatInput from "./ChatInput"

export default function ChatFooter({connRef,setRefetch,dialogKey})
{
    return(
        <div className="ChatFooter__container">
            <ChatInput connRef={connRef} dialogKey= {dialogKey} setRefetch={setRefetch} ></ChatInput>
        </div>
    )
}