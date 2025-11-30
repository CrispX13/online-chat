import { useContext, useRef } from "react"
import { AuthContext } from "../AuthContext"
import { SignalRContext } from "../SignalRConf/SignalRContext"
export default function ChatCard({setSearchValue=null,refreshContacts=null,isSearch = false,contact,setStyleActive=null,styleActive=false}){
    const {jwtKey,userId} = useContext(AuthContext)
    const {setActiveUser} = useContext(SignalRContext)
    const liRef = useRef()
    return (
        <li ref={liRef} onClick={() => {setActiveUser(contact)
            if(setStyleActive!= null)
                setStyleActive(contact.id)
            if(isSearch){
                fetch("/api/Dialog",
                    {
                        method: "POST",
                        headers: { 
                            "Content-Type": "application/json" ,
                            Authorization: `Bearer ${jwtKey}`,
                        },
                        body:JSON.stringify({
                            userKey1:userId,
                            userKey2:contact.id
                        })
                    }
                ).then(()=>{refreshContacts();
                    setSearchValue(null)
                })
            }
        }} className={`ChatCard ${styleActive === contact.id ? "active-chat" : ""} ${contact.notification ? "notification" : ""}`}>
            <img className="ChatCard__img" src={null} alt="Аватарка" />
            <div className="ChatCard__text-container">
                <h3 className="ChatCard__name">{contact.name}</h3>
                <span className="ChatCard__last-message"></span>
            </div>
        </li>
    )
}