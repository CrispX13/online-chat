import { useState, useContext } from "react";
import ChatCard from "./ChatCard";
import { ContactsContext } from "../ContactService/ContactsContext";
import { useModal } from "../ModalService/ModalProvider";
import CreateGroupModal from "./CreateGroupModal/";

export default function Chats({ onOpenChat }) {
  const [styleActive, setStyleActive] = useState(null);
  const [chatType, setChatType] = useState("direct"); // direct по дефолту

  const { contacts, groupChats } = useContext(ContactsContext);
  const { openModal } = useModal();

  const handleOpenCreateGroup = () => {
    openModal(<CreateGroupModal />);
  };

  const directCards = contacts.map((element) => {
    const contact = {
      id: element.contact.id,
      name: element.contact.name,
      chatId: element.chatId,
      newNotifications: element.newNotifications,
      newContact: element.newContact,
      type: "direct",
    };

    return (
      <ChatCard
        styleActive={styleActive}
        setStyleActive={setStyleActive}
        key={`direct-${contact.chatId}`}
        contact={contact}
        onOpenChat={onOpenChat}
      />
    );
  });

  const groupCards = groupChats.map((group) => {
    const contact = {
      id: group.id,          // для группы id можно приравнять к chatId
      name: group.name,
      chatId: group.id,
      newNotifications: group.newNotifications,
      newContact: false,
      type: "group",
      membersCount: group.membersCount,
    };

    return (
      <ChatCard
        styleActive={styleActive}
        setStyleActive={setStyleActive}
        key={`group-${group.id}`}
        contact={contact}
        onOpenChat={onOpenChat}
      />
    );
  });

  return (
    <div className="Chats__container">
      <div className="Chats__header">
        <h3>Чаты</h3>
        {chatType === "group" && (
          <button
            type="button"
            className="Chats__create-group-btn"
            onClick={handleOpenCreateGroup}
          >
            + Группа
          </button>
        )}
      </div>

      <div className="Chats__toggle">
        <label className={`Chats__toggle-item ${chatType === "direct" ? "active" : ""}`}>
          <input
            type="radio"
            name="chatType"
            value="direct"
            checked={chatType === "direct"}
            onChange={() => setChatType("direct")}
          />
          Директ
        </label>

        <label className={`Chats__toggle-item ${chatType === "group" ? "active" : ""}`}>
          <input
            type="radio"
            name="chatType"
            value="group"
            checked={chatType === "group"}
            onChange={() => setChatType("group")}
          />
          Групповые
        </label>
      </div>

      <ul className="Chats__list">
        {chatType === "direct" ? directCards : groupCards}
      </ul>
    </div>
  );
}