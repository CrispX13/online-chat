import Search from "./Search";
import Chats from "./Chats";
import "./LeftSideBarStyles.css";

export default function LeftSideBar({ onOpenChat }) {
  return (
    <div className="LeftSideBar__container">
      <Search />
      <Chats onOpenChat={onOpenChat} />
    </div>
  );
}
