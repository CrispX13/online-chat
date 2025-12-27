import Search from "./Search" 
import Chats from "./Chats"
import "./LeftSideBarStyles.css"

export default function LeftSideBar(){
    return(
        <div className="LeftSideBar__container">
            <Search></Search>
            <Chats></Chats>
        </div>
    )
}