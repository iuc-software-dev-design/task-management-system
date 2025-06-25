import React, { useState, useEffect } from "react";
import "./ProfilePage.css";

function ProfilePage() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isEditing, setIsEditing] = useState(false);
  const [editData, setEditData] = useState({
    name: "",
    email: "",
  });
  const [sendingVerification, setSendingVerification] = useState(false);

  useEffect(() => {
    fetchUserProfile();
  }, []);

  const fetchUserProfile = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/User/me`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to fetch profile");
      }

      const userData = await response.json();
      setUser(userData);
      setEditData({
        name: userData.name || "",
        email: userData.email || "",
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateProfile = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/User/me`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(editData),
        }
      );

      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || "Failed to update profile");
      }

      const updatedUser = await response.json();
      setUser(updatedUser);
      setIsEditing(false);
      setSuccess("Profile updated successfully!");
      setTimeout(() => setSuccess(""), 3000);
    } catch (err) {
      setError(err.message);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  };

  const handleSendVerificationEmail = async () => {
    try {
      setSendingVerification(true);
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/Account/send-verification/${user.id}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (response.ok) {
        setSuccess("Verification email sent! Check your inbox.");
        setTimeout(() => setSuccess(""), 5000);
      } else {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to send verification email");
      }
    } catch (err) {
      setError(`Error sending verification email: ${err.message}`);
      setTimeout(() => setError(""), 5000);
    } finally {
      setSendingVerification(false);
    }
  };

  const getRoleText = (role) => {
    switch (role) {
      case "USER":
        return "User";
      case "TEAM_LEAD":
        return "Team Lead";
      case "MANAGER":
        return "Manager";
      default:
        return "Unknown";
    }
  };

  const getRoleColor = (role) => {
    switch (role) {
      case 0:
        return "#6c757d"; // User - Gray
      case 1:
        return "#007bff"; // Team Lead - Blue
      case 2:
        return "#28a745"; // Manager - Green
      default:
        return "#6c757d";
    }
  };

  if (loading) return <div className="loading">Loading profile...</div>;

  return (
    <div className="profile-container">
      <div className="profile-header">
        <h1>My Profile</h1>
        <div className="profile-nav">
          <button
            onClick={() => (window.location.href = "/home")}
            className="nav-btn"
          >
            ← Back to Dashboard
          </button>
          <button onClick={handleLogout} className="logout-btn">
            Logout
          </button>
        </div>
      </div>

      {error && <div className="error">{error}</div>}
      {success && <div className="success">{success}</div>}

      <div className="profile-card">
        {!isEditing ? (
          <div className="profile-view">
            <div className="profile-info">
              <div className="info-item">
                <label>Name:</label>
                <span>{user?.name || "Not set"}</span>
              </div>
              <div className="info-item">
                <label>Email:</label>
                <span>{user?.email}</span>
              </div>
              <div className="info-item">
                <label>Role:</label>
                <span
                  className="role-badge"
                  style={{ backgroundColor: getRoleColor(user?.userRole) }}
                >
                  {getRoleText(user?.userRole)}
                </span>
              </div>
              <div className="info-item">
                <label>Email Verified:</label>
                <div className="email-verification">
                  <span
                    className={
                      user?.emailVerified ? "verified" : "not-verified"
                    }
                  >
                    {user?.emailVerified ? "✓ Verified" : "✗ Not Verified"}
                  </span>
                  {!user?.emailVerified && (
                    <button
                      onClick={handleSendVerificationEmail}
                      className="verify-email-btn"
                      disabled={sendingVerification}
                    >
                      {sendingVerification
                        ? "Sending..."
                        : "📧 Send Verification Email"}
                    </button>
                  )}
                </div>
              </div>
            </div>
            <button onClick={() => setIsEditing(true)} className="edit-btn">
              Edit Profile
            </button>
          </div>
        ) : (
          <div className="profile-edit">
            <form onSubmit={handleUpdateProfile}>
              <div className="form-group">
                <label>Name:</label>
                <input
                  type="text"
                  value={editData.name}
                  onChange={(e) =>
                    setEditData({ ...editData, name: e.target.value })
                  }
                  required
                />
              </div>
              <div className="form-group">
                <label>Email:</label>
                <input
                  type="email"
                  value={editData.email}
                  onChange={(e) =>
                    setEditData({ ...editData, email: e.target.value })
                  }
                  required
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="save-btn">
                  Save Changes
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setIsEditing(false);
                    setEditData({
                      name: user?.name || "",
                      email: user?.email || "",
                    });
                  }}
                  className="cancel-btn"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
}

export default ProfilePage;
