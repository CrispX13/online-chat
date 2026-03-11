import { createContext } from "react";

export const MessagesContext = createContext({
  messages: [],
  AddMessage: () => {},
  SetAllMessages: () => {},
  DeleteMessageLocal: () => {},
  EditMessageLocal: () => {},
  editingMessage: null,
  setEditingMessage: () => {},
});
