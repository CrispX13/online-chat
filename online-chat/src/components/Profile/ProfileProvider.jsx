
import {
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";
import { AuthContext } from "../AuthContext";

export const ProfileContext = createContext(null);

export function ProfileProvider({ children }) {
  const [account, setAccount] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const {jwtKey, userId} = useContext(AuthContext);

  // 1) загрузка данных аккаунта
  useEffect(() => {
    if (!jwtKey) return;          // нет токена — не грузим

    setLoading(true);
    setError(null);

    fetch(`/api/contacts/${userId}`, {
      headers: {
        credentials: "include",
      },
    })
      .then((r) => {
        if (!r.ok) throw new Error("Failed to load account");
        return r.json();
      })
      .then((data) => setAccount(data))
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [jwtKey]);

  // 2) изменить имя
  const updateName = async (name) => {
    const res = await fetch("/api/account/name", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify({ name }),
    });
    if (!res.ok) throw new Error("Cannot update name");
    const updated = await res.json();
    setAccount((prev) => ({ ...prev, name: updated.name }));
  };

  // 3) изменить пароль
  const updatePassword = async (oldPassword, newPassword) => {
    const res = await fetch("/api/account/password", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify({ oldPassword, newPassword }),
    });
    if (!res.ok) throw new Error("Cannot update password");
    // пароль в состоянии обычно не храним
  };

  // 4) изменить фото (аватар)
  const updateAvatar = async (file) => {
    const formData = new FormData();
    formData.append("avatar", file);

    const res = await fetch("/api/account/avatar", {
      method: "PUT",
      credentials: "include",
      body: formData,
    });
    if (!res.ok) throw new Error("Cannot update avatar");
    const updated = await res.json();
    setAccount((prev) => ({ ...prev, avatarUrl: updated.avatarUrl }));
  };

  const value = {
    account,
    loading,
    error,
    updateName,
    updatePassword,
    updateAvatar,
  };

  return (
    <ProfileContext.Provider value={value}>
      {children}
    </ProfileContext.Provider>
  );
}

// удобный хук
export function useProfile() {
  const ctx = useContext(ProfileContext);
  if (!ctx) {
    throw new Error("useProfile must be used within ProfileProvider");
  }
  return ctx;
}
