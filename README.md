# 404 FOUND - Task Management System

## 🎥 Video Link

- https://drive.google.com/file/d/1RGIA-68jwli602w_n3oDI2tbrdLpxxQy/view

## 📋 Introduction

The Task Management System is a comprehensive web application designed to help teams efficiently manage their workflow. It provides essential features such as user authentication, task assignment, role-based access control, and automated email notifications. Our goal is to streamline task tracking, improve productivity, and enhance team collaboration.

## ⚙️ Features

- **User Authentication**: Secure login and registration system with JWT-based authentication
- **Email Verification**: Users must verify their email before accessing the system
- **Role-Based Access Control**: Three user roles (USER, MANAGER, TEAM_LEAD) with different permissions
- **Task Management**:
  - Create, assign, and update tasks with detailed information
  - Track task status (NotStarted, InProgress, Completed)
  - View personal tasks and all tasks based on user role
  - Real-time task status updates
- **User Management**:
  - Managers can change user roles
  - View users by role
  - User profile management
- **Email Notifications**:
  - Automated email notifications for task assignments
  - Status update notifications for task creators and assignees
  - Professional HTML email templates
- **Responsive UI**: Modern, clean interface built with React and Tailwind CSS

## 📋 UML Diagram

![TMS UML Week2](https://github.com/user-attachments/assets/b34324bb-7b79-46cc-bc6c-3c0eb6bcda3c)

## 🧑‍💻 Technology Stack

### Backend

- **Framework**: ASP.NET Core 8.0 & C#
- **Authentication**: JSON Web Token (JWT) with ASP.NET Identity
- **Database**: MySQL with Entity Framework Core
- **API**: RESTful API with Swagger documentation
- **Email Service**: SMTP-based email notifications
- **Authorization**: Role-based access control

### Frontend

- **Framework**: React.js with modern hooks
- **UI Styling**: Tailwind CSS for responsive design
- **HTTP Client**: Fetch API for backend communication
- **State Management**: React useState and useEffect

### DevOps & Infrastructure

- **Version Control**: Git & GitHub
- **Containerization**: Docker & Docker Compose
- **Database**: MySQL 8.0
- **Development**: Hot reload and development servers

## 📂 Project Structure

```
task-management-system/
├── backend/                    # ASP.NET Core Web API
│   ├── src/
│   │   ├── ApplicationUser/    # User entity and management
│   │   ├── Authentication/     # JWT authentication logic
│   │   ├── Task/              # Task management (Entity, Service, Controller, DTOs)
│   │   ├── User/              # User management endpoints
│   │   ├── Notification/      # Email notification services
│   │   └── Services/          # Email and other services
│   ├── Data/                  # Database context and configurations
│   ├── Interfaces/            # Repository and service interfaces
│   ├── Migrations/            # Entity Framework migrations
│   └── Program.cs             # Application entry point
├── frontend/                  # React.js application
│   ├── src/
│   │   ├── pages/             # Main application pages
│   │   ├── components/        # Reusable UI components
│   │   └── styles/            # CSS and styling files
│   ├── public/                # Static files
│   └── package.json           # Frontend dependencies
├── docker-compose.yml         # Docker orchestration
└── README.md                  # Project documentation
```

## 🛠️ Installation & Setup

### Prerequisites

#### For Normal Setup:

- **.NET SDK 8.0** or later
- **Node.js 18+** and npm/yarn
- **MySQL Server 8.0** or later

#### For Docker Setup:

- **Docker** and **Docker Compose**

---

## 🚀 Quick Start with Docker (Recommended)

### 1. Clone the Repository

```bash
git clone <repository-url>
cd task-management-system
```

### 2. Start with Docker Compose

```bash
# Build and start all services (MySQL, Backend, Frontend)
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

### 3. Access the Applications

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5132
- **Swagger Documentation**: http://localhost:5132/swagger
- **MySQL**: localhost:3307

### 4. Stop Services

```bash
docker-compose down
```

---

## 🔧 Manual Setup (Development)

### Backend Setup

```bash
cd backend

# Install dependencies
dotnet restore

# Set up database connection string in appsettings.json
# Update DefaultConnection with your MySQL credentials

# Apply database migrations
dotnet ef database update

# Run the backend application
dotnet run
# Backend will be available at: https://localhost:5001 or http://localhost:5000
```

#### Backend Configuration

Create `appsettings.json` with your configurations:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDB;User=root;Password=yourpassword;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-here",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

### Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
echo "REACT_APP_API_URL=http://localhost:5132" > .env

# Start the frontend application
npm start
# Frontend will be available at: http://localhost:3000
```

---

## 🗄️ Database Setup

### Automatic Setup (Docker)

The database is automatically created and migrated when using Docker Compose.

### Manual Setup

1. Install MySQL Server 8.0+
2. Create a database named `TaskManagementDB`
3. Update connection string in `appsettings.json`
4. Run migrations: `dotnet ef database update`

---

## 🎯 API Endpoints

### Authentication

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/verify-email` - Email verification

### Tasks

- `GET /api/task` - Get all tasks
- `GET /api/task/my` - Get user's assigned tasks
- `POST /api/task` - Create new task (MANAGER/TEAM_LEAD only)
- `PUT /api/task/{id}` - Update task
- `DELETE /api/task/{id}` - Delete task (MANAGER/TEAM_LEAD only)
- `POST /api/task/{id}/assign` - Assign task to users (MANAGER/TEAM_LEAD only)

### Users

- `GET /api/user` - Get all users
- `PUT /api/user/{id}/role` - Change user role (MANAGER only)
- `GET /api/user/role/{role}` - Get users by role (MANAGER/TEAM_LEAD only)

---

## 👥 User Roles & Permissions

| Feature            | USER | TEAM_LEAD | MANAGER |
| ------------------ | ---- | --------- | ------- |
| View own tasks     | ✅   | ✅        | ✅      |
| View all tasks     | ❌   | ✅        | ✅      |
| Create tasks       | ❌   | ✅        | ✅      |
| Update task status | ✅   | ✅        | ✅      |
| Delete tasks       | ❌   | ✅        | ✅      |
| Assign tasks       | ❌   | ✅        | ✅      |
| Manage user roles  | ❌   | ❌        | ✅      |
| View all users     | ❌   | ✅        | ✅      |

---

## 📧 Email Notifications

The system automatically sends HTML-formatted emails for:

- **Task Assignment**: When a task is assigned to users
- **Status Updates**: When task status changes, notifications are sent to:
  - Task creator
  - All assigned users

---

## 🔒 Security Features

- JWT token-based authentication
- Role-based authorization
- Secure password hashing with ASP.NET Identity
- CORS protection
- Input validation and sanitization

---

## 🐳 Docker Configuration

The project includes Docker support with:

- **Multi-stage builds** for optimized images
- **Docker Compose** for easy orchestration
- **MySQL container** with persistent data
- **Networking** between services
- **Environment variable** management

---

## 🚀 Production Deployment

### Using Docker

```bash
# Build for production
docker-compose -f docker-compose.prod.yml up --build -d
```

### Manual Deployment

1. Build frontend: `npm run build`
2. Publish backend: `dotnet publish -c Release`
3. Configure production database and email settings
4. Deploy to your hosting provider

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

---

## 📄 License

This project is licensed under the MIT License.
