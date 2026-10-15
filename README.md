# BookKeeper

BookKeeper is a small library system. A user can add books, list them, loan a book out, return it and delete it. The domain logic was built with test-driven development (red, green, refactor), and is available through a REST API.

## Requirements

- .NET 10 SDK

## Run the tests

From the repository root:

    dotnet test

This runs 14 tests:
- 6 domain tests for `BookService` (`BookKeeper.Tests/Domain`)
- 7 API contract tests, one for each line in `Documentation/api.md` (`BookKeeper.Tests/Api`)
- 1 smoke test that checks that the API can start in memory

## Run the API

    dotnet run --project BookKeeper.Api

The API runs on `http://localhost:5135`. Books are stored in memory, so they are lost when the API stops.

## Test the API manually

There are two ways to test the API by hand. Both send real HTTP requests to the running API.

### Scalar

Open `http://localhost:5135/scalar` in a browser while the API is running. Scalar lists every endpoint and lets you send requests by clicking **Test Request**.

1. Send `POST /books` with a title and an author in the body.
2. Copy the `id` from the response.
3. Paste it as `bookId` in loan, return and delete.

### REST Client

Open `BookKeeper.Api/BookKeeper.Api.http` in VS Code with the REST Client extension installed, and click **Send Request** above each request, from top to bottom. The file covers every line in `api.md`, including the two 409 errors, and reuses the `id` from the first request automatically.

### Why both

- **Scalar** shows the whole API in the browser, with no extra tools to install. It is the quickest way to see the endpoints and try them one by one.
- **The `.http` file** is part of the repository. It documents exactly which requests were tested by hand, runs the whole flow in order without copying ids, and can be run again by anyone.

## Project structure

| Folder | Content |
|---|---|
| `BookKeeper.Core` | Domain logic: `Book` and `BookService` |
| `BookKeeper.Api` | REST API: controller, request type and `Program.cs` |
| `BookKeeper.Tests/Domain` | Domain tests |
| `BookKeeper.Tests/Api` | API contract tests and the smoke test |
| `Excalidraw` | Sketch of the MVP flow (`flow.excalidraw` and `flow.png`) |
| `Documentation` | Specification, API contract and the assignment text |

## Documentation

- [Specification](Documentation/Specification.md): requirements, flow, interface, test list, decisions and limitations
- [API contract](Documentation/api.md): endpoints, requests, responses and status codes
- [MVP flow](Excalidraw/flow.png)