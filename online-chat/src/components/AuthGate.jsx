import React,{useContext, useState} from "react"
import { AuthContext } from "./AuthContext";
import AuthModalContainer from "./AuthModalFolder/AuthModalContainer";
import App from '../App.jsx'

export default function AuthGate(){
    const {jwtKey} = useContext(AuthContext);

    if (jwtKey == null){
        return <AuthModalContainer/>;
    }

    return <App/>;
} 