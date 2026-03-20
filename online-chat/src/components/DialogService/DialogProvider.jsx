import {useEffect } from 'react'
import { useState, useContext } from "react";
import { SignalRContext } from '../SignalRConf/SignalRContext';
import { DialogContext } from '../DialogService/DialogContext';
import { AuthContext } from '../AuthContext';

export default function DialogProvider({children}){
    
    const [dialogKey, setDialogKey] = useState(null)
    const {activeUser} = useContext(SignalRContext)
    const {userId} = useContext(AuthContext)

      // при каждом выборе нового пользователя, подгружается id диалога с ним
    useEffect(() => {
        if (activeUser != null) {
        // получение id для диалога
        fetch(`/api/Chat/direct?UserKey1=${userId}&UserKey2=${activeUser.id}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            })
            .then((response) => {
                if (!response.ok) {
                // если чата ещё нет или ошибка
                setDialogKey(null);
                return null;
                }
                return response.json();
            })
            .then((json) => {
                if (!json) return;
                // бэкенд: return Ok(new { dialogKey });
                setDialogKey(json.chatKey.id);
            })
            .catch((e) => {
                console.error(e);
                setDialogKey(null);
            });
        }

        // setContacts(prev=>
        //     prev.map(contact=>contact.id === activeUser.id?{ ...contact, notification: false }:contact)
        // )
    }, [activeUser])

    const value = { dialogKey, setDialogKey};

    return <DialogContext.Provider value={value}>
        {children}
    </DialogContext.Provider> 
}