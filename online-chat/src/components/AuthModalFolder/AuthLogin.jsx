import React from 'react'
import { AuthContext } from "../AuthContext";
import { useContext } from "react";

export default function AuthLogin(){
    const {setJwtKey,setUserId} = useContext(AuthContext);
    const handleSubmit = (e) => {
        e.preventDefault();

        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const login = {
            userName: data.UserName,
            password: data.Password
        }

        fetch(`/api/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify( login )
        }).then(response => response.json())
            .then(json => {setJwtKey(json.result.token)
                setUserId(json.result.id)
            })
    }

    return(
            <form className="Auth__form" onSubmit={handleSubmit}>
                <input className="Auth__input"  id="UserName" name="UserName" type="text" placeholder="Имя" />
                <input className="Auth__input"  id="Password" name="Password" type="password" placeholder="Пароль" />
                <button className="Auth__button" type="submit" >Войти</button>
            </form>
    )
}