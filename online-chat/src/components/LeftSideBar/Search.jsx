import { useContext } from "react";
import { useContactSearch } from "./useContactSearch";
import SearchResultBlock from "./SearchResultBlock";
import ProfileAvatarButton from "../Profile/ProfileAvatarButton";

export default function Search() {
  const { query, setQuery, results } = useContactSearch("");

  return (
    <div className="Search">
      <div className="Search_container">
        <ProfileAvatarButton />
        <input
          onChange={(e) => setQuery(e.target.value)}
          className="Search__input"
          type="text"
          placeholder="Search"
          value={query}
        />
        <SearchResultBlock
          setSearchValue={setQuery}
          searchResult={results}
        />
      </div>
    </div>
  );
}