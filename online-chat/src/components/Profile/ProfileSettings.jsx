
import React, { useContext, useState } from "react";
import { ContactsContext } from "../ContactService/ContactsContext"
import { useProfile } from "./ProfileProvider";

export default function ProfileSettings() {
  const { account } = useProfile();
  const { updateName, updatePassword, updateAvatar } = useContext(ContactsContext);

  const [name, setName] = useState(account?.name ?? "");
  const [lastPassword, setLastPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [avatarPreview, setAvatarPreview] = useState(null);
  const [avatarFile, setAvatarFile] = useState(null);

  const [savingName, setSavingName] = useState(false);
  const [savingPassword, setSavingPassword] = useState(false);
  const [savingAvatar, setSavingAvatar] = useState(false);

    const handleAvatarChange = (e) => {
    const file = e.target.files && e.target.files[0];
    if (!file) return;
    setAvatarFile(file);
    setAvatarPreview(URL.createObjectURL(file));
    };


  const handleSaveAvatar = async () => {
    if (!avatarFile) return;
    setSavingAvatar(true);
    try {
      await updateAvatar(avatarFile);
      // после успешного обновления сбросим локальную превью
      setAvatarFile(null);
      setAvatarPreview(null);
    } catch (e) {
      console.error(e);
      alert("Ошибка при загрузке аватарки");
    } finally {
      setSavingAvatar(false);
    }
  };

  const handleSaveName = async () => {
    if (!name.trim()) return;
    setSavingName(true);
    try {
      await updateName(name.trim());
      alert("Имя обновлено");
    } catch (e) {
      console.error(e);
      alert("Ошибка при изменении имени");
    } finally {
      setSavingName(false);
    }
  };

  const handleSavePassword = async () => {
    if (!lastPassword || !newPassword) return;
    setSavingPassword(true);
    try {
      await updatePassword(lastPassword, newPassword);
      setLastPassword("");
      setNewPassword("");
      alert("Пароль обновлён");
    } catch (e) {
      console.error(e);
      alert("Ошибка при изменении пароля");
    } finally {
      setSavingPassword(false);
    }
  };

  return (
    <div className="ProfileSettings">
      <h2>Настройки профиля</h2>

      {/* Аватар */}
      <section className="ProfileSettings__block">
        <h3>Аватар</h3>
        <div className="ProfileSettings__avatar-row">
          <img
            className="ProfileSettings__avatar-preview"
            src={
              avatarPreview ??
              `/api/profile/${account?.id}/avatar?${Date.now()}`
            }
            alt="Аватар"
          />
          <div className="ProfileSettings__avatar-controls">
            <input
              id="avatar-input"
              type="file"
              accept="image/*"
              onChange={handleAvatarChange}
              className="ProfileSettings__file-input"
            />

            <label htmlFor="avatar-input" className="ProfileSettings__file-label">
              Выбрать файл
            </label>

            <button
              onClick={handleSaveAvatar}
              disabled={!avatarFile || savingAvatar}
            >
              {savingAvatar ? "Сохранение..." : "Сохранить аватар"}
            </button>
          </div>
        </div>
      </section>

      {/* Имя */}
      <section className="ProfileSettings__block">
        <h3>Имя</h3>
        <input
          className="ProfileSettings__input"
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Имя"
        />
        <button onClick={handleSaveName} disabled={savingName}>
          {savingName ? "Сохранение..." : "Сохранить имя"}
        </button>
      </section>

      {/* Пароль */}
      <section className="ProfileSettings__block">
        <h3>Пароль</h3>
        <input
          className="ProfileSettings__input"
          type="password"
          placeholder="Старый пароль"
          value={lastPassword}
          onChange={(e) => setLastPassword(e.target.value)}
        />
        <input
          className="ProfileSettings__input"
          type="password"
          placeholder="Новый пароль"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
        />
        <button onClick={handleSavePassword} disabled={savingPassword}>
          {savingPassword ? "Сохранение..." : "Сменить пароль"}
        </button>
      </section>
    </div>
  );
}
