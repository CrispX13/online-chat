import React, { useContext } from "react";
import { AuthContext } from "./AuthContext";
import AuthModalContainer from "./AuthModalFolder/AuthModalContainer";
import App from "../App.jsx";

export default function AuthGate() {
  const { userId, isLoadingAuth } = useContext(AuthContext);

  if (isLoadingAuth) {
    // можно вернуть спиннер / пустой экран
    return <div>Загрузка...</div>;
  }

  if (userId == null) {
    return <AuthModalContainer />;
  }

  return <App />;
}
