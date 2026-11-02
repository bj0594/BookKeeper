# Architecture Decision Record

## Problem

The key that signs the JWTs (`Jwt:Key`) is in `BookKeeper.Api/appsettings.json`, and the database and pgAdmin passwords are in `docker-compose.yaml`. Both files are in a public GitHub repository. With the key, anyone can sign their own token with any user id and the role `Librarian`, and the API accepts it. Removing the key from the file is not enough, because it stays in the git history.

We need a way to give the API its secrets without committing them, that works with Docker Compose, with `dotnet run`, and in the tests.

## Alternatives

| | A: Environment variables from `.env` | B: Docker secrets | C: External secret manager |
| --- | --- | --- | --- |
| How it works | Docker Compose reads `.env` (not in git) and sets `Jwt__Key` on the API. `dotnet run` uses user-secrets. | Each secret is a file in `/run/secrets/`, which the API reads with `AddKeyPerFile`. | The API fetches the secrets from Azure Key Vault or HashiCorp Vault when it starts. |
| Advantages | No new package: .NET reads environment variables by default. `.env` is already in `.gitignore`. | The value is not visible in `docker inspect` or in the process environment. | Access control, an audit log of who read what, and rotation. |
| Disadvantages | The value can be read with `docker inspect` and from the process environment. | One file per secret, and a separate setup for `dotnet run` and the tests. | Needs a cloud service, an account and a network connection, also for local development. |
| Complexity | Low | Medium | High |
| Security | Good enough for local development and a small setup | Better | Best |
| Maintenance | `.env.example` must be kept in sync with the settings | The secret files must be created on every machine | The service must be run and paid for |

## Decision

Alternative A. The JWT key and the database passwords are read from environment variables. Docker Compose gets them from `.env`, which is not committed, and `dotnet run` gets them from user-secrets. The repository has a `.env.example` with placeholders. A new key replaces the one in the git history, and the API refuses to start if `Jwt:Key` is missing or shorter than 32 characters (`ValidateOnStart`).

## Why?

- It removes the secrets from the repository, which is the actual problem, with no new dependencies.
- The system runs only locally in Docker Compose. B and C protect against threats the system doesn't have yet (other people with access to the machine, many servers, rotation), at a much higher cost.
- .NET already reads `Jwt__Key` as `Jwt:Key`, so the code that uses the key does not change.
- The check at startup turns a missing key into a clear error when the API starts, instead of a 500 at the first login.
- If the API is deployed to a server, B is the next step. A makes that step small, because the settings already come from outside the code.

## Consequences

- The new key makes every existing token invalid, so everyone has to log in again.
- The old key is still in the git history, but it is worthless now that the API doesn't use it. The history is not rewritten.
- `docker compose up` stops with an error if `.env` is missing. The README explains how to copy `.env.example` to `.env`.
- The tests get the key from `BookKeeperFactory` (`ConfigureAppConfiguration` with `AddInMemoryCollection`), not from `appsettings.json`.
- Anyone with access to the machine can still read the secrets with `docker inspect`. That is accepted for now, and is the reason to move to B before deploying to a server.
