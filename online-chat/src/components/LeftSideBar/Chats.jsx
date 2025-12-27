import { useState, useContext } from "react"
import ChatCard from "./ChatCard"
import {ContactsContext} from "../ContactService/ContactsContext"

export default function Chats(){
    const [styleActive, setStyleActive] = useState(null)
    const {contacts} = useContext(ContactsContext)

    let ChatCards = []

    contacts.forEach(element => {
        ChatCards.push(<ChatCard styleActive={styleActive} setStyleActive = {setStyleActive} key={element.id} contact = {element}></ChatCard>)
    });
    

    return (
        <div className="Chats__container">
            <ul className="Chats__list">
                {ChatCards}
            </ul>
        </div>
    )
}