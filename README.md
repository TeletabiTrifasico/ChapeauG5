# Chapeau Restaurant Management System

![Chapeau Logo](https://raw.githubusercontent.com/username/ChapeauG5/main/ChapeauUI/logo.png)

A Windows Forms application developed for restaurant management at Chapeau Restaurant.

## Table of Contents
- [Description](#description)
- [Project Architecture](#project-architecture)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Usage](#usage)
- [Development Guidelines](#development-guidelines)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Description

Chapeau G5 is a desktop application built using .NET 8.0 and Windows Forms, designed to streamline restaurant operations including order management, inventory tracking, and employee management. This project follows a layered architecture with separate projects for data models, data access, business logic, and user interface.

## Project Architecture

The solution follows a layered architecture:

- **ChapeauModel:** Contains data models and entity classes
- **ChapeauDAL:** Data Access Layer for database operations
- **ChapeauService:** Business logic and service layer
- **ChapeauUI:** Windows Forms user interface
- **ChapeauG5:** Main project binding all components together

## Prerequisites

- .NET 8.0 SDK or later
- Windows operating system
- SQL Server (the application connects to a SQL Server database)
- Visual Studio 2022 or Visual Studio Code with C# extensions

## Getting Started

### Clone the Repository

Open a terminal or command prompt and clone the repository:

```bash
git clone https://github.com/yourusername/ChapeauG5.git
```

### Building the Project

**Using Visual Studio:**

- Open the solution file `ChapeauG5.sln` in Visual Studio
- Right-click on the solution in Solution Explorer and select **"Restore NuGet Packages"**
- Build the solution (`F6` or **Build > Build Solution**)

**Using Command Line:**

```bash
dotnet restore
dotnet build
```

### Running the Application

**Using Visual Studio:**

- Set `ChapeauUI` as the startup project (Right-click `ChapeauUI` > **Set as Startup Project**)
- Press `F5` or **Debug > Start Debugging**

**Using Command Line:**

```bash
dotnet run --project ChapeauUI/ChapeauUI.csproj
```

## Database Setup

The application uses a SQL Server database. You need to:

- Create a database named `chapeau` in SQL Server
- Run the database setup script:
  ```
  (Script content to be provided)
  ```
- Update the connection string in `App.config`:

```xml
<connectionStrings>
  <add name="ChapeauG5DB"
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=chapeau;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

## Usage

### Login Credentials

Use the following test credentials to log in:

- **Manager:**
  - Username: `M001`
  - Password: `1234`

- **Waiter:**
  - Username: `W001`, `W002`, `W003`
  - Password: `1234`

- **Bar:**
  - Username: `B001`
  - Password: `1234`

- **Kitchen:**
  - Username: `K001`, `K002`
  - Password: `1234`

### Application Flow

- The application starts with the login screen.
- Upon successful login, it will take you to the appropriate dashboard based on your role.
- Different forms will be shown for different user roles (currently in development).

## Development Guidelines

### Adding New Forms
- Create a new Windows Form class in the `ChapeauUI` project.
- Update the `OpenAppropriateForm` method in `LoginForm.cs` to navigate to your new form based on role.

### Database Access
- Create a model class in the `ChapeauModel` project.
- Create a DAO class in the `ChapeauDAL` project inheriting from `BaseDao`.
- Create a service class in the `ChapeauService` project to handle business logic.

## Troubleshooting

### Database Connection Issues
- Verify connection string in `App.config`.
- Check that SQL Server is running.
- Ensure the database exists.
- Verify your SQL Server credentials.
- If using a remote server, ensure `TrustServerCertificate` is set to `True`.

### Build Errors
- Clear `bin/obj` folders using `dotnet clean`.
- Make sure you're using .NET 8.0 SDK.
- Restore NuGet packages.
- Check for syntax errors.

### Runtime Errors
- Look for exceptions in the debug output.
- Check that all required tables exist in the database.
- Verify that column names in the database match property names in the code.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

_Last updated: May 20, 2025 by ChapeauG5 Team_
