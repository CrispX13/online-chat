import React, { useEffect } from 'react'
import { useState, useContext } from "react";
import { SignalRContext } from "./SignalRContext";
import { AuthContext } from '/src/components/AuthContext'
import * as signalR from "@microsoft/signalr";

export default function SignalRProvider({children}){

    const { jwtKey } = useContext(AuthContext)
    const [activeUser,setActiveUser] = useState();
    const [connection, setConnection] = useState();
    const [isConnected, getConnected] = useState();

    useEffect(()=>{
        if(jwtKey!==null){
            const conn = new signalR.HubConnectionBuilder()
                  .withUrl("/hubs/chat", { accessTokenFactory: () => jwtKey })
                  .withAutomaticReconnect()
                  .build();
            setConnection(conn)
    
            conn
                .start()
                .then(() => {
                    getConnected(true);
                    console.log("SignalR connected");
                })
                .catch((err) => console.error("SignalR connect error", err));
    
            return () => {
            conn.stop();
            };
        }
    },[jwtKey])

    const value = {activeUser,setActiveUser,connection,isConnected}

    return <SignalRContext.Provider value={value}>
        {children}
    </SignalRContext.Provider> 
}