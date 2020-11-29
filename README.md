# On-Demand Crew Scheduling API

![Build & Tests](https://github.com/masduo/crew-scheduling-api/workflows/Build%20&%20Tests/badge.svg?branch=main)

### Skip to

- [Run](#run)
- [Tests](#tests)
- [Tech Stack](#tech-stack)
- [`// todo: ...`](#todo)

Crew Scheduling API (code name `crew-scheduling-api`) uses REST API design principles to provide two functions:

- Read the pilot availabiliy for any given period, and

- Schedule a pilot for any given period.

### Notes/Assumptions:

- Dates are received, stored, and retrieved in UTC without any timezone conversion. The date are in ISO-8601 format which conforms with [RFC 3339](https://tools.ietf.org/html/rfc3339#section-5.6)

Consult with the `/swagger` endpoint of the API for more details.

# Run 🚀

Depending on the use case, choose one of the following three approaches to run the API locally.

Then browse to:

- http://localhost:5000/swagger
- http://localhost:5000/healthcheck

## 1) Run with .Net CLI

To run on bare metals locally. Also to get secure connection:

1. Install [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

2. In your favourite shell run

   ```sh
   dotnet run -p src

   # or with file watcher
   dotnet watch -p src run
   ```

3. To get secure connection, trust the dotnet development certs `dotnet dev-certs https --trust` and then browse to https://localhost:5001/swagger

## 2) Run with Docker or Docker Compose

To run inside Docker and to test the Dockerfile:

1.  Install [Docker Desktop](https://www.docker.com/products/docker-desktop) (_tested on docker desktop community v2.5 - engine v19.03_)

2.  Then run

    ```sh
    docker-compose up --build

    # or do it yourself
    docker build -t crew-scheduling-api .

    docker run -p 5000:5000 crew-scheduling-api
    ```

## 3) Run with Skaffold

To run in a local Kubernetes cluster. Since Skaffold uses Helm to install and deploy the API, this approach is particularly useful to test how the API changes will play out in test or production Kubernetes clusters:

1.  Install

    - [Docker Desktop](https://www.docker.com/products/docker-desktop) (_tested on docker desktop community v2.5 - engine v19.03_)
    - Enable Kubernetes in Docker Desktop (_tested with K8s v1.19_)
    - [Helm](https://github.com/helm/helm/releases/tag/v3.3.4) (_tested on v3.3_)
    - [Skaffold](https://skaffold.dev/docs/install/) (_tested on v1.17_)

2.  Run

    ```sh
    skaffold run --tail

    # or with file watcher
    skaffold dev
    ```

# Tests

Two sets of tests are provided that can be run using the dotnet CLI:

```sh
dotnet test tests.unit

dotnet test tests.integration
```

Tests are also run in `Dockerfile` to stop faulty code being built into the API's Docker image.

# Tech Stack

### Stylecop

To enforce basic unobtrusive coding standards for consistency.

### Serilog

To enable structured logging with templates, and to allow logging with different formats. Configurable for different environments via `appsettings<.env>.json`.

The API logs to console (stdout). Another service (fluentd or filebeat) will be required to harvest the logs files and transfer them to log stores (elasticsearch, splunk, etc.).

### MediatR

To separate read models from write models, working towards CQRS (initially withou ES).

### Open API Specification (Swagger)

To add live/auto-generated documentation to the API.

### Docker and Docker Compose

To enable building and running the API inside Docker.

### Helm

To enable deploying the API to Kubernetes instance. There are two configuration available for development and production environments under `/helm/values/`.

Skaffold uses the development configuration to deploy locally.

For other environment a shell script will be added into the CI specification to deploy:

```yaml
# Install the API and set secrets, in a pseudo CI specification file
script:
  - helm upgrade --install crew-scheduling-api ./helm
    -f ./helm/values/production.yaml
    --set deployment.secret=$SECRET
```

### Skaffold

To deploy the API to Kubernetes insance running inside Docker Desktop.

### Versioning

Add versioning via urls. `/v1/` is the current default version.

### DotNetEnv

To sets environment variables for local development. Add a (_gitignored and dockerignored_) `.env` file to the root and write environemnt variables and secrets:

```sh
# to run with production configuration
ASPNETCORE_ENVIRONMENT=production
# set a secret variable
SECRET=[redacted-secret]
```

# `// todo: ...`

- API Versioning could move to configuration, currently it is defaulted to `v1.0`.

- The hypermedia links applied via `SharedModels.Links` is just to show the idea, it should be improved using HATEOAS libraries.

- The JSON DB files are prone to race condition and dirty reads if multiple read/write requests get issued to the server. Also since DB files exists per server, the API does not scale as it is. Migrating to a modern DB, or better management of file system with locking and unlocking would resolve this.

- JSON DBs could be cached at the server level or distributed. Any write to the file would trigger cache invalidation.

- Load Distribution: The schedules are prioritised based on the count of past and future schedules of available pilots. This can get smarter in many ways, e.g. the load should not take the whole history of schedules into account and instead should count schedules of a sliding recent period, or the load can be distributed based on the pilot's working days, so if a pilot works for only one day in week they should get more weigth, etc.

- XML documentation is done on what is expose on API's specification (swagger), and some of the interfaces. The domain entities and other interfaces need to be documented as well.

- The source code (`./src`) can be broken down to multiple assemblies in later iterations before going live. The current folder structure lends itself to that.

- POST schedules feature lacks tests, the integration and validation test would be similar to GET availability feature. The logs could be tested as well.