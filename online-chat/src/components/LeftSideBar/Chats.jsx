import { useState, useContext } from "react";
import ChatCard from "./ChatCard";
import { ContactsContext } from "../ContactService/ContactsContext";
import { useModal } from "../ModalService/ModalProvider";
import CreateGroupModal from "./CreateGroupModal/";

export default function Chats({ onOpenChat }) {
  const [styleActive, setStyleActive] = useState(null);
  const { contacts } = useContext(ContactsContext);
  const { openModal } = useModal();

  const ChatCards = contacts.map((element) => {
    const contact = {
      id: element.contact.id,
      name: element.contact.name,
      chatId: element.chatId,
      newNotifications: element.newNotifications,
      newContact: element.newContact,
    };

    return (
      <ChatCard
        styleActive={styleActive}
        setStyleActive={setStyleActive}
        key={contact.id}
        contact={contact}
        onOpenChat={onOpenChat}
      />
    );
  });

  const handleOpenCreateGroup = () => {
    openModal(<CreateGroupModal />);
  };

  return (
    <div className="Chats__container">
      <div className="Chats__header">
        <h3>Чаты</h3>
        <button
          type="button"
          className="Chats__create-group-btn"
          onClick={handleOpenCreateGroup}
        >
          + Группа
        </button>
      </div>

      <ul className="Chats__list">{ChatCards}</ul>
    </div>
  );
}