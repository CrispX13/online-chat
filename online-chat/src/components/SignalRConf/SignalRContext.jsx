import { createContext } from "react"
export const SignalRContext = createContext(
    {
        activeUser:null,setActiveUser:()=>{},
        connection:null, isConnected: false
    }
)   