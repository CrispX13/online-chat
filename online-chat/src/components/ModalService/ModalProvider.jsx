import { createContext, useContext, useState } from "react";

const ModalContext = createContext(null);

export function ModalProvider({ children }) {
  const [modalContent, setModalContent] = useState(null);
  const [showCloseBottom, setShowCloseBottom] = useState(true);

  const openModal = (content) => {
    setShowCloseBottom(true);
    setModalContent(content);
  };

  const closeModal = () => {
    setModalContent(null);
    setShowCloseBottom(true);
  };

  return (
    <ModalContext.Provider
      value={{ openModal, closeModal, setShowCloseBottom }}
    >
      {children}

      {modalContent && (
        <div className="modal-backdrop" onClick={closeModal}>
          <div
            className="modal-window"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-window__body">
              {modalContent}
            </div>

            <div className="modal-window__footer">
              {showCloseBottom && (
                <button onClick={closeModal}>Закрыть</button>
              )}
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