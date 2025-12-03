import React, { useCallback, useEffect } from 'react'
import { useState, useContext } from "react";
import { MessagesContext } from "./MessagesContext";
import * as signalR from "@microsoft/signalr";
import { AuthContext } from '../AuthContext';
import { SignalRContext } from "../SignalRConf/SignalRContext"

export default function MessagesProvider({children}){

    const {userId} = useContext(AuthContext)
    const {activeUser} = useContext(SignalRContext)
    const [messages, setMessages] = useState([]);

    const AddMessage = useCallback((message) => {
        setMessages(prev => {
            return [...prev, message];
        });
        // отправка уведомления при новом сообщении 
        // if(userId !== String(message.userId)){
        //     setContacts(prev=>
        //         prev.map(contact=>contact.id === message.userId?{ ...contact, notification: true }:contact)
        //     )
        //     // setContacts([])
        // }
    }, []);

    //  [activeUser,userId]

    const SetAllMessages = useCallback((arr) => {
    setMessages(arr);
    }, []);

    const value = { messages, AddMessage, SetAllMessages };

    return <MessagesContext.Provider value={value}>
        {children}
    </MessagesContext.Provider> 
}