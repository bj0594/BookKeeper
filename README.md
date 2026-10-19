# BookKeeper

BookKeeper is a small library system. A user can add books, list them, loan a book out, return it and delete it. The domain logic and the API contract were both built with test-driven development (red, green, refactor). The API stores the books in PostgreSQL through EF Core, and the whole system runs with Docker Compose.

## Requirements

- .NET 10 SDK
- Docker Desktop (for Docker Compose)

## Run the tests

From the repository root:

```bash
dotnet test
```

This runs 13 tests:
- 6 domain tests for `BookService` (`BookKeeper.Tests/Domain`)
- 7 API contract tests, one for each `/books` line in `Documentation/api.md` (`BookKeeper.Tests/Api`)

The tests use `FakeBookRepository`, so they do not need Docker or a database.

## Run with Docker Compose

Start Docker Desktop, and run from the repository root:

```bash
docker compose up --build -d
```

This starts four services: the API, PostgreSQL, pgAdmin and Traefik. The API creates the tables itself when it starts (EF Core migrations).

| Service | Address |
|---|---|
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
5. Right-click `Books` → **View/Edit Data** → **All Rows**.

The host is `db`, the service name in `docker-compose.yaml`, because pgAdmin runs in the same Docker network as the database.

### Where the data is stored

The books are stored in PostgreSQL, in the Docker volume `db_data`. They survive a restart of the API container (`docker compose restart api`) and `docker compose down`.

| Command | What it does |
|---|---|
| `docker compose stop` | Stops all services. `docker compose start` starts them again. |
| `docker compose down` | Removes the containers, keeps the data. pgAdmin forgets the registered server. |
| `docker compose down -v` | Also deletes the volume, so all books are gone. |

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
- `dotnet user-secrets set` and `dotnet tool install` are only needed once. `database update` is only needed when the tables do not exist yet.

The API runs on `http://localhost:5135`.

## Test the API manually

There are two ways to test the API by hand. Both send real HTTP requests to the running API.

### Scalar

Open `http://localhost:8080/scalar` in a browser while the API runs in Docker Compose (`http://localhost:5135/scalar` with `dotnet run`). Scalar lists every endpoint and lets you send requests by clicking **Test Request**.

1. Send `POST /books` with a title and an author in the body.
2. Copy the `id` from the response.
3. Paste it as `bookId` in loan, return and delete.

### REST Client

Open `BookKeeper.Api/BookKeeper.Api.http` in VS Code with the REST Client extension installed, and click **Send Request** above each request, from top to bottom. The file covers every line in `api.md`, including the two 409 errors and the health check, and reuses the `id` from the first request automatically. `@host` at the top is `http://localhost:8080` for Docker Compose. Change it to `http://localhost:5135` for `dotnet run`.

### Why both

- **Scalar** shows the whole API in the browser, with no extra tools to install. It is the quickest way to see the endpoints and try them one by one.
- **The `.http` file** is part of the repository. It documents exactly which requests were tested by hand, runs the whole flow in order without copying ids, and can be run again by anyone.

## Project structure

| Folder or file | Content |
|---|---|
| `BookKeeper.Core` | Domain logic: `Book` and `BookService`, and `IBookRepository` with `FakeBookRepository` (`Repository/`) |
| `BookKeeper.Infrastructure` | Data access with EF Core: `BookKeeperDbContext`, `EfBookRepository` and the migrations (`Migrations/`) |
| `BookKeeper.Api` | REST API: controller, request type, `Program.cs` and the `Dockerfile` |
| `BookKeeper.Tests/Domain` | Domain tests |
| `BookKeeper.Tests/Api` | API contract tests |
| `docker-compose.yaml` | The API, PostgreSQL, pgAdmin and Traefik |
| `Excalidraw` | Sketch of the MVP flow (`flow.excalidraw` and `flow.png`) |
| `Documentation` | Specification and API contract |

`BookService` only depends on `IBookRepository`. The API uses `EfBookRepository` (PostgreSQL), and the tests use `FakeBookRepository` (memory).

## Documentation

- [Specification](Documentation/Specification.md): requirements, flow, interface, test list, decisions and limitations
- [API contract](Documentation/api.md): endpoints, requests, responses and status codes
- [MVP flow](Excalidraw/flow.png)
