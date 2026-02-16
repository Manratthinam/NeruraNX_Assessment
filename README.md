# Neuranx

Neuranx is a task management application built with a Clean Architecture approach, utilizing a .NET 8 backend and an Angular frontend.

## Prerequisites

Before starting, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (LTS version recommended)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer edition)

## Getting Started

### Backend Setup

1.  **Navigate to the API directory:**
    ```bash
    cd src/neuranx.Api
    ```

2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Update Database Connection:**
    Open `src/neuranx.Api/appsettings.json` and ensure the `DefaultConnection` string points to your local SQL Server instance.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=neurannx;User Id=your_user;Password=your_password;TrustServerCertificate=True;Trusted_Connection=True"
    }
    ```

4.  **Apply Migrations:**
    Create the database and apply any pending migrations.
    ```bash
    dotnet ef database update --project ../neuranx.Infrastructure --startup-project .
    ```

5.  **Run the API:**
    ```bash
    dotnet run
    ```
    The API will be available at `http://localhost:5000` (or the port specified in `launchSettings.json`).

### Frontend Setup

1.  **Navigate to the frontend directory:**
    ```bash
    cd src/neuranx-web
    ```

2.  **Install dependencies:**
    ```bash
    npm install
    ```

3.  **Run the application:**
    ```bash
    ng serve
    ```
    Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Database Migration with EF Core

The solution uses Entity Framework Core with migrations stored in the `neuranx.Infrastructure` project.

-   **Add a new migration:**
    Run this command from the `src/neuranx.Api` directory:
    ```bash
    dotnet ef migrations add <MigrationName> --project ../neuranx.Infrastructure --startup-project .
    ```

-   **Update the database:**
    ```bash
    dotnet ef database update --project ../neuranx.Infrastructure --startup-project .
    ```

## API Architecture

The solution follows the **Clean Architecture** principles to ensure separation of concerns and scalability.

-   **neuranx.Domain**: Contains the core business logic, entities, enums, and interfaces. It has no dependencies on other projects.
-   **neuranx.Application**: Contains the application logic, DTOs, interfaces for infrastructure services, and CQRS handlers (if used). It depends on `Domain`.
-   **neuranx.Infrastructure**: Implements interfaces defined in `Application`. It handles database access (EF Core), identity, file systems, and other external services. It depends on `Application` and `Domain`.
-   **neuranx.Api**: The entry point of the application. It contains Controllers, Filters, and configuration settings (`appsettings.json`, `Program.cs`). It depends on `Application` and `Infrastructure`.

### Key Components

-   **Controllers**: REST API endpoints.
-   **DependencyInjection**: Registers services in the IoC container.
-   **Middleware**: Handles cross-cutting concerns like logging and exception handling.
-   **Authentication**: Implements JWT Authentication with Refresh Token support.

### Yet To Implement

-   **Pagination** , **Sorting** , **Filtering** ,**Search by task Id**, **Task History Tracking**