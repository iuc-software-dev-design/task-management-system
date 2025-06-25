import React, { useEffect, useState } from "react";
import "./HomePage.css";

function HomePage() {
  const [tasks, setTasks] = useState([]);
  const [users, setUsers] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [currentUser, setCurrentUser] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newTask, setNewTask] = useState({
    title: "",
    description: "",
    startDate: "",
    endDate: "",
    assigneeIds: [],
  });

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        setLoading(true);
        const token = localStorage.getItem("token");

        // Fetch current user
        const userResponse = await fetch(
          `${process.env.REACT_APP_API_URL}/api/User/me`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );
        if (userResponse.ok) {
          const userData = await userResponse.json();
          setCurrentUser(userData);

          // Fetch notifications for current user
          const notifResponse = await fetch(
            `${process.env.REACT_APP_API_URL}/api/Notification/user/${userData.id}`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          if (notifResponse.ok) {
            const notifData = await notifResponse.json();
            setNotifications(notifData);
          }
        }
      } catch (err) {
        setError(err.message);
        console.error("Error fetching user data:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUserData();
  }, []);

  useEffect(() => {
    const fetchTasksData = async () => {
      if (!currentUser) return;

      try {
        const token = localStorage.getItem("token");

        // Fetch tasks based on user role
        if (
          currentUser.userRole === "MANAGER" ||
          currentUser.userRole === "TEAM_LEAD"
        ) {
          // Managers and Team Leads can see all tasks
          const tasksResponse = await fetch(
            `${process.env.REACT_APP_API_URL}/api/Task`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          if (tasksResponse.ok) {
            const tasksData = await tasksResponse.json();
            setTasks(tasksData);
          }

          // Fetch all users for task assignment
          const usersResponse = await fetch(
            `${process.env.REACT_APP_API_URL}/api/User`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          if (usersResponse.ok) {
            const usersData = await usersResponse.json();
            setUsers(usersData);
          }
        } else {
          // Regular users can only see their assigned tasks
          const myTasksResponse = await fetch(
            `${process.env.REACT_APP_API_URL}/api/Task/my`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          if (myTasksResponse.ok) {
            const tasksData = await myTasksResponse.json();
            setTasks(tasksData);
          }
        }
      } catch (err) {
        setError(err.message);
        console.error("Error fetching tasks data:", err);
      }
    };

    fetchTasksData();
  }, [currentUser]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  };

  const handleCreateTask = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/Task`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            title: newTask.title,
            description: newTask.description,
            startDate: newTask.startDate,
            endDate: newTask.endDate,
            assigneeIds: newTask.assigneeIds,
          }),
        }
      );

      if (response.ok) {
        const createdTask = await response.json();
        setTasks([...tasks, createdTask]);
        setNewTask({
          title: "",
          description: "",
          startDate: "",
          endDate: "",
          assigneeIds: [],
        });
        setShowCreateForm(false);
      } else {
        setError("Failed to create task");
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleUpdateTaskStatus = async (taskId, newStatus) => {
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/Task/${taskId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ taskStatus: parseInt(newStatus) }),
        }
      );

      if (response.ok) {
        // State'i hemen güncelle
        setTasks(
          tasks.map((task) =>
            task.id === taskId
              ? { ...task, taskStatus: parseInt(newStatus) }
              : task
          )
        );

        // Success mesajı göster
        console.log("Task status updated successfully");
      } else {
        throw new Error("Failed to update task status");
      }
    } catch (err) {
      setError(`Error updating task status: ${err.message}`);
      console.error("Error updating task status:", err);
    }
  };

  const handleDeleteTask = async (taskId) => {
    if (!window.confirm("Are you sure you want to delete this task?")) return;

    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/Task/${taskId}`,
        {
          method: "DELETE",
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (response.ok) {
        setTasks(tasks.filter((task) => task.id !== taskId));
      }
    } catch (err) {
      setError(err.message);
    }
  };

  // User Management Functions
  const handleDeleteUser = async (userId, userName) => {
    if (
      !window.confirm(
        `Are you sure you want to delete user "${userName}"? This action cannot be undone.`
      )
    )
      return;

    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/User/${userId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (response.ok) {
        // Users listesinden kullanıcıyı kaldır
        setUsers(users.filter((user) => user.id !== userId));

        // Eğer silinen kullanıcı task'larda assigned ise, o task'lardaki assignee'yi kaldır
        setTasks(
          tasks.map((task) => ({
            ...task,
            assignees: task.assignees.filter(
              (assignee) => assignee.id !== userId
            ),
          }))
        );

        console.log(`User ${userName} deleted successfully`);
      } else {
        const errorText = await response.text();
        console.error("Delete user error response:", errorText);

        if (
          response.status === 500 &&
          errorText.includes("foreign key constraint")
        ) {
          throw new Error(
            `Cannot delete user "${userName}" because they have created tasks. Please delete their tasks first.`
          );
        } else {
          throw new Error(errorText || "Failed to delete user");
        }
      }
    } catch (err) {
      setError(`Error deleting user: ${err.message}`);
    }
  };

  const handleChangeUserRole = async (userId, newRole) => {
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/User/${userId}/role`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ role: parseInt(newRole) }),
        }
      );

      if (response.ok) {
        // Users listesindeki kullanıcının role'ünü güncelle
        const roleNames = ["USER", "TEAM_LEAD", "MANAGER"];
        setUsers(
          users.map((user) =>
            user.id === userId
              ? { ...user, userRole: roleNames[parseInt(newRole)] }
              : user
          )
        );
      } else {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to update user role");
      }
    } catch (err) {
      setError(`Error updating user role: ${err.message}`);
    }
  };

  // Task Update Functions
  const [editingTask, setEditingTask] = useState(null);
  const [taskUpdateData, setTaskUpdateData] = useState({
    title: "",
    description: "",
    startDate: "",
    endDate: "",
    assigneeIds: [],
  });

  const handleEditTask = (task) => {
    setEditingTask(task.id);
    setTaskUpdateData({
      title: task.title,
      description: task.description,
      startDate: task.startDate.slice(0, 16), // datetime-local format
      endDate: task.endDate.slice(0, 16),
      assigneeIds: task.assignees.map((a) => a.id),
    });
  };

  const handleUpdateTask = async (taskId) => {
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/Task/${taskId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            title: taskUpdateData.title,
            description: taskUpdateData.description,
            startDate: taskUpdateData.startDate,
            endDate: taskUpdateData.endDate,
          }),
        }
      );

      if (response.ok) {
        // Assignee güncelleme ayrı endpoint
        if (taskUpdateData.assigneeIds.length > 0) {
          await fetch(
            `${process.env.REACT_APP_API_URL}/api/Task/${taskId}/assign`,
            {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
              },
              body: JSON.stringify({
                assigneeIds: taskUpdateData.assigneeIds,
              }),
            }
          );
        }

        // Task'ı yeniden fetch et
        const updatedTaskResponse = await fetch(
          `${process.env.REACT_APP_API_URL}/api/Task/${taskId}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (updatedTaskResponse.ok) {
          const updatedTask = await updatedTaskResponse.json();
          setTasks(
            tasks.map((task) => (task.id === taskId ? updatedTask : task))
          );
        }

        setEditingTask(null);
      } else {
        throw new Error("Failed to update task");
      }
    } catch (err) {
      setError(`Error updating task: ${err.message}`);
    }
  };

  const markNotificationAsRead = async (notificationId) => {
    try {
      const token = localStorage.getItem("token");
      await fetch(
        `${process.env.REACT_APP_API_URL}/api/Notification/${notificationId}/read`,
        {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setNotifications(notifications.filter((n) => n.id !== notificationId));
    } catch (err) {
      console.error("Error marking notification as read:", err);
    }
  };

  const getStatusText = (status) => {
    switch (parseInt(status)) {
      case 0:
        return "Not Started";
      case 1:
        return "In Progress";
      case 2:
        return "Completed";
      default:
        return "Unknown";
    }
  };

  const getStatusColor = (status) => {
    switch (parseInt(status)) {
      case 0:
        return "#6c757d"; // Not Started - Gray
      case 1:
        return "#007bff"; // In Progress - Blue
      case 2:
        return "#28a745"; // Completed - Green
      default:
        return "#6c757d";
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div className="homepage-container">
      <header className="homepage-header">
        <h1>Task Management Dashboard</h1>
        <div className="header-info">
          {currentUser && (
            <span>
              Welcome, {currentUser.name} ({currentUser.userRole})
            </span>
          )}
          <div className="header-buttons">
            <button
              onClick={() => (window.location.href = "/profile")}
              className="profile-btn"
            >
              My Profile
            </button>
            <button onClick={handleLogout} className="logout-btn">
              Logout
            </button>
          </div>
        </div>
      </header>

      {error && <div className="error-message">{error}</div>}

      {/* Notifications */}
      {notifications.length > 0 && (
        <div className="notifications-section">
          <h3>Notifications ({notifications.length})</h3>
          {notifications.slice(0, 3).map((notification) => (
            <div key={notification.id} className="notification-item">
              <span>{notification.message}</span>
              <button onClick={() => markNotificationAsRead(notification.id)}>
                ✓
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Create Task Section - Only for MANAGER and TEAM_LEAD */}
      {currentUser &&
        (currentUser.userRole === "MANAGER" ||
          currentUser.userRole === "TEAM_LEAD") && (
          <div className="create-task-section">
            <button
              onClick={() => setShowCreateForm(!showCreateForm)}
              className="create-btn"
            >
              {showCreateForm ? "Cancel" : "Create New Task"}
            </button>

            {showCreateForm && (
              <form onSubmit={handleCreateTask} className="create-form">
                <input
                  type="text"
                  placeholder="Task Title"
                  value={newTask.title}
                  onChange={(e) =>
                    setNewTask({ ...newTask, title: e.target.value })
                  }
                  required
                />
                <textarea
                  placeholder="Description"
                  value={newTask.description}
                  onChange={(e) =>
                    setNewTask({ ...newTask, description: e.target.value })
                  }
                  required
                />
                <input
                  type="datetime-local"
                  value={newTask.startDate}
                  onChange={(e) =>
                    setNewTask({ ...newTask, startDate: e.target.value })
                  }
                  required
                />
                <input
                  type="datetime-local"
                  value={newTask.endDate}
                  onChange={(e) =>
                    setNewTask({ ...newTask, endDate: e.target.value })
                  }
                  required
                />
                {users.length > 0 && (
                  <select
                    multiple
                    value={newTask.assigneeIds}
                    onChange={(e) =>
                      setNewTask({
                        ...newTask,
                        assigneeIds: Array.from(
                          e.target.selectedOptions,
                          (option) => option.value
                        ),
                      })
                    }
                  >
                    {users.map((user) => (
                      <option key={user.id} value={user.id}>
                        {user.name}
                      </option>
                    ))}
                  </select>
                )}
                <button type="submit">Create Task</button>
              </form>
            )}
          </div>
        )}

      {/* Tasks Section */}
      <div className="tasks-section">
        <h2>
          {currentUser &&
          (currentUser.userRole === "MANAGER" ||
            currentUser.userRole === "TEAM_LEAD")
            ? `All Tasks (${tasks.length})`
            : `My Tasks (${tasks.length})`}
        </h2>

        {tasks.length === 0 ? (
          <p>
            {currentUser &&
            (currentUser.userRole === "MANAGER" ||
              currentUser.userRole === "TEAM_LEAD")
              ? "No tasks created yet."
              : "No tasks assigned to you yet."}
          </p>
        ) : (
          <div className="tasks-grid">
            {tasks.map((task) => (
              <div key={task.id} className="task-card">
                <div className="task-header">
                  <h3>{task.title}</h3>
                  <div className="task-actions">
                    <select
                      value={task.taskStatus}
                      onChange={(e) =>
                        handleUpdateTaskStatus(
                          task.id,
                          parseInt(e.target.value)
                        )
                      }
                      style={{
                        backgroundColor: getStatusColor(task.taskStatus),
                      }}
                    >
                      <option value={0}>Not Started</option>
                      <option value={1}>In Progress</option>
                      <option value={2}>Completed</option>
                    </select>
                    {/* Edit and Delete buttons only for MANAGER and TEAM_LEAD */}
                    {currentUser &&
                      (currentUser.userRole === "MANAGER" ||
                        currentUser.userRole === "TEAM_LEAD") && (
                        <>
                          <button
                            onClick={() => handleEditTask(task)}
                            className="edit-btn"
                          >
                            ✏️
                          </button>
                          <button
                            onClick={() => handleDeleteTask(task.id)}
                            className="delete-btn"
                          >
                            🗑️
                          </button>
                        </>
                      )}
                  </div>
                </div>

                {/* Task Edit Form */}
                {editingTask === task.id ? (
                  <div className="task-edit-form">
                    <input
                      type="text"
                      value={taskUpdateData.title}
                      onChange={(e) =>
                        setTaskUpdateData({
                          ...taskUpdateData,
                          title: e.target.value,
                        })
                      }
                      placeholder="Task Title"
                    />
                    <textarea
                      value={taskUpdateData.description}
                      onChange={(e) =>
                        setTaskUpdateData({
                          ...taskUpdateData,
                          description: e.target.value,
                        })
                      }
                      placeholder="Description"
                    />
                    <input
                      type="datetime-local"
                      value={taskUpdateData.startDate}
                      onChange={(e) =>
                        setTaskUpdateData({
                          ...taskUpdateData,
                          startDate: e.target.value,
                        })
                      }
                    />
                    <input
                      type="datetime-local"
                      value={taskUpdateData.endDate}
                      onChange={(e) =>
                        setTaskUpdateData({
                          ...taskUpdateData,
                          endDate: e.target.value,
                        })
                      }
                    />
                    {users.length > 0 && (
                      <select
                        multiple
                        value={taskUpdateData.assigneeIds}
                        onChange={(e) =>
                          setTaskUpdateData({
                            ...taskUpdateData,
                            assigneeIds: Array.from(
                              e.target.selectedOptions,
                              (option) => option.value
                            ),
                          })
                        }
                      >
                        {users.map((user) => (
                          <option key={user.id} value={user.id}>
                            {user.name}
                          </option>
                        ))}
                      </select>
                    )}
                    <div className="task-edit-buttons">
                      <button onClick={() => handleUpdateTask(task.id)}>
                        Save Changes
                      </button>
                      <button onClick={() => setEditingTask(null)}>
                        Cancel
                      </button>
                    </div>
                  </div>
                ) : (
                  <>
                    <p className="task-description">{task.description}</p>

                    <div className="task-info">
                      <small>Created by: {task.createdBy}</small>
                      <small>
                        Start: {new Date(task.startDate).toLocaleDateString()}
                      </small>
                      <small>
                        End: {new Date(task.endDate).toLocaleDateString()}
                      </small>
                    </div>

                    {task.assignees && task.assignees.length > 0 && (
                      <div className="task-assignees">
                        <strong>Assignees:</strong>
                        <ul>
                          {task.assignees.map((assignee) => (
                            <li key={assignee.id}>{assignee.name}</li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Users Section - Only for MANAGER and TEAM_LEAD */}
      {currentUser &&
        (currentUser.userRole === "MANAGER" ||
          currentUser.userRole === "TEAM_LEAD") &&
        users.length > 0 && (
          <div className="users-section">
            <h2>Team Members ({users.length})</h2>
            <div className="users-list">
              {users.map((user) => (
                <div key={user.id} className="user-item">
                  <div className="user-info">
                    <span className="user-name">
                      {user.name} ({user.email})
                    </span>
                    <span
                      className={`user-role role-${user.userRole.toLowerCase()}`}
                    >
                      {user.userRole}
                    </span>
                  </div>
                  <div className="user-actions">
                    {/* Role değiştirme - sadece MANAGER için */}
                    {currentUser.userRole === "MANAGER" &&
                      user.id !== currentUser.id && (
                        <select
                          value={
                            user.userRole === "USER"
                              ? 0
                              : user.userRole === "TEAM_LEAD"
                              ? 1
                              : 2
                          }
                          onChange={(e) =>
                            handleChangeUserRole(user.id, e.target.value)
                          }
                          className="role-select"
                        >
                          <option value={0}>User</option>
                          <option value={1}>Team Lead</option>
                          <option value={2}>Manager</option>
                        </select>
                      )}

                    {/* User silme - MANAGER ve TEAM_LEAD için (kendi hesabını silemez) */}
                    {user.id !== currentUser.id && (
                      <button
                        onClick={() => handleDeleteUser(user.id, user.name)}
                        className="delete-user-btn"
                        title="Delete User"
                      >
                        🗑️
                      </button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
    </div>
  );
}

export default HomePage;
