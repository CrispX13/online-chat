import { useContext, useState, useRef, useEffect } from "react";
import { AuthContext } from "../../AuthContext";
import { SignalRContext } from "../../SignalRConf/SignalRContext";
import { MessagesContext } from "../../MessagesService/MessagesContext";

export default function Message({ info, index }) {
  const { userId } = useContext(AuthContext);
  const {connection} = useContext(SignalRContext)
  const { setEditingMessage } = useContext(MessagesContext);


  const isoString = info.messageDateTime;
  const date = new Date(isoString);
  const hours = date.getHours();
  const minutes = date.getMinutes();

  const [menuVisible, setMenuVisible] = useState(false);
  const [menuPos, setMenuPos] = useState({ x: 0, y: 0 });
  const menuRef = useRef(null);
  const longPressTimer = useRef(null);
  const LONG_PRESS_MS = 500;

  const canManage = String(info.fromUserId) === String(userId);

  const openMenu = (x, y) => {
    if (!canManage) return;

    // сначала ставим примерные координаты и показываем
    setMenuPos({ x, y });
    setMenuVisible(true);

    // во время следующего кадра измеряем и двигаем
    requestAnimationFrame(() => {
      if (!menuRef.current) return;

      const rect = menuRef.current.getBoundingClientRect();
      const { innerWidth, innerHeight } = window;

      let newX = x;
      let newY = y;

      // если справа не влазит — прижимаем к правому краю или переносим влево
      if (rect.width + x > innerWidth) {
        newX = Math.max(0, innerWidth - rect.width - 8);
      }

      // если снизу не влазит — поднимаем вверх
      if (rect.height + y > innerHeight) {
        newY = Math.max(0, innerHeight - rect.height - 8);
      }

      setMenuPos({ x: newX, y: newY });
    });
  };


  const closeMenu = () => {
    setMenuVisible(false);
  };

  const handleContextMenu = (e) => {
    e.preventDefault();
    openMenu(e.clientX, e.clientY);
  };

  const handleMouseDown = (e) => {
    // длинное нажатие для тач/стилуса
    if (e.pointerType === "mouse") return;
    longPressTimer.current = setTimeout(() => {
      const touch = e.touches ? e.touches[0] : e;
      openMenu(touch.clientX, touch.clientY);
    }, LONG_PRESS_MS);
  };

  const handleMouseUpLeave = () => {
    if (longPressTimer.current) {
      clearTimeout(longPressTimer.current);
      longPressTimer.current = null;
    }
  };

  const handleMenuAction = async (action) => {
    closeMenu();
    if (action === "delete") {
      await connection.invoke("DeleteMessage",info.id);
    }
    if (action === "edit") {
      setEditingMessage(info)
    }
  };

  // Клик/тап вне меню -> закрыть
  useEffect(() => {
    if (!menuVisible) return;

    const handleOutside = (e) => {
      if (!menuRef.current) return;
      if (!menuRef.current.contains(e.target)) {
        closeMenu();
      }
    };

    document.addEventListener("mousedown", handleOutside);
    document.addEventListener("touchstart", handleOutside);

    return () => {
      document.removeEventListener("mousedown", handleOutside);
      document.removeEventListener("touchstart", handleOutside);
    };
  }, [menuVisible]);

  return (
    <>
      <div
        key={index}
        className={`message ${
          userId !== String(info.toUserId) ? " message_right" : ""
        }`}
        onContextMenu={handleContextMenu}
        onMouseDown={handleMouseDown}
        onTouchStart={handleMouseDown}
        onMouseUp={handleMouseUpLeave}
        onMouseLeave={handleMouseUpLeave}
        onTouchEnd={handleMouseUpLeave}
      >
        <p className="message__text">{info.messageText}</p>
        <span className="message__time">
          {`${hours}:${minutes.toString().padStart(2, "0")}`}
        </span>
      </div>

      {menuVisible && (
        <div
          ref={menuRef}
          className="message-menu message-menu--enter"
          style={{ top: menuPos.y, left: menuPos.x }}
        >
          <ul className="message-menu__list">
            <li
              className="message-menu__item"
              onClick={() => handleMenuAction("edit")}
            >
              ✏️ Редактировать
            </li>
            <li
              className="message-menu__item message-menu__item--danger"
              onClick={() => handleMenuAction("delete")}
            >
              🗑️ Удалить
            </li>
          </ul>
        </div>
      )}
    </>
  );
}
