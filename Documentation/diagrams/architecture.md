# Architecture

Every part of BookKeeper as it runs with docker-compose.yaml, how the parts talk to each other, where the data is stored, and where the JWT is sent. Solid arrows are requests, dotted arrows are dependencies between the projects and on the Docker Engine. Start order (depends_on): db first, then api when db is healthy, then web; pgadmin waits for db.

```mermaid
flowchart LR
    subgraph host["Your machine (localhost)"]
        browser(["Browser"])
        restclient["REST Client<br/>BookKeeper.Api.http"]
        engine[["Docker Engine<br/>/var/run/docker.sock"]]
    end

    subgraph compose["Docker Compose network: the containers find each other by service name"]
        traefik["traefik<br/>ingress"]
        web["web<br/>Blazor Server<br/>TokenStore: token per tab, in memory"]

        subgraph api["api: ASP.NET Core"]
            apiproj["BookKeeper.Api<br/>controllers, issues and checks the JWT<br/>Scalar on /scalar"]
            core["BookKeeper.Core<br/>BookService, the rules"]
            infra["BookKeeper.Infrastructure<br/>EF Core repositories"]
            apiproj -- "calls" --> core
            infra -. "implements IBookRepository" .-> core
            apiproj -. "registers" .-> infra
        end

        db[("db<br/>PostgreSQL 17<br/>tables Books, Users")]
        vol[["volume db_data"]]
        pgadmin["pgadmin"]
    end

    browser -- "HTTP + WebSocket (SignalR)<br/>localhost:5100" --> web
    browser -- "HTTP localhost:80<br/>dashboard localhost:8081" --> traefik
    browser -- "HTTP localhost:8080<br/>/scalar" --> apiproj
    browser -- "HTTP localhost:5050" --> pgadmin
    restclient -- "HTTP localhost:8080<br/>Authorization: Bearer JWT" --> apiproj
    traefik -- "every path → api:8080" --> apiproj
    traefik -. "reads the labels" .-> engine
    web -- "HTTP api:8080<br/>Authorization: Bearer JWT" --> apiproj
    infra -- "Npgsql db:5432" --> db
    pgadmin -- "db:5432" --> db
    db --- vol

    classDef client fill:#e8f0fe,stroke:#4a6fa5,color:#1a1a1a
    classDef storage fill:#fff4e0,stroke:#b07d2b,color:#1a1a1a
    class browser,restclient client
    class db,vol storage```
