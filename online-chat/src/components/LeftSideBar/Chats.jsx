import { useState, useContext } from "react"
import ChatCard from "./ChatCard"
import {ContactsContext} from "../ContactService/ContactsContext"

export default function Chats(){
    const [styleActive, setStyleActive] = useState(null)
    const {contacts} = useContext(ContactsContext)

    let ChatCards = []

    contacts.forEach(element => {
        let contact = {
                id: element.contact.id,
                name:element.contact.name,
                newNotifications: element.newNotifications
        }
        ChatCards.push(<ChatCard styleActive={styleActive} setStyleActive = {setStyleActive} key={contact.id} contact = {contact}></ChatCard>)
    });
    

    return (
        <div className="Chats__container">
            <ul className="Chats__list">
                {ChatCards}
            </ul>
        </div>
    )
}