import ChatInput from "./ChatInput";

export default function ChatFooter({ text, setText, startSearch, stopSearch, isEmojiOpen, setIsEmojiOpen }) {
  return (
    <div className="ChatFooter__container">
      <ChatInput
        text={text}
        setText={setText}
        startSearch={startSearch}
        stopSearch={stopSearch}
        isEmojiOpen={isEmojiOpen}
        setIsEmojiOpen={setIsEmojiOpen}
      />
    </div>
  );
}