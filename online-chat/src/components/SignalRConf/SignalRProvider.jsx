import React, { useEffect, useState, useContext } from "react";
import { SignalRContext } from "./SignalRContext";
import { AuthContext } from "/src/components/AuthContext";
import * as signalR from "@microsoft/signalr";

export default function SignalRProvider({ children }) {
  const { jwtKey } = useContext(AuthContext);
  const [activeUser, setActiveUser] = useState(null);
  const [connection, setConnection] = useState(null);
  const [isConnected, setConnected] = useState(false);

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
      .build();

    setConnection(conn);

    conn
      .start()
      .then(() => {
        setConnected(true);
        console.log("SignalR connected");
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

  const value = { activeUser, setActiveUser, connection, isConnected };

  return (
    <SignalRContext.Provider value={value}>
      {children}
    </SignalRContext.Provider>
  );
}
