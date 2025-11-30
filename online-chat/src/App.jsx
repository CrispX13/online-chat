import { useContext, useEffect, useState } from 'react'
import LeftSideBar from './components/LeftSideBar/LeftSideBar'
import CenterSideBar from './components/CenterSideBar/CenterSideBar'
import { AuthContext } from './components/AuthContext'
import { SignalRContext } from './components/SignalRConf/SignalRContext';


function App() {
  // id диалога, который открыт в данный момент
  const [dialogKey, setDialogKey] = useState()

  const { jwtKey, userId } = useContext(AuthContext)
  const {activeUser} = useContext(SignalRContext)
  // список контактов для пользователя
  const [contacts,setContacts] = useState([])

  // при каждом выборе нового пользователя, подгружается id диалога с ним
  useEffect(() => {
    if (activeUser != null) {
      // получение id для диалога
      fetch(`/api/Dialog?UserKey1=${userId}&UserKey2=${activeUser.id}`, {
        method: "GET",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${jwtKey}`, }
      }
      )
        .then(response => response.json())
        .then(json => setDialogKey(json.dialogKey.id));
      
      setContacts(prev=>
              prev.map(contact=>contact.id === activeUser.id?{ ...contact, notification: false }:contact)
          )
    }
  }, [activeUser])

  return (
    <>
      <LeftSideBar contacts={contacts} setContacts={setContacts}></LeftSideBar>
      <CenterSideBar  setContacts={setContacts} contact={activeUser} dialogKey={dialogKey}></CenterSideBar>
    </>

  )
}

export default App
