import { useContext, useRef } from "react"
import { AuthContext } from "../AuthContext"
import { SignalRContext } from "../SignalRConf/SignalRContext"
import { ContactsContext } from "../ContactService/ContactsContext"

export default function ChatCard(
        {
            setSearchValue=null,
            isSearch = false,
            contact,
            setStyleActive=null,
            styleActive=false
        }
){

    const {jwtKey,userId} = useContext(AuthContext)
    const {setActiveUser} = useContext(SignalRContext)
    const {refreshContacts} = useContext(ContactsContext)
    const liRef = useRef()

    const className = [
        "ChatCard",
        styleActive === contact.id && "active-chat",
        contact.newNotifications && "notification"
    ].filter(Boolean).join(" ");

    return (
        <li ref={liRef} onClick={() => {
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
            }else{
                setActiveUser(contact)
                if(contact.newContact){
                    refreshContacts()
                }
            }
        }} className={className}>
            <img className="ChatCard__img" src={null} alt="Аватарка" />
            <div className="ChatCard__text-container">
                <h3 className="ChatCard__name">{contact.name}</h3>
                <span className="ChatCard__last-message"></span>
            </div>
            {contact.newContact && (
                <div className="contact__badge">
                    <svg xmlns="http://www.w3.org/2000/svg" width="40" height="18" viewBox="0 0 40 18">
                        <rect x="0" y="0" width="40" height="18" rx="9" ry="9" fill="#16a34a"/>
                        <text x="20" y="12" text-anchor="middle" font-family="system-ui, sans-serif"
                                font-size="10" fill="#ffffff" font-weight="600">
                            New
                        </text>
                    </svg>
                </div>
            )}
        </li>
        
    )
}