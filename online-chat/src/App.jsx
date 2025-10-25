import { useContext, useEffect, useState, useRef, useCallback } from 'react'
import LeftSideBar from './components/LeftSideBar/LeftSideBar'
import CenterSideBar from './components/CenterSideBar/CenterSideBar'
import { AuthContext } from './components/AuthContext'
import { HubConnectionBuilder, HubConnectionState, LogLevel} from "@microsoft/signalr";

function App() {
  const [anotherUserKey, setAnotherUserKey] = useState()
  const [dialogKey, setDialogKey] = useState()
  const { jwtKey, userId } = useContext(AuthContext)
  const connRef = useRef(null);

  let [connection, setConnection] = useState(false);

  useEffect(() => {
    if (!jwtKey) return

    const conn = new HubConnectionBuilder()
      .withUrl("/hubs/chat", { accessTokenFactory: () => jwtKey })
      .withAutomaticReconnect()
      .build();

    connRef.current = conn;

    (async () => {
        await conn.start().then(setConnection(true))
        
    })();

  }, [jwtKey])


  useEffect(() => {
    if (anotherUserKey != null) {
      fetch(`/api/Dialog?UserKey1=${userId}&UserKey2=${anotherUserKey.id}`, {
        method: "GET",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${jwtKey}`, }
      }
      )
        .then(response => response.json())
        .then(json => setDialogKey(json.dialogKey.id));
    }
  }, [anotherUserKey])

  useEffect(()=>{
      const conn = connRef.current

      if (!dialogKey || !conn) return;
      if (conn.state !== HubConnectionState.Connected) return;
      (async () => {
        await conn.invoke("JoinDialog", String(dialogKey))
        })()
  },[dialogKey])


  return (
    <>
      <LeftSideBar setAnotherUserKey={setAnotherUserKey}></LeftSideBar>
      <CenterSideBar connection={connection} connRef={connRef} contact={anotherUserKey} dialogKey={dialogKey}></CenterSideBar>
    </>

  )
}

export default App


 // подключить логирование 
    // .configureLogging(LogLevel.Information)

    // также логи
    // console.log("typeof conn.on =", typeof conn.on, "is start a function?", typeof conn.start);

    // пример установки подписки на отправку сообщений
    // conn.on("MessageCreated", (payload) => {
    //   setTestMessages((prev) => [...prev, payload])
    // });

     // ___________________________________________
  // для input на тест
  // const submit = async () => {
  //   const conn = connRef.current;
  //   await conn.invoke("SendMessage", text, String(testDialog), String(userId ?? ""));
  // }
  // ____________________________________________
