# 📚 BookLibrary API  

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-green)
![Build](https://github.com/YOUR_GITHUB_USERNAME/BookLibraryApi/actions/workflows/dotnet.yml/badge.svg)

---

RESTful API do zarządzania książkami i kontami użytkowników.  
Projekt zbudowany w **ASP.NET Core 9**, oparty na architekturze **CQRS + MediatR + FluentValidation**.  

---

## 🚀 Funkcjonalności

✅ Rejestracja i logowanie (hashowane hasła, JWT + Refresh Tokeny)  
✅ Pełna obsługa sesji – logout, logout/all, automatyczne czyszczenie tokenów  
✅ CQRS (Commands/Queries) + MediatR  
✅ FluentValidation + AutoMapper  
✅ Middleware błędów  
✅ Testy jednostkowe i integracyjne (xUnit + EF Core InMemory)

---

## 🛠 Technologie

- ASP.NET Core 9
- Entity Framework Core
- MediatR
- FluentValidation
- AutoMapper
- JWT (JSON Web Tokens)
- CQRS
- xUnit (testy jednostkowe i integracyjne)

---

## 📐 Architektura

- **Commands** – logika zapisu (create/update/delete)  
- **Queries** – logika odczytu (filtrowanie/pobieranie)  
- **MediatR** – delegowanie zapytań i komend z kontrolerów  
- **FluentValidation** – walidacja danych wejściowych  
- **JWT & Refresh Tokeny** – pełna obsługa sesji użytkowników  

---

## 🔑 Endpoints

### 🔐 AuthController

| Metoda | Endpoint           | Opis                                  |
|--------|--------------------|---------------------------------------|
| POST   | `/api/auth/register` | Rejestracja nowego użytkownika       |
| POST   | `/api/auth/login`    | Logowanie i zwrot pary tokenów       |
| POST   | `/api/auth/refresh`  | Odświeżenie Access Tokena           |
| POST   | `/api/auth/logout`   | Wylogowanie jednego urządzenia       |
| POST   | `/api/auth/logout/all` | Wylogowanie ze wszystkich urządzeń |
| GET    | `/api/auth/me`       | Dane zalogowanego użytkownika       |
| PUT    | `/api/auth/me/password` | Zmiana hasła                     |
| PUT    | `/api/auth/me`       | Edycja danych konta                 |
| GET    | `/api/auth/admin/users` | Lista wszystkich użytkowników (admin) |
| PUT    | `/api/auth/admin/users/role` | Zmiana roli użytkownika (admin) |
| DELETE | `/api/auth/admin/users/{id}` | Usunięcie użytkownika (admin) |

### 📖 BooksController

| Metoda | Endpoint              | Opis                                       |
|--------|-----------------------|--------------------------------------------|
| GET    | `/api/books`          | Lista książek zalogowanego użytkownika     |
| GET    | `/api/books/{id}`     | Szczegóły jednej książki                   |
| POST   | `/api/books`          | Dodanie nowej książki                      |
| PUT    | `/api/books/{id}`     | Edycja książki                            |
| DELETE | `/api/books/{id}`     | Usunięcie książki                         |
| GET    | `/api/books/admin/all-books` | Lista wszystkich książek (admin) |

---

## 📦 Struktura projektu

src/
└── BookLibraryApi
├── Features
│ ├── Users
│ ├── Books
├── Models
├── Services
└── Middleware

tests/
└── BookLibraryApi.Tests

yaml
Kopiuj
Edytuj

---

## ▶ Uruchomienie projektu

### 🔹 1. Klonowanie repozytorium

```bash
git clone https://github.com/YOUR_GITHUB_USERNAME/BookLibraryApi.git
cd BookLibraryApi
🔹 2. Uruchomienie API
bash
Kopiuj
Edytuj
dotnet build
dotnet run --project src/BookLibraryApi
Swagger: https://localhost:5001/swagger

🔹 3. Uruchomienie testów
bash
Kopiuj
Edytuj
dotnet test
🛠 CI (GitHub Actions)
W repozytorium dodaj plik .github/workflows/dotnet.yml:

yaml
Kopiuj
Edytuj
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
Dzięki temu w README wyświetli się badge:

md
Kopiuj
Edytuj
![Build](https://github.com/YOUR_GITHUB_USERNAME/BookLibraryApi/actions/workflows/dotnet.yml/badge.svg)
📜 Licencja
Projekt dostępny na licencji MIT.

👨‍💻 Autor
Projekt stworzony jako zaawansowane ćwiczenie architektury ASP.NET Core + CQRS + MediatR.
Kod gotowy do rozwoju, z testami integracyjnymi i wzorcami stosowanymi w projektach komercyjnych.
