import React, { useState } from "react";

// List of DTOs to display as buttons
const dtoList = [
    "UserDto",
    "LoginDto",
    "RegisterDto",
    "AuthResponseDto",
    "TaskDto",
    "CreateTaskDto",
    "UpdateTaskDto",
    "AssignTaskDto",
];

// Login Page Component Example
function LoginPage({ goToSignUp, onLoginSuccess }) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        try {
            const response = await fetch(
                process.env.REACT_APP_API_URL + "/api/login",
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email, password }),
                }
            );
            const data = await response.json();
            if (response.ok) {
                localStorage.setItem("token", data.token); // Save JWT
                onLoginSuccess();
            } else {
                setError(data.message || "Login failed");
            }
        } catch (err) {
            setError("Something went wrong");
        }
    };

    return (
        <form onSubmit={handleSubmit} style={formStyle}>
            <h2>Login</h2>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email"
                required
                style={inputStyle}
            />
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Password"
                required
                style={inputStyle}
            />
            <button type="submit" style={buttonStyle}>
                Login
            </button>
            <button
                type="button"
                style={{ ...buttonStyle, background: "#eee", color: "#333" }}
                onClick={goToSignUp}
            >
                Sign Up
            </button>
            {error && <div style={errorStyle}>{error}</div>}
        </form>
    );
}

// Sign Up Page Component Example
function SignUpPage({ goToLogin }) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess("");
        try {
            const response = await fetch(
                process.env.REACT_APP_API_URL + "/api/signup",
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email, password }),
                }
            );
            const data = await response.json();
            if (response.ok) {
                setSuccess("Registration successful! Please log in.");
            } else {
                setError(data.message || "Sign up failed");
            }
        } catch (err) {
            setError("Something went wrong");
        }
    };

    return (
        <form onSubmit={handleSubmit} style={formStyle}>
            <h2>Sign Up</h2>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email"
                required
                style={inputStyle}
            />
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Password"
                required
                style={inputStyle}
            />
            <button type="submit" style={buttonStyle}>
                Sign Up
            </button>
            <button
                type="button"
                style={{ ...buttonStyle, background: "#eee", color: "#333" }}
                onClick={goToLogin}
            >
                Login
            </button>
            {error && <div style={errorStyle}>{error}</div>}
            {success && <div style={successStyle}>{success}</div>}
        </form>
    );
}

// Main HomePage Component
export default function HomePage() {
    const [page, setPage] = useState("login");

    const handleDtoClick = (dto) => {
        alert(`Clicked on ${dto}. (You can implement navigation or actions here.)`);
    };

    if (page === "login")
        return (
            <LoginPage
                goToSignUp={() => setPage("signup")}
                onLoginSuccess={() => setPage("home")}
            />
        );
    if (page === "signup")
        return <SignUpPage goToLogin={() => setPage("login")} />;

    // "home" page after login
    return (
        <div style={{ padding: "2rem" }}>
            <h1>Homepage</h1>
            <h2>DTOs</h2>
            <div
                style={{
                    display: "flex",
                    flexDirection: "column",
                    gap: "1rem",
                    maxWidth: "300px",
                }}
            >
                {dtoList.map((dto) => (
                    <button
                        key={dto}
                        onClick={() => handleDtoClick(dto)}
                        style={buttonStyle}
                    >
                        {dto}
                    </button>
                ))}
            </div>
        </div>
    );
}

// Simple styles
const buttonStyle = {
    padding: "1rem",
    fontSize: "1rem",
    cursor: "pointer",
    marginBottom: "0.5rem",
};

const formStyle = {
    display: "flex",
    flexDirection: "column",
    maxWidth: "300px",
    gap: "1rem",
    margin: "2rem auto",
};

const inputStyle = {
    padding: "0.75rem",
    fontSize: "1rem",
};

const errorStyle = {
    color: "red",
    marginTop: "0.5rem",
};

const successStyle = {
    color: "green",
    marginTop: "0.5rem",
};