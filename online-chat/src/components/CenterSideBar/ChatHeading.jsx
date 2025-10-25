export default function ChatHeading({children}){
    return(
        <div className="ChatHeading__container">
            <div className="ChatHeading__names">
                <h3 className="ChatHeading__name">{children.name}</h3>
                <span className="ChatHeading__status"></span>
            </div>
            <div className="ChatHeading__actions">
                
            </div>
        </div>
    )
}