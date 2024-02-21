import React, { useState } from "react";
//import { useHistory } from "react-router-dom";

const Login = () => {
  //const history = useHistory();

  const [loginData, setLoginData] = useState({
    username: "",
    password: "",
  });

  const [registerData, setRegisterData] = useState({
    email: "",
    username: "",
    password: "",
    name: "",
  });

  const [emailError, setEmailError] = useState("");
  const [isLoggedIn, setLoggedIn] = useState(false);

  const checkEmailAvailability = async () => {
    try {
      const response = await fetch("hhttps://recipeappapi.azurewebsites.net/api/check-email", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: registerData.email,
        }),
      });

      const result = await response.json();

      setEmailError(result.available ? "" : "Email is already in use");
    } catch (error) {
      console.error("Error checking email availability:", error);
    }
  };

  const loginUser = async () => {
    try {
      const response = await fetch("https://recipeappapi.azurewebsites.net/api/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(loginData),
      });

      const result = await response.json();

      console.log("Login successful:", result);

      setLoggedIn(true);

      //history.push("/");
    } catch (error) {
      console.error("Error during login:", error);
    }
  };

  const registerUser = async () => {
    try {
      await checkEmailAvailability();

      if (emailError) {
        return;
      }

      const response = await fetch("https://recipeappapi.azurewebsites.net/api/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(registerData),
      });

      const result = await response.json();

      console.log("Registration successful:", result);
    } catch (error) {
      console.error("Error during registration:", error);
    }
  };

  const logoutUser = () => {
    setLoggedIn(false);

    //history.push("/");
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
                setEmailError("");
              }}
              onBlur={checkEmailAvailability}
            />
            {emailError && <p style={{ color: "red" }}>{emailError}</p>}
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
