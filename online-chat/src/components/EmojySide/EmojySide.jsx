import EmojiPicker from 'emoji-picker-react';
import { useState } from 'react';

export default function EmojySide({ onSend }) {
  const [message, setMessage] = useState('');
  const [showPicker, setShowPicker] = useState(false);

  const handleEmojiClick = (emojiData) => {
    // emojiData.emoji — сам символ
    setMessage((prev) => prev + emojiData.emoji);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!message.trim()) return;
    onSend(message);
    setMessage('');
  };

  return (
    <div style={{ position: 'relative' }}>
      <form onSubmit={handleSubmit} style={{ display: 'flex', gap: 8 }}>
        <button
          type="button"
          onClick={() => setShowPicker((v) => !v)}
        >
          🙂
        </button>

        <input
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />

        <button type="submit">Отправить</button>
      </form>

      {showPicker && (
        <div style={{ position: 'absolute', bottom: '40px', left: 0, zIndex: 10 }}>
          <EmojiPicker onEmojiClick={handleEmojiClick} />
        </div>
      )}
    </div>
  );
}
