export default function Message({info,index}){

    const isoString = info.messageDateTime;
    const date = new Date(isoString);

    const hours = date.getHours();
    const minutes = date.getMinutes();
    return (
        <div key={index} className="message">
            <p className="message__text">{info.messageText}</p>
            <span className="message__time">{`${hours}:${minutes.toString().padStart(2, "0")}`}</span>
        </div>
    )
}