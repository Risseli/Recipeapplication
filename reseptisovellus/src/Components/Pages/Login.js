import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../Authentication";
import './Login.css';

const Login = () => {
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
  const { login } = useAuth();
  const navigate = useNavigate();

  const [isLoggedIn, setLoggedIn] = useState(false);
  const [loginError, setLoginError] = useState(null);
  const [loginAttempts, setLoginAttempts] = useState(0);
  const [recoverPasswordPrompt, setRecoverPasswordPrompt] = useState(false);
  const [recoveryEmail, setRecoveryEmail] = useState('');
  const [registerError, setRegisterError] = useState(null);

  const loginUser = async () => {
    try {
      const response = await fetch("https://localhost:7005/api/User/login", { // https://recipeappapi.azurewebsites.net/api/User/login
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

      setLoggedIn(true);
      navigate("/");
    } catch (error) {
      console.error("Error during login:", error);
      setLoginError("An error occurred during login.");
    }
  };

  const registerUser = async () => {

    const response = await fetch("https://localhost:7005/api/User/register", { // https://recipeappapi.azurewebsites.net/api/User/register
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(registerData),
    });

    if (!response.ok) {
      console.error("Registration failed.");
      return;
    }

    const result = await response.json();

    console.log("Registration successful:", result);
    // Check if the result contains an error message for duplicate email or username
    if (result.error) {
      if (result.error.includes("email")) {
        setRegisterError("Email already exists. Please use a different email.");
      }
      if (result.error.includes("username")) {
        setRegisterError("Username already exists. Please choose a different username.");
      }
      return;
    }

  };


  const recoverPassword = async () => {
    try {
      const response = await fetch(`https://localhost:7005/api/User/RecoverPassword/${recoveryEmail}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        }
      });

      if (!response.ok) {
        alert("A recovery message has been sent if an account exists with that email.");

        console.error("Failed to initiate password recovery.");
        setRecoverPasswordPrompt(false);
        return;
      }

      console.log("Password recovery initiated successfully.");
      alert("A recovery message has been sent if an account exists with that email.");

      // Close the password recovery prompt and reset email input
      setRecoverPasswordPrompt(false);
      setRecoveryEmail('');
    } catch (error) {
      console.error("Error initiating password recovery:", error);
    }
  };

  const handleLogin = () => {
    if (recoverPasswordPrompt) {
      recoverPassword();
    } else {
      loginUser();
    }
  };

  const logoutUser = () => {
    setLoggedIn(false);
    setLoginAttempts(0);
    setLoginData({ username: "", password: "" });
    setLoginError(null);
  };

  return (
    <div className="container">
      {isLoggedIn ? (
        <>
          <p>Welcome! You are logged in.</p>
          <button className="logout-button" type="button" onClick={logoutUser}>
            Logout
          </button>
        </>
      ) : (
        <>
          <h1 className="login-heading">Login</h1>
          {loginError && <p className="error-message">{loginError}</p>}
          <form>
            <div className="form-group">
              <input
                className="input-field"
                type="text"
                placeholder="Username"
                value={loginData.username}
                onChange={(e) => setLoginData({ ...loginData, username: e.target.value })}
              />
            </div>
            <div className="form-group">
              <input
                className="input-field"
                type="password"
                placeholder="Password"
                value={loginData.password}
                onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
              />
            </div>
            <br />
            <p className="recover-password-text">
              Forgot your password? <a href="#" onClick={() => setRecoverPasswordPrompt(!recoverPasswordPrompt)}>Click here to recover it.</a>
            </p>
            <div className="form-group">
              <button className="login-button" type="button" onClick={handleLogin}>
                Login
              </button>
            </div>
          </form>
          {recoverPasswordPrompt && (
            <div>
              <h1 className="recover-password-heading">Password Recovery</h1>
              <form>
                <div className="form-group">
                  <input
                    className="input-field"
                    type="email"
                    placeholder="Your Email"
                    value={recoveryEmail}
                    onChange={(e) => setRecoveryEmail(e.target.value)}
                  />
                </div>
                <div className="form-group">
                  <button className="recover-password-button" type="button" onClick={recoverPassword}>
                    Recover Password
                  </button>
                </div>
              </form>
            </div>
          )}

          <h1 className="register-heading">Register</h1>
          {registerError && <p className="error-message">{registerError}</p>} {/* Display register error */}
          <form>
            <div className="form-group">
              <input
                className="input-field"
                type="email"
                placeholder="Email"
                value={registerData.email}
                onChange={(e) => setRegisterData({ ...registerData, email: e.target.value })}
              />
            </div>
            <div className="form-group">
              <input
                className="input-field"
                type="text"
                placeholder="Username"
                value={registerData.username}
                onChange={(e) => setRegisterData({ ...registerData, username: e.target.value })}
              />
            </div>
            <div className="form-group">
              <input
                className="input-field"
                type="password"
                placeholder="Password"
                value={registerData.password}
                onChange={(e) => setRegisterData({ ...registerData, password: e.target.value })}
              />
            </div>
            <div className="form-group">
              <input
                className="input-field"
                type="text"
                placeholder="Name"
                value={registerData.name}
                onChange={(e) => setRegisterData({ ...registerData, name: e.target.value })}
              />
            </div>
            <br />
            <div className="form-group">
              <button className="register-button" type="button" onClick={registerUser}>
                Register
              </button>
            </div>
          </form>
        </>
      )}
    </div>
  );
};
export { Login };
