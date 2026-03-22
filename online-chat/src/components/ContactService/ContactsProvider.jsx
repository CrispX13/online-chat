import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../AuthContext";
import { ContactsContext } from "./ContactsContext";
import { SignalRContext } from "../SignalRConf/SignalRContext";

export default function ContactsProvider({ children }) {
  const [contacts, setContacts] = useState([]);
  const [refresh, setRefresh] = useState(true);

  const refreshContacts = () => setRefresh((prev) => !prev);

  const { userId } = useContext(AuthContext);
  const { activeUser, connection, isConnected } = useContext(SignalRContext);

  const clearNewContactFlag = (id) => {
    setContacts((prev) =>
      prev.map((c) =>
        c.contact.id === id ? { ...c, newContact: false } : c
      )
    );
  };

  // смена имени текущего пользователя
  const updateName = async (newName) => {
    if (!userId) return;
    try {
      const res = await fetch(`/api/contacts/me/name`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({newName}), // string в теле
      });
      if (!res.ok) throw new Error("Ошибка при смене имени");

      // обновляем локальный список контактов (там, где текущий userId)
      setContacts((prev) =>
        prev.map((c) =>
          c.contact.id === userId
            ? { ...c, contact: { ...c.contact, name: newName } }
            : c
        )
      );
    } catch (e) {
      console.error(e);
      throw e;
    }
  };

  // смена пароля текущего пользователя
  const updatePassword = async (lastPassword, newPassword) => {
    if (!userId) return;
    try {
      const res = await fetch(`/api/contacts/me/password`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({
          lastPassword,
          newPassword,
        }),
      });
      if (!res.ok) throw new Error("Ошибка при смене пароля");
    } catch (e) {
      console.error(e);
      throw e;
    }
  };

  // смена аватарки текущего пользователя
  const updateAvatar = async (file) => {
    if (!userId || !file) return;
    try {
      const formData = new FormData();
      // имя поля должно совпадать с AvatarUploadDto.Avatar
      formData.append("avatar", file);

      await fetch(`/api/contacts/me/avatar`, {
        method: "POST",
        credentials: "include",
        body: formData,
      });

      // самый простой вариант — перезагрузить контакты
      refreshContacts();
    } catch (e) {
      console.error(e);
      throw e;
    }
  };

  // загрузка списка контактов текущего пользователя
  useEffect(() => {
    if (!userId) return;

    fetch(`/api/contacts/me/list`, {
      method: "GET",
      credentials: "include",
    })
      .then((response) => {
        if (!response.ok) throw new Error("Ошибка загрузки контактов");
        return response.json();
      })
      .then((json) => setContacts(json))
      .catch((e) => {
        console.error(e);
        setContacts([]);
      });
  }, [userId, refresh]);

  // сброс флага newNotifications при открытии диалога
  useEffect(() => {
        if (!activeUser || !contacts?.length) return;

        const hasWithNotif = contacts.some(
            (c) => c.contact.id === activeUser.id && c.newNotifications
        );
        if (!hasWithNotif) return;

        setContacts((prev) =>
            prev.map((contact) =>
            contact.contact.id === activeUser.id
                ? { ...contact, newNotifications: false }
                : contact
            )
        );
    }, [activeUser, contacts]);


  // обновление списка контактов по сигналу из SignalR
  useEffect(() => {
    if (!isConnected || !connection) return;

    connection.on("NewChat", refreshContacts);

    return () => {
      connection.off("NewChat", refreshContacts);
    };
  }, [isConnected, connection]);

  const value = {
    contacts,
    setContacts,
    refreshContacts,
    clearNewContactFlag,
    updateName,
    updatePassword,
    updateAvatar,
  };

  return (
    <ContactsContext.Provider value={value}>
      {children}
    </ContactsContext.Provider>
  );
}
