import React, { useEffect, useState } from "react";
import "./pages/HomePage.css"; 

function HomePage() {
    const [items, setItems] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchItems = async () => {
            try {
                const response = await fetch(`${process.env.REACT_APP_API_URL}/api/home`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                });

                if (!response.ok) {
                    throw new Error("Failed to fetch items");
                }

                const data = await response.json();
                setItems(data);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchItems();
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        window.location.href = "/login"; // Redirect to login page
    };

    return (
        <div>
            <h1>Welcome to the Homepage</h1>
            {error && <p style={{ color: "red" }}>{error}</p>}
            <ul>
                {items.map((item, index) => (
                    <li key={index}>{item.name}</li>
                ))}
            </ul>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
}

export default HomePage;