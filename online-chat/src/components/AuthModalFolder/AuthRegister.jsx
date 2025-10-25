export default function AuthRegister({toSwitch}){
    const handleSubmit = (e) => {
        e.preventDefault();

        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const login = {
            userName: data.UserName,
            password: data.Password
        }

        fetch(`/api/Auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify( login )
        }).then(toSwitch())
    }

    return(
            <form className="Auth__form" onSubmit={handleSubmit}>
                <input className="Auth__input" id="UserName" name="UserName" type="text" placeholder="Имя" />
                <input className="Auth__input" id="Password" name="Password" type="password" placeholder="Пароль" />
                <button className="Auth__button" type="submit" >Зарегистрироваться</button>
            </form>
    )
}