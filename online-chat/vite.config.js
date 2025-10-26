import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

<<<<<<< HEAD
// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7050',  // или http://localhost:5103
        changeOrigin: true,
        secure: false, // dev-сертификат
      },
      "/hubs": {
        target: "https://localhost:7050",
        changeOrigin: true,
        secure: false,
        ws: true, // ВАЖНО для WebSocket
      },
    },
  },
  devtool: "source-map",
})

=======
export default defineConfig({
  plugins: [react()],
  server: {
    host: true, // или "0.0.0.0" — слушать все IP интерфейсы
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7050', // твой ASP.NET Core backend
        changeOrigin: true,
        secure: false,
      },
      '/hubs': {
        target: 'https://localhost:7050',
        changeOrigin: true,
        secure: false,
        ws: true,
      },
    },
  },
  devtool: 'source-map',
})


>>>>>>> newcontent
// ipconfig - что бы узнать мой Ipv4

// server: {
//   host: true,
//   port: 5173,
//   hmr: {
//     host: '192.168.1.10', // твой LAN-IP
//     protocol: 'ws',       // по умолчанию
//     port: 5173            // обычно тот же
//   }
// }

// Разовый запуск
// npm run dev -- --host 

