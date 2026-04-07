import { useEffect, useRef, useContext, useState } from "react";
import ChatHeading from "./ChatHeading";
import ChatBody from "./CenterChatBar/ChatSide";
import ChatFooter from "./ChatFooter";
import "./CenterSideBar.css";
import { MessagesContext } from "../MessagesService/MessagesContext";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import EmojiPicker from "emoji-picker-react";
import MiniProfile from "../Profile/MiniProfile";

export default function CenterSideBar({ onBack = null }) {
  const [inputText, setInputText] = useState("");

  const isMobile = window.innerWidth < 1050;

  const handleEmojiClick = (emojiData) => {
    setInputText((prev) => prev + emojiData.emoji);
  };

  const { messages, SetAllMessages } = useContext(MessagesContext);
  const messagesEndRef = useRef(null);
  const { activeUser } = useContext(SignalRContext);

  useEffect(() => {
    if (activeUser == null) return;

    fetch(`/api/Chat/${activeUser.chatId}/messages`, {
      method: "GET",
      credentials: "include",
    })
      .then(async (response) => {
        if (!response.ok) {
          console.error("Failed to load messages", response.status);
          return [];
        }
        return await response.json();
      })
      .then((json) => {
        SetAllMessages(json);
      })
      .catch((e) => {
        console.error(e);
        SetAllMessages([]);
      });
  }, [activeUser]);

  useEffect(() => {
    if (!isMobile && messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({
        behavior: "smooth",
        block: "end",
      });
    }
  }, [messages, isMobile]);


  return (
    <div className="CenterSideBar__container">
      <div className="Chat__container">
        {activeUser && (
          <div className="ChatHeading__container">
            {onBack && (
              <button
                className="ChatHeading__back-btn"
                type="button"
                onClick={onBack}
              >
                ← Назад
              </button>
            )}
            <ChatHeading />
          </div>
        )}

        <ChatBody ref={messagesEndRef} />
        <ChatFooter text={inputText} setText={setInputText} />
      </div>

      {!isMobile && (
        <div className="CenterSideBar__RigthSide">
          <div className="EmojiSidebar">
            <EmojiPicker
              onEmojiClick={handleEmojiClick}
              height="100%"
              previewConfig={{ showPreview: false }}
            />
          </div>
          <MiniProfile />
        </div>
      )}

    </div>
  );
}
