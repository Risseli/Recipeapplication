import React from "react";
import { Link } from "react-router-dom";
import "./Navbar.css";
import { useState } from "react";
import { useAuth } from "./Authentication";

export const Navbar = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const {user, logout}= useAuth();
  return (
    <nav>
      <Link to="/" className="title">
      <img src="blackAndWhiteLogo.png" alt="Home"/>
      </Link>
      <div
        className="menu"
        onClick={() => {
          setMenuOpen(!menuOpen);
        }}
      >
        <span></span>
        <span></span>
        <span></span>
        <span></span>
      </div>
      <ul className={menuOpen ? "open" : ""}>
        
        <li>
          <Link to="/about">About</Link>
        </li>
        <li>
          <Link to="/recipes">Recipes</Link>
          </li>
        {user ? (
          <li>
            <Link to="/" onClick={logout}>
              Logout
            </Link>
          </li>
        ) : (
          <li>
            <Link to="/login">Login</Link>
          </li>
        )}
        <li>
          <Link to="/profile">Profile</Link>
        </li>
      </ul>
    </nav>
  );
};
