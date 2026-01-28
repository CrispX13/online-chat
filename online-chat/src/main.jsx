import React from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import AuthGate from './components/AuthGate.jsx'
import AuthProvider from './components/AuthProvider.jsx'
import SignalRProvider from './components/SignalRConf/SignalRProvider.jsx'
import MessagesProvider from "./components/MessagesService/MessagesProvider.jsx"
import DialogProvider from "./components/DialogService/DialogProvider.jsx"
import ContactsProvider from "./components/ContactService/ContactsProvider.jsx"
import { ModalProvider } from './components/ModalService/ModalProvider.jsx'
import { ProfileProvider } from './components/Profile/ProfileProvider.jsx'

createRoot(document.getElementById('root')).render(

  <AuthProvider>
    <SignalRProvider>
      <ContactsProvider>
        <DialogProvider>
          <MessagesProvider>
            <ProfileProvider>
              <ModalProvider>
                <AuthGate/>
              </ModalProvider>
            </ProfileProvider>
          </MessagesProvider>
        </DialogProvider>
      </ContactsProvider>
    </SignalRProvider>
  </AuthProvider>

)
