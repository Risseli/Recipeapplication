import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../Authentication";


const Login = () => {
 


  const [loginData, setLoginData] = useState({
    username: "",
    password: "",
  });
  const {login} = useAuth();
  const navigate = useNavigate();
 

  const [registerData, setRegisterData] = useState({
    email: "",
    username: "",
    password: "",
    name: "",
  });

  const [isLoggedIn, setLoggedIn] = useState(false);
  const [loginError, setLoginError] = useState(null);

  const loginUser = async () => {
    try {
      const response = await fetch("https://recipeappapi.azurewebsites.net/api/User/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(loginData),
      });
  
      if (!response.ok) {
        console.error("Väärät tiedot.");
        setLoginError("Wrong Username or Password");
        return;
      }
  
      const result = await response.json();
  
      console.log("Login successful:", result);
  
      login({
        userId: result.userId,
        token: result.token,
      });
  
      setLoggedIn(true); // Optionally set a state to track login status
  
      navigate("/");
    } catch (error) {
      console.error("Error during login:", error);
      setLoginError("An error occurred during login.");
    }
  };
  const registerUser = async () => {

      const response = await fetch("https://recipeappapi.azurewebsites.net/api/user/Register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(registerData),
      });

      const result = await response.json();

      console.log("Registration successful:", result);

  };

  const logoutUser = () => {
    setLoggedIn(false);

  };

  return (
    <div>
      {isLoggedIn ? (
        <>
          <p>Welcome! You are logged in.</p>
          <button type="button" onClick={logoutUser}>
            Logout
          </button>
        </>
      ) : (
        <>
          <h1>Login</h1>
          {loginError && <p style={{color:"red"}}>{loginError}</p>}
          <form>
            <input
              type="text"
              placeholder="Username"
              value={loginData.username}
              onChange={(e) => setLoginData({ ...loginData, username: e.target.value })}
            />
            <input
              type="password"
              placeholder="Password"
              value={loginData.password}
              onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
            />
            <br />
            <button type="button" onClick={loginUser}>
              Login
            </button>
          </form>

          <h1>Register</h1>
          <form>
            <input
              type="email"
              placeholder="Email"
              value={registerData.email}
              onChange={(e) => {
                setRegisterData({ ...registerData, email: e.target.value });
              }}
            />
            <input
              type="text"
              placeholder="Username"
              value={registerData.username}
              onChange={(e) => setRegisterData({ ...registerData, username: e.target.value })}
            />
            <input
              type="password"
              placeholder="Password"
              value={registerData.password}
              onChange={(e) => setRegisterData({ ...registerData, password: e.target.value })}
            />
            <input
              type="text"
              placeholder="Name"
              value={registerData.name}
              onChange={(e) => setRegisterData({ ...registerData, name: e.target.value })}
            />
            <br />
            <button type="button" onClick={registerUser}>
              Register
            </button>
          </form>
        </>
      )}
    </div>
  );
};

export { Login };
