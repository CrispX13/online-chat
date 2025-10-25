import ChatCard from "./ChatCard";

export default function SearchResultBlock({setSearchValue,refreshContacts,setAnotherUserKey,searchResult}){
    let ChatCards = []
    if(searchResult != null && searchResult.length>0){
            searchResult.forEach(element => {
                ChatCards.push(<ChatCard setSearchValue={setSearchValue} refreshContacts={refreshContacts} isSearch={true} setAnotherUserKey = {setAnotherUserKey} key={element.id} contact = {element}></ChatCard>)
            });
            return (
            <ul className="Search__result-container">
                {ChatCards}
            </ul>
            );

    }
}