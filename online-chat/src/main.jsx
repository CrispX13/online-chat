import React from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import AuthGate from './components/AuthGate.jsx'
import AuthProvider from './components/AuthProvider.jsx'
import SignalRProvider from './components/SignalRConf/SignalRProvider.jsx'
import MessagesProvider from "./components/MessagesService/MessagesProvider.jsx"

createRoot(document.getElementById('root')).render(

  <AuthProvider>
    <SignalRProvider>
      <MessagesProvider>
        <AuthGate/>
      </MessagesProvider>
    </SignalRProvider>
  </AuthProvider>

)
