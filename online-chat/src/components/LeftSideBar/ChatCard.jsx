import { useContext, useRef } from "react"
import { AuthContext } from "../AuthContext"
export default function ChatCard({setSearchValue=null,refreshContacts=null,isSearch = false,contact, setAnotherUserKey,setStyleActive=null,styleActive=false}){
    const {jwtKey,userId} = useContext(AuthContext)
    const liRef = useRef()
    return (
        <li ref={liRef} onClick={() => {setAnotherUserKey(contact)
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
        }} className={`ChatCard ${styleActive === contact.id ? "active-chat" : ""}`}>
            <img className="ChatCard__img" src={null} alt="Аватарка" />
            <div className="ChatCard__text-container">
                <h3 className="ChatCard__name">{contact.name}</h3>
                <span className="ChatCard__last-message"></span>
            </div>
        </li>
    )
}