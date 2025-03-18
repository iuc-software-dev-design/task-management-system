# Task Management System

## 📋 Introduction
The Task Management System is designed to help teams efficiently manage their workflow by providing essential features such as user authentication, task assignment, role-based access control, and email notifications. Our goal is to streamline task tracking, improve productivity, and enhance team organization.

## ⚙️ Features
- **User Authentication**: Secure login and registration system (JWT-based authentication).
- **Email Verification**: Users must verify their email before accessing the system.
- **Task Management**:
  - Create, assign, and update task statuses.
  - Users can view and manage their assigned tasks.
- **Role-Based Access Control**:
  - Admins can assign tasks and manage user roles.
- **Email Notifications**:
  - Automated email notifications for task assignments and updates.

## 🧑‍💻 Technology Stack
### Backend
- **Framework**: ASP.NET Core & C#
- **Authentication**: JSON Web Token (JWT)
- **Database**: MySQL
- **API**: RESTful API structure

### Frontend
- **Framework**: React.js
- **UI Styling**: Tailwind CSS
- **State Management**: React Context or Redux (optional)

### DevOps & Collaboration
- **Version Control**: Git & GitHub
- **Containerization**: Docker
- **CI/CD**: GitHub Actions (for deployment automation)

## 📂 Project Structure
```
 Task-Management-System
 ├── backend/   # ASP.NET Core backend
 ├── frontend/  # React frontend
 ├── docs/      # Project documentation
 ├── docker/    # Docker configuration files 

```
## 🛠️ Installation & Setup
### Prerequisites
- **Backend** requires .NET SDK and MySQL installation.
- **Frontend** requires Node.js and npm/yarn.

### Backend Setup
```sh
cd backend
# Install dependencies
dotnet restore
# Apply database migrations
dotnet ef database update
# Run the application
dotnet run
```

### Frontend Setup
```sh
cd frontend
# Install dependencies
npm install
# Start the application
npm start
```


