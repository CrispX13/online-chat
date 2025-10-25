import { useState, useEffect, useContext } from "react"
import ChatCard from "./ChatCard"

export default function Chats({contacts,setAnotherUserKey}){
    const [styleActive, setStyleActive] = useState(null)


    let ChatCards = []

    contacts.forEach(element => {
        ChatCards.push(<ChatCard styleActive={styleActive} setStyleActive = {setStyleActive} setAnotherUserKey = {setAnotherUserKey} key={element.id} contact = {element}></ChatCard>)
    });
    

    return (
        <div className="Chats__container">
            <ul className="Chats__list">
                {ChatCards}
            </ul>
        </div>
    )
}