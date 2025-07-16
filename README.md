# BookLibrary API

RESTful API do zarządzania książkami oraz kontami użytkowników – zbudowane w technologii ASP.NET Core, z wykorzystaniem nowoczesnej architektury CQRS + MediatR + FluentValidation.

---

## Technologie

- ASP.NET Core 8
- Entity Framework Core
- MediatR
- FluentValidation
- AutoMapper
- JWT (JSON Web Tokens)
- SQL Server (LocalDB / SQL Express)

---

## Architektura

Projekt oparty na wzorcu CQRS:
- Commands – do zapisu (np. tworzenie, aktualizacja, usuwanie)
- Queries – do odczytu (np. filtrowanie, pobieranie danych)
- MediatR – do delegowania logiki z kontrolerów
- FluentValidation – walidacja wszystkich DTO i komend
- JWT i Refresh Tokeny – pełna obsługa sesji

---

## System uwierzytelniania

- Rejestracja i logowanie z hashowaniem haseł (PasswordHasher)
- Generowanie Access Tokenów (15 minut) i Refresh Tokenów (7 dni)
- Automatyczne czyszczenie tokenów przez BackgroundService
- Obsługa logout i logout/all per urządzenie

---

## Endpoints – podział

### AuthController

| Metoda | Endpoint | Opis |
|--------|----------|------|
| POST   | /register             | Rejestracja nowego użytkownika |
| POST   | /login                | Logowanie i zwrot pary tokenów |
| PUT    | /refresh              | Odświeżenie Access Tokena |
| POST   | /logout               | Wylogowanie jednego urządzenia |
| POST   | /logout/all           | Wylogowanie ze wszystkich urządzeń |
| GET    | /me                   | Dane zalogowanego użytkownika |
| PUT    | /me/password          | Zmiana hasła |
| PUT    | /me                   | Edycja danych konta |
| GET    | /admin/users          | Lista wszystkich użytkowników (admin) |
| PUT    | /admin/users/role     | Zmiana roli użytkownika (admin) |
| DELETE | /admin/users/{id}     | Usunięcie użytkownika (admin) |

---

### BooksController

| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET    | /books                     | Lista książek zalogowanego użytkownika (z filtrami i paginacją) |
| GET    | /books/{id}               | Szczegóły jednej książki |
| POST   | /books                    | Dodanie nowej książki |
| PUT    | /books/{id}               | Edycja książki |
| DELETE | /books/{id}               | Usunięcie książki |
| GET    | /books/admin/all-books    | Lista wszystkich książek w systemie (admin) |

---

## Przykładowe DTO

```json
// RegisterUserDto
{
  "username": "testuser",
  "password": "Secure123"
}

// TokenPairDto
{
  "accessToken": "eyJhbGciOiJIUz...",
  "refreshToken": "d9841bc9..."
}
```

---

## Uruchomienie projektu

1. Skonfiguruj connection string do SQL Server w `appsettings.json`
2. Uruchom migracje (jeśli są dodane)
3. Włącz projekt i otwórz Swagger: `https://localhost:{port}/swagger`
4. Przetestuj rejestrację i logowanie – token JWT można wkleić w Swaggerze (Authorize 🔒)

---

## Struktura katalogów

```
Features
 ┣ Users
 ┃ ┣ Commands
 ┃ ┣ Queries
 ┃ ┗ Validators
Models
 ┣ Dtos
 ┗ User.cs, Book.cs
Services
 ┗ JwtService.cs, RefreshTokenCleanUpService.cs
```

---

## Autor

Projekt stworzony jako ćwiczenie architektury aplikacji ASP.NET Core.  
Kod uporządkowany, testowalny, gotowy do rozwoju lub prezentacji jako część portfolio.

---

## Postęp

- [x] CQRS
- [x] MediatR
- [x] Tokeny JWT + Refresh
- [x] Middleware błędów
- [x] Walidacja FluentValidation
- [ ] Testy jednostkowe (w trakcie realizacji)
