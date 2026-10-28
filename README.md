# BookKeeper

BookKeeper is a small library system. Users register and log in with a JWT, and have one of two roles: the librarian adds and deletes books and sees every book, and library users see the available books, loan them, see their own loans and return them. Each loan is tied to the user who made it, so a user only sees and returns their own loans. The domain logic and the API contract were both built with test-driven development (red, green, refactor). The API stores the books in PostgreSQL through EF Core, a Blazor frontend lets users do the whole flow in the browser, and the whole system runs with Docker Compose.

## Requirements

- .NET 10 SDK
- Docker Desktop (for Docker Compose)

## Run the tests

From the repository root:

```bash
dotnet test
```

This runs 61 tests:
- 12 domain tests for `BookService` (`BookKeeper.Tests/Domain`)
- 26 API contract tests, one for each line in `Documentation/api.md`: 18 for `/books` and 8 for `/auth` (`BookKeeper.Tests/Api`)
- 23 frontend tests for the pages and the API client (`BookKeeper.Web.Tests`)

The tests use `FakeBookRepository` and `FakeUserRepository`, so they do not need Docker or a database. The API tests register and log in through the API, and send the token like a real client. The frontend tests render the pages with bUnit and a fake API client, so they do not need the API.

## Run with Docker Compose

Start Docker Desktop, and run from the repository root:

```bash
docker compose up --build -d
```

This starts five services: the API, the Blazor frontend, PostgreSQL, pgAdmin and Traefik. The API creates the tables itself when it starts (EF Core migrations).

| Service | Address |
|---|---|
| Frontend | `http://localhost:5100` |
| API | `http://localhost:8080/books` |
| API through Traefik (ingress) | `http://localhost/books` |
| Scalar | `http://localhost:8080/scalar` |
| Health check | `http://localhost:8080/health` |
| pgAdmin | `http://localhost:5050` |
| Traefik dashboard | `http://localhost:8081/dashboard/` |

### Check the health status

```bash
docker compose ps
```

`api` and `db` show `(healthy)` when they are ready. `GET /health` returns `200 Healthy` when the API can reach the database, and `503 Unhealthy` when it cannot.

### View the data in pgAdmin

1. Open `http://localhost:5050`, and log in with `bookkeeper@book.com` and `password`.
2. Right-click **Servers** → **Register** → **Server**. Give it any name under **General**.
3. Under **Connection**: host `db`, port `5432`, maintenance database `bookkeeper`, username `bookkeeper`, password `password`.
4. Open **Servers** → your server → **Databases** → `bookkeeper` → **Schemas** → `public` → **Tables**.
5. Right-click `Books` or `Users` → **View/Edit Data** → **All Rows**. `Users` stores only the BCrypt hash of each password, and `LoanedByUserId` in `Books` is the id of the user who has the book.

The host is `db`, the service name in `docker-compose.yaml`, because pgAdmin runs in the same Docker network as the database.

### Where the data is stored

The books and the users are stored in PostgreSQL, in the Docker volume `db_data`. They survive a restart of the API container (`docker compose restart api`) and `docker compose down`.

| Command | What it does |
|---|---|
| `docker compose stop` | Stops all services. `docker compose start` starts them again. |
| `docker compose down` | Removes the containers, keeps the data. pgAdmin forgets the registered server. |
| `docker compose down -v` | Also deletes the volume, so all books and users are gone. |

### Build and run the API image alone

```bash
docker build -t bookkeeper-api -f BookKeeper.Api/Dockerfile .
docker run --rm -p 8080:8080 bookkeeper-api
```

The `Dockerfile` is in `BookKeeper.Api/`, but the build runs from the repository root, because the API also needs `BookKeeper.Core` and `BookKeeper.Infrastructure`. This checks that the image builds and the API starts. Without a database, the endpoints do not work, so use Docker Compose for everything else.

## Run the API without Docker

The API needs PostgreSQL, so the database still runs in Docker. From the repository root:

```bash
docker compose up -d db
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bookkeeper;Username=bookkeeper;Password=password" --project BookKeeper.Api
dotnet tool install --global dotnet-ef
dotnet ef database update --project BookKeeper.Infrastructure --startup-project BookKeeper.Api -- --environment Development
dotnet run --project BookKeeper.Api
```

- The connection string uses `localhost` instead of `db`, because the API runs on your machine, outside the Docker network.
- `dotnet user-secrets set` and `dotnet tool install` are only needed once. `database update` is only needed when the tables do not exist yet, or after a new migration.

The API runs on `http://localhost:5135`.

## Log in

There are two roles. One librarian adds and deletes books. Library users loan and return them.

1. Register: `POST /auth/register` with `{ "username": "anna", "password": "Secret-123", "role": "LibraryUser" }`. Use `"role": "Librarian"` for the librarian. Only one librarian can be registered.
2. Log in: `POST /auth/login` with the username and password, and copy `token` from the response.
3. Send the token with each request, as `Authorization: Bearer <token>`.

Without a valid token, the book endpoints answer `401 Unauthorized`. With the wrong role, they answer `403 Forbidden`. The token is valid for one hour. `GET /auth/me` shows the claims in your token.

## Use the frontend

The frontend is a Blazor app (`BookKeeper.Web`). It only talks to the API over HTTP, like Scalar and the `.http` file do.

- With Docker Compose: `docker compose up --build -d`, and open `http://localhost:5100`.
- Without Docker: start the API, then `dotnet run --project BookKeeper.Web`, and open the address in the log (`http://localhost:5020`). The frontend finds the API through `ApiBaseUrl` in `BookKeeper.Web/appsettings.Development.json`.

The flow in the browser:

1. **Register** (`/register`): choose a username, a password and a role. Only one librarian can be registered.
2. **Log in** (`/login`). You come to **Books**.
3. As the librarian: add a book with the form, and delete books. A loaned book is marked **Loaned**, and deleting it shows the API's error message.
4. As a library user: loan a book from **Books**, see it in **My loans**, and return it there.
5. **Log out** in the menu. To be two users at the same time, use a normal window for one and a private window for the other.

The login lasts while the tab is open. Reloading the page (F5) logs you out, and so does an expired token (after one hour).

## Test the API manually

There are two ways to test the API by hand. Both send real HTTP requests to the running API.

### Scalar

Open `http://localhost:8080/scalar` in a browser while the API runs in Docker Compose (`http://localhost:5135/scalar` with `dotnet run`). Scalar lists every endpoint and lets you send requests by clicking **Test Request**.

1. Register and log in with `/auth/register` and `/auth/login` (see **Log in**), and copy the `token`.
2. Paste the token under **Authentication** at the top of the page. Scalar sends it with every request.
3. As the librarian, send `POST /books` with a title and an author in the body, and copy the `id` from the response.
4. Paste it as `bookId` in loan and return (as a library user) and delete (as the librarian). Log in again and paste the other token to switch user.

If `GET /auth/me` still shows the previous user after you pasted a new token, Scalar is sending the old one. Reload the page with Ctrl+Shift+R, and paste the token again.

### REST Client

Open `BookKeeper.Api/BookKeeper.Api.http` in VS Code with the REST Client extension installed, and click **Send Request** above each request, from top to bottom. The file registers and logs in a librarian and two library users, and runs the whole flow with the right token on each request, including every error in `api.md` and the health check. It reuses the tokens and the book `id` from earlier responses automatically. `@host` at the top is `http://localhost:8080` for Docker Compose. Change it to `http://localhost:5135` for `dotnet run`.

### Why both

- **Scalar** shows the whole API in the browser, with no extra tools to install. It is the quickest way to see the endpoints and try them one by one.
- **The `.http` file** is part of the repository. It documents exactly which requests were tested by hand, runs the whole flow in order without copying ids, and can be run again by anyone.

## Project structure

| Folder or file | Content |
|---|---|
| `BookKeeper.Core` | Domain logic: `Book`, `BookService`, `User` and `Roles`, and the repository interfaces with their fakes (`Repository/`) |
| `BookKeeper.Infrastructure` | Data access with EF Core: `BookKeeperDbContext`, `EfBookRepository`, `EfUserRepository` and the migrations (`Migrations/`) |
| `BookKeeper.Api` | REST API: controllers, `Program.cs` and the `Dockerfile` |
| `BookKeeper.Api/Auth` | Registration and login (`AuthService`), password hashing with BCrypt (`PasswordHasher`), and the token setup for Scalar |
| `BookKeeper.Api/Dtos` | Request bodies: `RegisterRequest`, `LoginRequest` and `CreateBookRequest` |
| `BookKeeper.Tests/Domain` | Domain tests |
| `BookKeeper.Tests/Api` | API contract tests, `AuthHelper` (logs in a test user) and `BookKeeperFactory` (the API with the fakes) |
| `BookKeeper.Web` | Blazor frontend: the pages (`Components/Pages`), the API clients and `TokenStore` (`Api/`), the style (`wwwroot/app.css`) and the `Dockerfile` |
| `BookKeeper.Web.Tests` | Frontend tests with bUnit, the fake clients and `StubHandler` (a fake API for `BooksClient`) |
| `docker-compose.yaml` | The API, the frontend, PostgreSQL, pgAdmin and Traefik |
| `Excalidraw` | Sketch of the MVP flow (`flow.excalidraw` and `flow.png`) |
| `Documentation` | Specification and API contract |

`BookService` only depends on `IBookRepository`, and `AuthService` on `IUserRepository`. The API uses `EfBookRepository` and `EfUserRepository` (PostgreSQL), and the tests use `FakeBookRepository` and `FakeUserRepository` (memory). The controllers read the user id from the token and pass it to `BookService`, which passes it to the repository.

The frontend pages only know `IBooksClient` and `IAuthClient`. In the app they are `BooksClient` and `AuthClient`, which call the API with the token from `TokenStore`, and in the tests they are `FakeBooksClient` and `FakeAuthClient`.

## Documentation

- [Specification](Documentation/Specification.md): requirements, flow, interface, test list, decisions and limitations
- [API contract](Documentation/api.md): endpoints, requests, responses and status codes
- [MVP flow](Excalidraw/flow.png)
