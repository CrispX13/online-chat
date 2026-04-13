import { useContext, useState, useEffect } from "react";
import { useModal } from "../ModalService/ModalProvider";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import { useContactSearch } from "../LeftSideBar/useContactSearch";

export default function CreateGroupModal() {
  const { closeModal, setShowCloseBottom } = useModal();
  const { setActiveUser } = useContext(SignalRContext);

  useEffect(() => {
    setShowCloseBottom(false);

    return () => {
      setShowCloseBottom(true);
    };
  }, [setShowCloseBottom]);

  const [name, setName] = useState("");
  const [selectedIds, setSelectedIds] = useState(new Set());
  const [selectedUsers, setSelectedUsers] = useState([]);

  const { query, setQuery, results, loading } = useContactSearch("");

  const toggleUser = (user) => {
    const id = user.id;

    setSelectedIds((prev) => {
      const next = new Set(prev);

      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }

      return next;
    });

    setSelectedUsers((prev) => {
      const exists = prev.some((u) => u.id === id);

      if (exists) {
        return prev.filter((u) => u.id !== id);
      }

      return [...prev, user];
    });
  };

  const removeUser = (id) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      next.delete(id);
      return next;
    });

    setSelectedUsers((prev) => prev.filter((u) => u.id !== id));
  };

  const handleCreate = async (e) => {
    e.preventDefault();

    const trimmedName = name.trim();
    if (!trimmedName || selectedIds.size === 0) return;

    const body = {
      name: trimmedName,
      participantIds: Array.from(selectedIds),
    };

    const res = await fetch("/api/Chat/group", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      console.error("Ошибка создания группового чата", res.status);
      return;
    }

    await res.json();
    setActiveUser(null);
    closeModal();
  };

  return (
    <div className="CreateGroupModal">
      <h2 className="CreateGroupModal__title">Создать групповой чат</h2>

      <form onSubmit={handleCreate} className="CreateGroupModal__form">
        <div className="CreateGroupModal__field">
          <label className="CreateGroupModal__label">Название группы</label>
          <input
            type="text"
            className="CreateGroupModal__input"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Например, Друзья, Проект X..."
          />
        </div>

        <div className="CreateGroupModal__field">
          <label className="CreateGroupModal__label">Участники</label>

          <input
            type="text"
            className="CreateGroupModal__input"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Поиск по пользователям"
          />

          {loading && (
            <div className="CreateGroupModal__loading">Поиск...</div>
          )}

          {selectedUsers.length > 0 && (
            <div className="CreateGroupModal__selected">
              {selectedUsers.map((user) => (
                <div key={user.id} className="CreateGroupModal__chip">
                  <span className="CreateGroupModal__chip-name">
                    {user.name}
                  </span>
                  <button
                    type="button"
                    className="CreateGroupModal__chip-remove"
                    onClick={() => removeUser(user.id)}
                    aria-label={`Удалить ${user.name}`}
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
          )}

          <div className="CreateGroupModal__contacts-list">
            {(results || []).map((c) => {
              const checked = selectedIds.has(c.id);

              return (
                <label
                  key={c.id}
                  className={`CreateGroupModal__contact ${
                    checked ? "selected" : ""
                  }`}
                >
                  <input
                    type="checkbox"
                    checked={checked}
                    onChange={() => toggleUser(c)}
                  />

                  <div className="CreateGroupModal__contact-main">
                    <span className="CreateGroupModal__contact-name">
                      {c.name}
                    </span>

                    {c.email && (
                      <span className="CreateGroupModal__contact-email">
                        {c.email}
                      </span>
                    )}
                  </div>
                </label>
              );
            })}
          </div>
        </div>

        <div className="CreateGroupModal__actions">
          <button
            type="button"
            className="CreateGroupModal__button CreateGroupModal__button--ghost"
            onClick={closeModal}
          >
            Отмена
          </button>

          <button
            type="submit"
            className="CreateGroupModal__button CreateGroupModal__button--primary"
            disabled={!name.trim() || selectedIds.size === 0}
          >
            Создать
          </button>
        </div>
      </form>
    </div>
  );
}