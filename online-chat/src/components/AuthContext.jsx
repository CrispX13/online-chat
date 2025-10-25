import { createContext } from "react"
export const AuthContext = createContext(
    {jwtKey:null,setJwtKey:()=>{}, 
    userId:null, setUserId:()=>{}
})   