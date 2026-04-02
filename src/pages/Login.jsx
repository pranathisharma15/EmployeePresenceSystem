import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import API from "../services/api";
import "./Login.css";

const Login = ({ setIsAuthenticated }) => {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    username: "",
    password: "",
  });

  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    if (!form.username.trim() || !form.password.trim()) {
      alert("Please enter username and password");
      return;
    }

    try {
      setLoading(true);

      const response = await API.post("/auth/login", {
        username: form.username,
        password: form.password,
      });

      localStorage.setItem("token", response.data.token);

      if (setIsAuthenticated) {
        setIsAuthenticated(true);
      }

      navigate("/dashboard");
    } catch (error) {
      console.error("Login failed:", error);
      alert("Invalid username or password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-wrapper">
      <div className="login-left">
        <h2>Login</h2>

        <input
          type="text"
          placeholder="Username"
          value={form.username}
          onChange={(e) =>
            setForm({ ...form, username: e.target.value })
          }
        />

        <input
          type="password"
          placeholder="Password"
          value={form.password}
          onChange={(e) =>
            setForm({ ...form, password: e.target.value })
          }
          onKeyDown={(e) => e.key === "Enter" && handleLogin()}
        />

        <button onClick={handleLogin} disabled={loading}>
          {loading ? "Signing in..." : "Login"}
        </button>
      </div>

      <div className="login-right">
        <div className="branding">
          <h1>Welcome to Nexer</h1>
          <p>Employee Presence Tracking System</p>
        </div>
      </div>
    </div>
  );
};

export default Login;