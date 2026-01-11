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

    const AddMessage = (message) => {
        setMessages(prev => {
            return [...prev, message];
        });
        // отправка уведомления при новом сообщении 
        if (dialogKey !== message.dialogId ){
            if(userId === String(message.toUserId)){
                setContacts(prev=>
                    prev.map(contact=>contact.contact.id === message.fromUserId?{ ...contact, newNotifications: true }:contact)
                )
                console.log(contacts)
            }
        }
    };
            
    const SetAllMessages = useCallback((arr) => {
    setMessages(arr);
    }, []);

    useEffect(() => {

        if (!isConnected || !connection) return;

        connection.on("MessageCreated", AddMessage);

        return () => {
            connection.off("MessageCreated", AddMessage);
        };
        }, [AddMessage, isConnected, connection]

    );

    

    const value = { messages, AddMessage, SetAllMessages };

    return <MessagesContext.Provider value={value}>
        {children}
    </MessagesContext.Provider> 
}