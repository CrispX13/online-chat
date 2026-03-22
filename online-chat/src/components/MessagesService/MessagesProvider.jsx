import React, { useCallback, useEffect } from 'react'
import { useState, useContext } from "react";
import { MessagesContext } from "./MessagesContext";
import * as signalR from "@microsoft/signalr";
import { AuthContext } from '../AuthContext';
import { SignalRContext } from "../SignalRConf/SignalRContext"
import { ContactsContext } from '../ContactService/ContactsContext';
import { DialogContext } from '../DialogService/DialogContext';

export default function MessagesProvider({children}){

    const {userId} = useContext(AuthContext)
    const {connection,isConnected} = useContext(SignalRContext)
    const [messages, setMessages] = useState([]);
    const {contacts,setContacts} = useContext(ContactsContext)
    const {dialogKey} = useContext(DialogContext)
    const [editingMessage, setEditingMessage] = useState(null);

    const AddMessage = (message) => {
    setMessages((prev) => [...prev, message]);

    // уведомление, если сообщение пришло НЕ в текущий чат
    if (dialogKey !== message.chatId) {
        if (String(userId) === String(message.toUserId)) {
            setContacts((prev) =>
                prev.map((c) =>
                c.contact.id === message.fromUserId
                    ? { ...c, newNotifications: true }
                    : c
                )
            );
        }
    }
    };
            
    const SetAllMessages = useCallback((arr) => {
    setMessages(arr);
    }, []);

    const DeleteMessageLocal = (id) => {
        setMessages(prev => prev.filter(m => m.id !== id));
    };

    const EditMessageLocal = (updatedMessage) => {
        setMessages(prev =>
            prev.map(m => (m.id === updatedMessage.id ? updatedMessage : m))
        );
    };


    useEffect(() => {
        if (!isConnected || !connection) return;

        connection.on("MessageCreated", AddMessage);
        connection.on("MessageDeleted", DeleteMessageLocal);
        connection.on("MessageEdited", EditMessageLocal);

        return () => {
            connection.off("MessageCreated", AddMessage);
            connection.off("MessageDeleted", DeleteMessageLocal);
            connection.off("MessageEdited", EditMessageLocal);
        };
    }, [AddMessage, isConnected, connection]);


    

    const value = { messages, AddMessage, SetAllMessages, DeleteMessageLocal, EditMessageLocal, setEditingMessage, editingMessage};

    return <MessagesContext.Provider value={value}>
        {children}
    </MessagesContext.Provider> 
}