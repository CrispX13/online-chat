import { useContext } from "react"
import { SignalRContext } from "../SignalRConf/SignalRContext"

export default function ChatHeading(){
    const {activeUser} = useContext(SignalRContext)
    return(
        <div className="ChatHeading__container">
            <div className="ChatHeading__names">
                <h3 className="ChatHeading__name">{activeUser.name}</h3>
                <span className="ChatHeading__status"></span>
            </div>
            <div className="ChatHeading__actions">
                
            </div>
        </div>
    )
}