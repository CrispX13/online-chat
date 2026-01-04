import { useContext, useEffect, useState } from 'react'
import LeftSideBar from './components/LeftSideBar/LeftSideBar'
import CenterSideBar from './components/CenterSideBar/CenterSideBar'

function App() {

  return (
    <>
      <LeftSideBar></LeftSideBar>
      <CenterSideBar></CenterSideBar>
    </>

  )
}

export default App
