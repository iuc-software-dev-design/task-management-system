import React, { useState } from "react";
import "./SignUpPage.css";

function SignUpPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [validationError, setValidationError] = useState("");

    const validateForm = () => {
        if (!email.includes("@")) {
            return "Invalid email format.";
        }
        if (password.length < 6) {
            return "Password must be at least 6 characters long.";
        }
        return "";
    };

    const handleSignUp = async (e) => {
        e.preventDefault();
        const validationMessage = validateForm();
        if (validationMessage) {
            setValidationError(validationMessage);
            return;
        }
        setValidationError("");

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/signup`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
            });

            if (!response.ok) {
                throw new Error("Signup failed");
            }

            window.location.href = "/login"; // Redirect to login page
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div className="container">
            <h1>Sign Up</h1>
            <form onSubmit={handleSignUp}>
                <div>
                    <label>Email:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                {validationError && <p style={{ color: "red" }}>{validationError}</p>}
                {error && <p style={{ color: "red" }}>{error}</p>}
                <button type="submit">Sign Up</button>
            </form>
        </div>
    );
}

export default SignUpPage;