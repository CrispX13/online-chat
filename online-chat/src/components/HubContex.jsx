import { createContext, useContext } from "react";
import { AuthContext } from "../AuthContext"

const HubContext = createContext(null)

export function HubProvider({children}){
    const [state, setState] = useState("disconnected")
    const connectionRef = useRef(null)
    const pedingGroupRef = useRef(new Set())
    const {jwtKey} = useContext(AuthContext)

    useEffect(()=>{
        if(!jwtKey) return

        setState("connecting")
        const conn = new signalR.HubConnectionBuilder()
        .wuthUrl("/api/hubs/chat",{
                headers: { "Content-Type": "application/json",Authorization: `Bearer ${jwtKey}`,}})
        .withAutomaticReconnect()
        .build()


    })
}
