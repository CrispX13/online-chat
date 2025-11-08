import Search from "./Search" 
import Chats from "./Chats"
import "./LeftSideBarStyles.css"
import { AuthContext } from "../AuthContext"
import { useCallback,useState,useContext,useEffect} from "react"

export default function LeftSideBar({contacts,setContacts,setAnotherUserKey}){
     const {jwtKey,userId} = useContext(AuthContext)

     const fetchContacts = useCallback(()=>{
        // получаю список контактов для конкретного пользователя
        fetch(`/api/contacts/all-for-id/${userId}`,{
                     method: "GET",
                     headers: { 
                         "Content-Type": "application/json" ,
                         Authorization: `Bearer ${jwtKey}`,
                     }})
               .then(response => response.json())
               .then(json => setContacts(json)
               )
     },[jwtKey, userId])

     useEffect(() => {
        fetchContacts();
           }, [fetchContacts])
    return(
        <div className="LeftSideBar__container">
            <Search  refreshContacts={fetchContacts} setAnotherUserKey={setAnotherUserKey}></Search>
            <Chats contacts={contacts} setAnotherUserKey = {setAnotherUserKey}></Chats>
        </div>
    )
}