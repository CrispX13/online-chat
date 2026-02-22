import React from 'react'
import { useState, useEffect } from "react";
import { AuthContext } from "./AuthContext";

export default function AuthProvider({children}){

    const [jwtKey,setJwtKey] = useState();
    const [userId, setUserId] = useState();
    const [isLoadingAuth, setIsLoadingAuth] = useState(true);

    useEffect(() => {
        fetch("/api/Auth/me", {
        method: "GET",
        credentials: "include",   // отправляем cookie accessToken
        })
        .then(res => (res.ok ? res.json() : null))
        .then(data => {
            if (data) {
            setUserId(data.id);
            setJwtKey(data.token);
            console.log(data.token)
            // при желании: setUserName(data.userName);
            } else {
            setUserId(null);
            }
        })
        .catch(() => {
            setUserId(null);
        })
        .finally(() => setIsLoadingAuth(false));
    }, []);

    const value = {
        jwtKey,
        setJwtKey,
        userId,
        setUserId,
        isLoadingAuth,
    };

    return (
        <AuthContext.Provider value={value}>
        {children}
        </AuthContext.Provider>
    );
}