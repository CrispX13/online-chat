import { createContext } from "react"
export const MessagesContext = createContext({
  messages: [],
  AddMessage: () => {},
  SetAllMessages: () => {},
});
