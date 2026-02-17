import { useContext, useEffect, useState,useRef } from "react"
import { AuthContext } from "../AuthContext"
import SearchResultBlock from "./SearchResultBlock"
import ProfileAvatarButton from "../Profile/ProfileAvatarButton";

export default function Search(){
    const [searchValue,setSearchValue] = useState()
    const [searchResult,setSearchResult] = useState(null)
    const {jwtKey} = useContext(AuthContext)
    const isFirstRender = useRef(true);

    useEffect(()=>{
        if (isFirstRender.current) {
            isFirstRender.current = false; // пропускаем первый запуск
            return;
        }
        if (!searchValue?.trim()) {
            setSearchResult(null)
            return;
        }
        fetch(`/api/contacts/search`,{
                method: "POST",
                headers: { 
                    "Content-Type": "application/json" ,
                    Authorization: `Bearer ${jwtKey}`,
                },
                body: JSON.stringify(searchValue)
                })
          .then(response => response.json())
          .then(json => setSearchResult(json)
          )
    },[searchValue])


    return (
        <div className="Search">
            <div className="Search_container">
                <ProfileAvatarButton />
                <input onChange={(e)=>{setSearchValue(e.target.value)}} className="Search__input" type="text" placeholder="Search"/>
                <SearchResultBlock  setSearchValue={setSearchValue} searchResult={searchResult}></SearchResultBlock>
            </div>
        </div>
    )
}