import { useState, useEffect } from "react";
import Search from "./Search";
import Chats from "./Chats";
import MiniProfile from "../Profile/MiniProfile";
import "./LeftSideBarStyles.css";

export default function LeftSideBar({ onOpenChat }) {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth <= 1050);

  useEffect(() => {
    const onResize = () => {
      setIsMobile(window.innerWidth <= 1050);
    };
    window.addEventListener("resize", onResize);
    return () => window.removeEventListener("resize", onResize);
  }, []);

  // На мобилке игнорируем коллапс, всегда полный режим
  const collapsed = !isMobile && isCollapsed;

  return (
    <div className={`LeftSideBar__container ${collapsed ? "collapsed" : ""}`}>
      {/* Верхний блок: профиль + поиск */}
      {!collapsed && (
        <>
          <MiniProfile />
          <Search />
        </>
      )}

      {collapsed && (
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
      <div className={`Chats__container ${collapsed ? "collapsed" : ""}`}>
        <Chats onOpenChat={onOpenChat} />
      </div>

      {/* Кнопка сворачивания/разворачивания — скрыта на мобилке через CSS */}
      <button
        type="button"
        className="LeftSideBar__collapse-toggle"
        onClick={() => setIsCollapsed((prev) => !prev)}
        aria-label={collapsed ? "Развернуть левую панель" : "Свернуть левую панель"}
      >
        {collapsed ? "❯" : "❮"}
      </button>
    </div>
  );
}