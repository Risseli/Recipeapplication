import React, { useState } from "react";

export const Login = () => {
  const [loginData, setLoginData] = useState({
    username: "",
    password: "",
  });

  const [registerData, setRegisterData] = useState({
    email: "",
    registerUsername: "",
    registerPassword: "",
    nickname: "",
  });

  const loginUser = async () => {
    try {
      const response = await fetch("gaming", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          action: "login",
          username: loginData.username,
          password: loginData.password,
        }),
      });

      const data = await response.json();
      console.log(data);

      // Handle successful login - e.g., redirect to another page
    } catch (error) {
      console.error("Error during login:", error);
    }
  };

  const registerUser = async () => {
    try {
      const response = await fetch("gaming", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          action: "register",
          email: registerData.email,
          username: registerData.registerUsername,
          password: registerData.registerPassword,
          nickname: registerData.nickname,
        }),
      });

      const data = await response.json();
      console.log(data);

      // Handle successful registration - e.g., show a success message
    } catch (error) {
      console.error("Error during registration:", error);
    }
  };

  return (
    <div>
      <h1>Login</h1>
      <form>
        <label>Username:</label>
        <input
          type="text"
          value={loginData.username}
          onChange={(e) => setLoginData({ ...loginData, username: e.target.value })}
        />
        <br />
        <label>Password:</label>
        <input
          type="password"
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
        <label>Email:</label>
        <input
          type="text"
          value={registerData.email}
          onChange={(e) => setRegisterData({ ...registerData, email: e.target.value })}
        />
        <br />
        <label>Username:</label>
        <input
          type="text"
          value={registerData.registerUsername}
          onChange={(e) => setRegisterData({ ...registerData, registerUsername: e.target.value })}
        />
        <br />
        <label>Password:</label>
        <input
          type="password"
          value={registerData.registerPassword}
          onChange={(e) => setRegisterData({ ...registerData, registerPassword: e.target.value })}
        />
        <br />
        <label>Nickname:</label>
        <input
          type="text"
          value={registerData.nickname}
          onChange={(e) => setRegisterData({ ...registerData, nickname: e.target.value })}
        />
        <br />
        <button type="button" onClick={registerUser}>
          Register
        </button>
      </form>
    </div>
  );
};
