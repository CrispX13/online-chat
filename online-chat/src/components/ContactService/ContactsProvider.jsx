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

    const updateName = async (newName) => {
        if (!userId) return;
        try {
        const res = await fetch(
            `/api/profile/change-name/${userId}`,
            {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${jwtKey}`,
            },
            body: JSON.stringify(newName),
            }
        );
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

    // смена пароля
    const updatePassword = async (lastPassword, newPassword) => {
        if (!userId) return;
        try {
        const res = await fetch(
            `/api/profile/change-password/${userId}`,
            {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${jwtKey}`,
            },
            body: JSON.stringify({
                lastPassword,
                newPassword,
            }),
            }
        );
        if (!res.ok) throw new Error("Ошибка при смене пароля");
        } catch (e) {
        console.error(e);
        throw e;
        }
    };

    // смена аватарки
    const updateAvatar = async (file) => {
        if (!userId || !file) return;
        try {
        const formData = new FormData();
        formData.append("file", file);

        await fetch(`/api/profile/${userId}/avatar`, {
            method: "POST",
            headers: {
                Authorization: `Bearer ${jwtKey}`,
            },
            body: formData,
            // если UploadAvatar будет с [Authorize], добавь заголовок:
            // headers: { Authorization: `Bearer ${jwtKey}` }
        });

        // чтобы аватарка в списке обновилась (если ты хранишь путь),
        // можно либо перезагрузить контакты, либо менять поле avatarUrl локально.
        refreshContacts();
        } catch (e) {
        console.error(e);
        throw e;
        }
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

    

    const value = {
        contacts,
        setContacts,
        refreshContacts,
        clearNewContactFlag,
        updateName,
        updatePassword,
        updateAvatar,
    };
    return <ContactsContext.Provider value={value}>
        {children}
    </ContactsContext.Provider> 
}