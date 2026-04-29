import { useState } from "react";
import Search from "./Search";
import Chats from "./Chats";
import MiniProfile from "../Profile/MiniProfile";
import "./LeftSideBarStyles.css";

export default function LeftSideBar({ onOpenChat }) {
  const [isCollapsed, setIsCollapsed] = useState(false);

  return (
    <div className={`LeftSideBar__container ${isCollapsed ? "collapsed" : ""}`}>
      {/* Верхний блок: профиль + поиск + кнопка сворачивания */}
      {!isCollapsed && (
        <>
          <MiniProfile />
          <Search />
        </>
      )}

      {isCollapsed && (
        <div className="LeftSideBar__collapsed-header">
          <button
            type="button"
            className="LeftSideBar__collapsed-search"
            aria-label="Поиск"
          >
            🔍
          </button>
        </div>
      )}

      {/* Список чатов */}
      <div className={`Chats__container ${isCollapsed ? "collapsed" : ""}`}>
        <Chats onOpenChat={onOpenChat} />
      </div>

      {/* Кнопка сворачивания/разворачивания внизу (или где хочешь) */}
      <button
        type="button"
        className="LeftSideBar__collapse-toggle"
        onClick={() => setIsCollapsed((prev) => !prev)}
        aria-label={isCollapsed ? "Развернуть левую панель" : "Свернуть левую панель"}
      >
        {isCollapsed ? "❯" : "❮"}
      </button>
    </div>
  );
}