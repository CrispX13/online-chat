import { forwardRef, useContext } from "react";
import Message from "./Message";
import { MessagesContext } from "../../MessagesService/MessagesContext";
import { SignalRContext } from "../../SignalRConf/SignalRContext";

const ChatBody = forwardRef(function ChatBody(props, messagesEndRef) {
  const { messages } = useContext(MessagesContext);
  const { activeUser } = useContext(SignalRContext);

  // если чат ещё не выбран
  if (!activeUser) {
    return (
      <div className="ChatBody_container">
        {/* Можно написать заглушку типа "Выберите чат" */}
      </div>
    );
  }

  const currentChatId = activeUser.chatId;

  const messageCards = messages
    .filter((m) => m.chatId === currentChatId)
    .map((m, index) => <Message key={m.id ?? index} info={m} />);

  return (
    <div className="ChatBody_container">
      {messageCards}
      <div ref={messagesEndRef}></div>
    </div>
  );
});

export default ChatBody;