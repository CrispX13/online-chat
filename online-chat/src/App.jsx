import { useEffect, useState } from 'react';
import LeftSideBar from './components/LeftSideBar/LeftSideBar';
import CenterSideBar from './components/CenterSideBar/CenterSideBar';

function App() {
  const [isMobile, setIsMobile] = useState(false);
  const [view, setView] = useState('contacts');
  const [isSlidingOut, setIsSlidingOut] = useState(false);

  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth < 768;
      setIsMobile(mobile);
      if (!mobile) {
        setView('contacts');
        setIsSlidingOut(false);
      }
    };

    handleResize();
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const handleOpenChat = () => {
    if (isMobile) {
      setIsSlidingOut(false);
      setView('chat');
    }
  };

  const handleBackToContacts = () => {
    if (!isMobile) return;
    setIsSlidingOut(true); // запускаем анимацию slide-out
  };

  const handleChatAnimationEnd = () => {
    if (isSlidingOut) {
      setView('contacts');
      setIsSlidingOut(false);
    }
  };

  if (isMobile) {
    return (
      <div className="app-mobile-layout">
        {view === 'contacts' && (
          <LeftSideBar onOpenChat={handleOpenChat} />
        )}

        {view === 'chat' && (
          <div
            className={`chat-slide-in ${isSlidingOut ? 'chat-slide-out' : ''}`}
            onAnimationEnd={handleChatAnimationEnd}
          >
            <CenterSideBar onBack={handleBackToContacts} />
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="app-desktop-layout">
      <LeftSideBar />
      <CenterSideBar />
    </div>
  );
}

export default App;
