import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import AuthGate from './components/AuthGate.jsx'
import AuthProvider from './components/AuthProvider.jsx'

createRoot(document.getElementById('root')).render(

    <AuthProvider>
      <AuthGate/>
    </AuthProvider>

)
