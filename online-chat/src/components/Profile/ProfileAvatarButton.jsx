import { useContext } from "react";
import { AuthContext } from "../AuthContext";
import { useModal } from "../ModalService/ModalProvider";
import { useProfile } from "./ProfileProvider";
import ProfileSettings from "./ProfileSettings";

export default function ProfileAvatarButton() {
  const { userId } = useContext(AuthContext);
  const { account } = useProfile();
  const { openModal } = useModal();

  const handleOpenSettings = () => {
    openModal(<ProfileSettings />);
  };

  return (
    <button
      type="button"
      className="ProfileAvatarButton"
      onClick={handleOpenSettings}
    >
      <img
        className="ProfileAvatarButton__img"
        src={`/api/profile/${userId}/avatar?${Date.now()}`}
        alt={account?.name || "Профиль"}
      />
    </button>
  );
}
