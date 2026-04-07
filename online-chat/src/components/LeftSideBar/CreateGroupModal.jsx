import { useContext, useState } from "react";
import { useModal } from "../ModalService/ModalProvider";
import { SignalRContext } from "../SignalRConf/SignalRContext";
import { useContactSearch } from "../LeftSideBar/useContactSearch"; // путь под себя

export default function CreateGroupModal() {
  const { closeModal } = useModal();
  const { setActiveUser } = useContext(SignalRContext);

  const [name, setName] = useState("");
  const [selectedIds, setSelectedIds] = useState(new Set());

  const { query, setQuery, results, loading } = useContactSearch("");

  const toggleUser = (id) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleCreate = async (e) => {
    e.preventDefault();

    const trimmedName = name.trim();
    if (!trimmedName || selectedIds.size === 0) return;

    const body = {
      name: trimmedName,
      participantIds: Array.from(selectedIds),
    };

    const res = await fetch(`/api/Chat/group`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      console.error("Ошибка создания группового чата", res.status);
      return;
    }

    const chat = await res.json();
    setActiveUser(null);
    closeModal();
  };

  return (
    <div className="CreateGroupModal">
      <h2>Создать групповой чат</h2>

      <form onSubmit={handleCreate} className="CreateGroupModal__form">
        <div className="CreateGroupModal__field">
          <label>Название группы</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Например, Друзья, Проект X..."
          />
        </div>

        <div className="CreateGroupModal__field">
          <label>Участники</label>
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Поиск по пользователям"
          />

          {loading && <div>Поиск...</div>}

          <div className="CreateGroupModal__contacts-list">
            {(results || []).map((c) => {
              const id = c.id; // /api/contacts/search возвращает Contact напрямую
              const checked = selectedIds.has(id);

              return (
                <label
                  key={id}
                  className={`CreateGroupModal__contact ${
                    checked ? "selected" : ""
                  }`}
                >
                  <input
                    type="checkbox"
                    checked={checked}
                    onChange={() => toggleUser(id)}
                  />
                  <span>{c.name}</span>
                </label>
              );
            })}
          </div>
        </div>

        <div className="CreateGroupModal__actions">
          <button
            type="submit"
            disabled={!name.trim() || selectedIds.size === 0}
          >
            Создать
          </button>
        </div>
      </form>
    </div>
  );
}