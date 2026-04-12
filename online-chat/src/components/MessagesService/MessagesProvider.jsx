import React, { useCallback, useEffect, useState, useContext } from "react";
import { MessagesContext } from "./MessagesContext";
import { AuthContext } from "../AuthContext";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import { ContactsContext } from "../ContactService/ContactsContext";

export default function MessagesProvider({ children }) {
  const { userId } = useContext(AuthContext);
  const { connection, isConnected, activeUser } = useContext(SignalRContext);
  const { setContacts, refreshContacts } = useContext(ContactsContext);

  const [messages, setMessages] = useState([]);
  const [editingMessage, setEditingMessage] = useState(null);

  const AddMessage = useCallback(
    (message) => {
      setMessages((prev) => [...prev, message]);

      // если сообщение не в текущем открытом чате
      if (!activeUser || activeUser.chatId !== message.chatId) {
        // DIRECT: если это личное сообщение мне
        if (String(userId) === String(message.toUserId)) {
          setContacts((prev) =>
            prev.map((c) =>
              c.contact.id === message.fromUserId
                ? { ...c, newNotifications: true }
                : c
            )
          );
        }

        // GROUP: обновляем список групп / контактов с сервера
        // (флаги NewNotifications уже обновил бэкенд)
        if (message.toUserId == null) {
          refreshContacts(); // или отдельный refreshGroupChats()
        }
      }
    },
    [activeUser, userId, setContacts, refreshContacts]
  );

  const SetAllMessages = useCallback((arr) => {
    setMessages(arr);
  }, []);

  const DeleteMessageLocal = useCallback((id) => {
    setMessages((prev) => prev.filter((m) => m.id !== id));
  }, []);

  const EditMessageLocal = useCallback((updatedMessage) => {
    setMessages((prev) =>
      prev.map((m) => (m.id === updatedMessage.id ? updatedMessage : m))
    );
  }, []);

  useEffect(() => {
    if (!isConnected || !connection) return;

    const handleCreated = (message) => {
      AddMessage(message);
    };

    connection.on("MessageCreated", handleCreated);
    connection.on("MessageDeleted", DeleteMessageLocal);
    connection.on("MessageEdited", EditMessageLocal);

    return () => {
      connection.off("MessageCreated", handleCreated);
      connection.off("MessageDeleted", DeleteMessageLocal);
      connection.off("MessageEdited", EditMessageLocal);
    };
  }, [isConnected, connection, AddMessage, DeleteMessageLocal, EditMessageLocal]);

  const value = {
    messages,
    AddMessage,
    SetAllMessages,
    DeleteMessageLocal,
    EditMessageLocal,
    setEditingMessage,
    editingMessage,
  };

  return (
    <MessagesContext.Provider value={value}>
      {children}
    </MessagesContext.Provider>
  );
}