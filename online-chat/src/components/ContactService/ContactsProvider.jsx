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
    const {activeUser} = useContext(SignalRContext)

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
            console.log(contacts)
     },[jwtKey, userId, refresh])

    useEffect(()=>{
        
        let notifFlag = false
        contacts.forEach((element)=>{
            if(element.id === activeUser.id){
                notifFlag = true
            }                                                                                                                                  
        })

        if (notifFlag){
            setContacts(prev=>
                prev.map(contact=>contact.id === activeUser.id?{ ...contact, notification: false }:contact)
            )
        }
    },[activeUser])

    const value = { contacts, setContacts,refreshContacts};

    return <ContactsContext.Provider value={value}>
        {children}
    </ContactsContext.Provider> 
}