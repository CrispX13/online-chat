import React, { useCallback, useEffect } from 'react'
import { useState, useContext } from "react";
import { AuthContext } from '../AuthContext';
import { ContactsContext } from "./ContactsContext";
import { SignalRContext } from '../SignalRConf/SignalRContext';

export default function ContactsProvider({children}){
    
    const [contacts, setContacts] = useState([])
    const [refresh, setRefresh] = useState(true)

    const refreshContacts = ()=> setRefresh(!refresh)

    const {jwtKey, userId} = useContext(AuthContext)
    const {activeUser,connection,isConnected} = useContext(SignalRContext)
    
    const clearNewContactFlag = (id) => {
        setContacts(prev =>
            prev.map(c =>
            c.contact.id === id ? { ...c, newContact: false } : c
            )
        );
    };

    useEffect(()=>{
        // получаю список контактов для конкретного пользователя
        fetch(`/api/contacts/all-for-id/${userId}`,{
                     method: "GET",
                     headers: { 
                         "Content-Type": "application/json" ,
                         Authorization: `Bearer ${jwtKey}`,
                     }})
               .then(response => response.json())
               .then(json => setContacts(json)
            )
     },[jwtKey, userId, refresh])

    useEffect(()=>{
        
        console.log(contacts)
        let notifFlag = false
        contacts.forEach((element)=>{
            if(element.contact.id === activeUser.id){
                notifFlag = true
            }                                                                                                                                  
        })

        if (notifFlag){
            setContacts(prev=>
                prev.map(contact=>contact.contact.id === activeUser.id?{ ...contact, newNotifications: false }:contact)
            )
        }
    },[activeUser])

    useEffect(() => {

            if (!isConnected || !connection) return;

            connection.on("NewDialog", refreshContacts);

            return () => {
                connection.off("NewDialog", refreshContacts);
            };

        }, [isConnected, connection]
    
    );

    const value = { contacts, setContacts,refreshContacts,clearNewContactFlag};

    return <ContactsContext.Provider value={value}>
        {children}
    </ContactsContext.Provider> 
}