import React from 'react'
import { useState } from "react"
import AuthLogin from "./AuthLogin"
import AuthRegister from "./AuthRegister"
import "./AuthStyles.css"

export default function AuthModalContainer(){
    const [switcher, setSwitcher] = useState(true)

    return(
            <div className="Auth__body">
                <div className="Auth__container">
                    <div className="Auth__buttons-container">
                        <button className={`Auth__link-button ${switcher ? "link__active":""}`} onClick={() => setSwitcher(true)}>Вход</button>
                        <button className={`Auth__link-button ${!switcher ? "link__active":""}`} onClick={() => setSwitcher(false)}>Регистрация</button>
                    </div>

                    {switcher 
                        ? <AuthLogin/> 
                        : <AuthRegister toSwitch={() => setSwitcher(true)} />
                    }
                </div>
            </div>
    )
}
