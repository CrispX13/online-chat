import React, { useEffect, useState, useContext } from "react";
import { SignalRContext } from "./SignalRContext";
import { AuthContext } from "/src/components/AuthContext";
import * as signalR from "@microsoft/signalr";

export default function SignalRProvider({ children }) {
  const { jwtKey } = useContext(AuthContext);
  const [activeUser, setActiveUser] = useState(null);
  const [connection, setConnection] = useState(null);
  const [isConnected, setConnected] = useState(false);
  const [participants, setParticipants] = useState({}); 

  useEffect(() => {
    if (!jwtKey) {
      // если токена нет, отрубаем подключение
      if (connection) {
        connection.stop();
        setConnection(null);
        setConnected(false);
      }
      return;
    }

    const conn = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/chat", {
        accessTokenFactory: () => jwtKey, // сюда уходит твой JWT
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.None)
      .build();

    setConnection(conn);

    conn
      .start()
      .then(() => {
        setConnected(true);
      })
      .catch((err) => {
        console.error("SignalR connect error", err);
        setConnected(false);
      });

    return () => {
      conn.stop();
      setConnected(false);
    };
  }, [jwtKey]);

  useEffect(() => {
    if (!activeUser?.chatId) return;

    fetch(`/api/chat/${activeUser.chatId}/participants`, {
      method: "GET",
      credentials: "include",
    })
      .then((res) => {
        if (!res.ok) throw new Error("Ошибка загрузки участников");
        return res.json();
      })
      .then((list) => {
        // list: [{ userId, name, avatarUrl }, ...]
        const map = {};
        for (const u of list) {
          map[u.userId] = { name: u.name, avatarUrl: u.avatarUrl };
        }
        setParticipants(map);
      })
      .catch(console.error);
  }, [activeUser?.chatId]);

  const value = { activeUser, setActiveUser, connection, isConnected, participants, setParticipants };

  return (
    <SignalRContext.Provider value={value}>
      {children}
    </SignalRContext.Provider>
  );
}
