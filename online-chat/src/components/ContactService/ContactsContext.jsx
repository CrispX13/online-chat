import { createContext } from "react"
export const ContactsContext = createContext(
    {
        contacts:[],setContacts:()=>{},
        refreshContacts:() => {}
    })   