import { useContext, useEffect, useState } from 'react'
import LeftSideBar from './components/LeftSideBar/LeftSideBar'
import CenterSideBar from './components/CenterSideBar/CenterSideBar'
import { AuthContext } from './components/AuthContext'
import { SignalRContext } from './components/SignalRConf/SignalRContext';


function App() {

  return (
    <>
      <LeftSideBar></LeftSideBar>
      <CenterSideBar></CenterSideBar>
    </>

  )
}

export default App
