import { createContext, useContext, useState } from "react";

const ModalContext = createContext(null);

export function ModalProvider({ children }) {
  const [modalContent, setModalContent] = useState(null);

  const openModal = (content) => setModalContent(content);
  const closeModal = () => setModalContent(null);

  return (
    <ModalContext.Provider value={{ openModal, closeModal }}>
      {children}

      {modalContent && (
        <div className="modal-backdrop" onClick={closeModal}>
          <div
            className="modal-window"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-window__body">
              {/* здесь именно содержимое модалки */}
              {modalContent}
            </div>
            <div className="modal-window__footer">
              <button onClick={closeModal}>Закрыть</button>
            </div>
          </div>
        </div>
      )}
    </ModalContext.Provider>
  );
}

export function useModal() {
  return useContext(ModalContext);
}
