import { createContext, useState } from "react";
import { AuthContext } from "./AuthContext";

export default function AuthProvider({children}){

    const [jwtKey,setJwtKey] = useState();
    const [userId, setUserId] = useState();

    const value = {jwtKey,setJwtKey,userId,setUserId}

    return <AuthContext.Provider value={value}>
        {children}
    </AuthContext.Provider>
}