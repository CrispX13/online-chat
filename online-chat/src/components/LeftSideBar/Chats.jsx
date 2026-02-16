import { useState, useContext } from "react";
import ChatCard from "./ChatCard";
import { ContactsContext } from "../ContactService/ContactsContext";

export default function Chats({ onOpenChat }) {
  const [styleActive, setStyleActive] = useState(null);
  const { contacts } = useContext(ContactsContext);

  const ChatCards = contacts.map(element => {
    const contact = {
      id: element.contact.id,
      name: element.contact.name,
      newNotifications: element.newNotifications,
      newContact: element.newContact
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

  return (
    <div className="Chats__container">
      <ul className="Chats__list">
        {ChatCards}
      </ul>
    </div>
  );
}
