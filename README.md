# On-Demand Crew Scheduling API

![Build & Tests](https://github.com/masduo/crew-scheduling-api/workflows/Build%20&%20Tests/badge.svg?branch=main)

### Skip to

- [Run](#run)
- [Tests](#tests)
- [Tech Stack](#tech-stack)
- [`// todo: ...`](#todo)

Crew Scheduling API (code name `crew-scheduling-api`) uses REST API design principles to provide two functions:

- Query the next available pilot to schedule for the requested period.

- Schedule a pilot for the requested period.

### Assumptions:

- The load is distributed evenly between available pilots using the count of their current schedules.

- The schedule endpoint re-checks the availability of the pilot before assigning them. But it does re-check the priority.

- Dates are received, stored, and retrieved in UTC without any timezone conversion. The date are in ISO-8601 format which conforms with [RFC 3339](https://tools.ietf.org/html/rfc3339#section-5.6)

- Please consult with `/swagger` endpoint for more details.

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

Enforces basic unobtrusive coding standards for consistency of commits.

### Serilog

Enables structured logging, and to allow logging with different configurations per environment.

The API logs to console (stdout). Another service (e.g. fluentd, filebeat) will be required to harvest the log files and ship them to log stores (e.g. Elasticsearch).

### MediatR

Enables separation of query models and handlers (read the next available pilot) from commands models and handlers (schedule a pilot). This is a step towards implementing CQRS architectural pattern.

### Open API Specification (Swagger)

Adds live/auto-generated documentation to the API endpoints.

### Docker and Docker Compose

Enables containerisation of the API by building and running it inside Docker. Makes CI/CD a breeze.

### Helm Chart

Enables deploying the API to Kubernetes instance. There are two configuration available for development and production environments under `/helm/values/`.

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

Deploys the API to Kubernetes insance running inside Docker Desktop. See [Run with Skaffold](#run-with-skaffold).

### API Versioning

Adds versioning via urls. `/v1/` is the current default version.

### DotNetEnv

Sets environment variables for local development. Add a (_gitignored and dockerignored_) `.env` file to the root and write environemnt variables and secrets:

```sh
# to run with production configuration
ASPNETCORE_ENVIRONMENT=production
# set a secret variable
SECRET=[redacted-secret]
```

# `// todo: ...`

This is more of a POC and direction of the solution for the problem. Below are few  extension points I would pick next to iterate on:

- The source code (`./src`) can be broken down into multiple assemblies. The current folder structure tries to reflect what the assemblies would have been: `Stores` for reading and writing data, `Domain` for shared entities, `Handlers` for services.

- The hypermedia links used via `SharedModels.Links` should be improved using proper HATEOAS paradigms e.g. HAL.

- POST `/schedules` feature lacks tests. The controller integration tests, and model validation tests would be quite similar to GET `/availability` feature tests. Also there are not tests for `AvailabilityQueryHandler`. The logs must be tested as well.

- XML documentation is done on what is expose on API's specification (swagger), and some of the interfaces. Domain entities and the rest of the interfaces need to be documented as well.

- Currently the models and handlers for quries and commands are separated, but they both access the same DB files (`schedules.json`). Ideally the read DB can be separated out into an eventually consistent file, a denormalised view per se. Anytime that a command changes the state of system, it should then publish an event, e.g. via a message queue, and a separate process would consume it to update the view data.

- Numerous issues arise by using an auto-incremented integer data type for primary keys. The Id for the pilots should change to a less predictable value, e.g. a GUID.

- The JSON DB files are prone to race condition and dirty reads if multiple read/write requests get issued to the server. Also since DB files exists per server, the API does not scale as it stands. These must either migrate to a modern DB System, or the file system management must be improved to manage race condition.

- Data for queries can be cached at the server level or in a distributed cache. Then any command that changes state should trigger a cache invalidation.

- Load Distribution: The schedules are prioritised based on the count of past and future schedules of available pilots. This can get smarter. For example the load should not take the whole history of schedules into account and instead should count schedules of a sliding recent period. Or the load can be distributed based on the pilot's working days, so if a pilot works for only one day in week they should get more weigth, etc. Another way to tackle the efficiency of the algorithm is to allow revisit past distributions data and make informed decisions using those reports.

Thanks for your time and consideration!