import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import logo from "../assets/nexer-logo.png";
import "./Navbar.css";

const Navbar = ({ setIsAuthenticated }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const logout = () => {
  localStorage.removeItem("token");

  if (setIsAuthenticated) {
    setIsAuthenticated(false);
  }

  navigate("/login");
};

  const getPageTitle = () => {
    switch (location.pathname) {
      case "/dashboard":
        return "Dashboard";
      case "/reports":
        return "Reports";
      default:
        return "Office Presence Tracking";
    }
  };

  const currentDateTime = new Date().toLocaleString();

  return (
    <header className="navbar">
      <div className="navbar-left">
        <h2>{getPageTitle()}</h2>
        <p>Monitor employee attendance and office presence in real time</p>
        <small className="nav-time">{currentDateTime}</small>
      </div>

      <div className="navbar-right">
        <img src={logo} alt="Nexer" className="nav-logo" />
        <button className="logout-btn" onClick={logout}>
          Logout
        </button>
      </div>
    </header>
  );
};

export default Navbar;