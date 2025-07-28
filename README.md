# 📚 BookLibrary API  

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-green)
![Build](https://github.com/Buggg43/BookLibraryApi/actions/workflows/ci.yml/badge.svg)


---

A RESTful API for managing books and user accounts.  
Built with **ASP.NET Core 9**, following **CQRS + MediatR + FluentValidation** architecture.  

---

## 🚀 Features

✅ User registration and login (hashed passwords, JWT + Refresh Tokens)  
✅ Full session management – logout, logout/all, automatic token cleanup  
✅ CQRS (Commands/Queries) + MediatR  
✅ FluentValidation + AutoMapper  
✅ Error handling middleware  
✅ Unit and integration tests (xUnit + EF Core InMemory)

---

## 🛠 Technologies

- ASP.NET Core 9
- Entity Framework Core
- MediatR
- FluentValidation
- AutoMapper
- JWT (JSON Web Tokens)
- CQRS
- xUnit (unit and integration tests)

---

## 📐 Architecture

- **Commands** – write operations (create/update/delete)  
- **Queries** – read operations (filtering/fetching)  
- **MediatR** – request/command handling directly from controllers  
- **FluentValidation** – input validation  
- **JWT & Refresh Tokens** – full session management  

---

## 🔑 Endpoints

### 🔐 AuthController

| Method | Endpoint                | Description                            |
|--------|-------------------------|----------------------------------------|
| POST   | `/api/auth/register`    | Register a new user                   |
| POST   | `/api/auth/login`       | Login and return token pair           |
| POST   | `/api/auth/refresh`     | Refresh Access Token                  |
| POST   | `/api/auth/logout`      | Logout from one device                |
| POST   | `/api/auth/logout/all`  | Logout from all devices               |
| GET    | `/api/auth/me`          | Get data of logged-in user           |
| PUT    | `/api/auth/me/password` | Change password                       |
| PUT    | `/api/auth/me`          | Edit account data                     |
| GET    | `/api/auth/admin/users` | List all users (admin only)           |
| PUT    | `/api/auth/admin/users/role` | Change user role (admin only)   |
| DELETE | `/api/auth/admin/users/{id}` | Delete a user (admin only)      |

### 📖 BooksController

| Method | Endpoint                   | Description                           |
|--------|----------------------------|---------------------------------------|
| GET    | `/api/books`               | List logged-in user's books          |
| GET    | `/api/books/{id}`          | Get details of a book                |
| POST   | `/api/books`               | Add a new book                       |
| PUT    | `/api/books/{id}`          | Edit a book                          |
| DELETE | `/api/books/{id}`          | Delete a book                        |
| GET    | `/api/books/admin/all-books` | List all books (admin only)       |

---

## 📦 Project Structure

```txt
src/
 └── BookLibraryApi
     ├── Features
     ├── Models
     ├── Services
     └── Middleware

tests/
 └── BookLibraryApi.Tests
```

---

## ▶ How to Run

### 🔹 1. Clone the repository

```bash
git clone https://github.com/YOUR_GITHUB_USERNAME/BookLibraryApi.git
cd BookLibraryApi
```

### 🔹 2. Run the API

```bash
dotnet build
dotnet run --project src/BookLibraryApi
```

Swagger: [https://localhost:5001/swagger](https://localhost:5001/swagger)

### 🔹 3. Run the tests

```bash
dotnet test
```

---

## 🛠 CI (GitHub Actions)

Add the file **.github/workflows/dotnet.yml**:

```yaml
name: .NET CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --verbosity normal
```

Add the badge to README:

```md
![Build](https://github.com/YOUR_GITHUB_USERNAME/BookLibraryApi/actions/workflows/dotnet.yml/badge.svg)
```

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

---

## 👨‍💻 Author

Created as an advanced **ASP.NET Core + CQRS + MediatR** architecture exercise.  
Code is clean, testable, and ready for further development.
