# Contributing to the Housing Finance Interim API

This document covers local setup, configuration, build and run, testing, known limitations, and the
development/deployment workflow for the Housing Finance Interim API. For an overview of the service, its
endpoints and dependencies, see the [README](README.md).

## Requirements

1. [Docker][docker-download] with the **`docker compose`** v2 plugin.
2. A recent version of [AWS CLI V2][aws-cli].
3. An AWS CLI profile for an environment the Finance DB is deployed in.
4. A recent version of the [Session Manager Plugin][session-manager-install] for the AWS CLI.
5. A **`LBHPACKAGESTOKEN`** with `read:packages` access to the `LBHackney-IT` GitHub Packages feed — the
   `Hackney.*` packages are not published to nuget.org:
   ```sh
   export LBHPACKAGESTOKEN=<token>
   ```
6. Copy [`.env.sample`](.env.sample) to `.env` and fill in the values. This is the supported local
   configuration path: `docker compose` interpolates every runtime variable from it, and a local Debug build
   (`make serve-local`) loads it directly via dotenv.net. See [Environment variables](#environment-variables)
   below.

On Windows:
- You will need to use Git Bash or Windows Subsystem for Linux to run the Make commands.
- To run with Docker you will need Windows Subsystem for Linux v2 with Docker Engine installed on it (NOT
  Docker Desktop on base Windows — this [does not support the host networking driver][host-networking-prereq]).

On macOS (Monterey — possibly others):
- You will need to [turn off the AirPlay receiver](https://developer.apple.com/forums/thread/682332) from your
  sharing settings due to a port 5000 conflict.

## Environment variables

- See the [Serverless configuration](HousingFinanceInterimApi/serverless.yml) `environment` block for the
  variables that are set from Parameter Store.
- Copy [`.env.sample`](.env.sample) into the same directory, rename it to `.env`, then set the values.
- Pay particular attention to `GOOGLE_API_KEY`, which must be JSON wrapped in single quotes and on a single
  line.

## Port forwarding to the finance DB

This is required to connect the API to a functional database locally.

1. Open the [finance_database.mk](finance_database.mk) Makefile, which contains the port-forwarding commands.
2. There is a helper that generates the connection string for a given AWS profile — run it and copy the output
   into your `.env`:
   ```sh
   make -f finance_database.mk local_connection_string_to_env
   ```
3. Ensure you have an AWS CLI profile (default `housing-{stage}`) matching the `PROFILE` variable and the same
   profile the credentials were sourced from.
4. Edit the file to use the correct stage (development / staging / production).
5. If you use an SSO profile, refresh your login with:
   ```sh
   make -f finance_database.mk sso_login
   ```
6. Start the port forwarding:
   ```sh
   make -f finance_database.mk port_forwarding_to_hfs_db
   ```

`make remote-db` performs steps 5 and 6 for the `housing-development` profile in one go.

### Connecting with a local client (optional)

- Connect to `localhost` on port `1433`.
- Use the username and password printed to the console after the port forwarding starts.

## Running with Docker

`docker compose` reads the `.env` file in the root directory and connects to the port-forwarded database on
`localhost:1433` using host networking.

On Windows you need Docker Desktop running with WSL2 integration enabled.

From the root directory:

```sh
make build && make serve
```

The API is then available on the port configured in [`docker-compose.yml`](docker-compose.yml).

## Running locally without Docker

The application loads the `.env` file in the root directory and connects to the port-forwarded database on
`localhost:1433`:

```sh
make serve-local
# or
dotnet run --project HousingFinanceInterimApi/HousingFinanceInterimApi.csproj
```

You can also configure your IDE of choice to run the `HousingFinanceInterimApi` project.

## Testing

```sh
make test
```

`make test` builds the test image ([`HousingFinanceInterimApi.Tests/Dockerfile`](HousingFinanceInterimApi.Tests/Dockerfile))
and runs the xUnit suite inside it against the `test-database` SQL Server container from
[`docker-compose.yml`](docker-compose.yml).

- The suite is **xUnit-only** (with FluentAssertions, Moq, AutoFixture and Bogus).
- Database-backed tests use the `DB_SERVER` / `DB_USERNAME` / `DB_PASSWORD` / `DB_DATABASE` environment
  variables supplied by `docker-compose.yml` and call `EnsureCreated()` against the stub database.
- `MockWebApplicationFactory`/`IntegrationTests` are present but their integration-test bodies are commented
  out.
- If the database schema changes, remove and recreate the database image (the `restart-db` make target does
  this).
- The `BaseApi.Tests` folder contains only a leftover `launchSettings.json`; it is not a project, is not in the
  solution and is not built.

### Agreed testing approach

- Use xUnit, FluentAssertions and Moq.
- Follow a TDD approach; keep tests independent of each other.
- Gateway tests interact with a real test instance of the database.
- Test coverage should never go down.
- All use cases should be covered by E2E tests.
- Unit and E2E tests run in CI.
- Test database schemas should match the production database schema.

## Known limitations

- The management-fee branch of `ApplyHeadOfChargeUseCase` throws `NotImplementedException`
  (`ManagementFeeUseCase`, `ChargesMaintenanceApiGateway`, `Infrastructure/UpdateAssetSummary`).
- The `importCashFileTransactions` / `importHousingFileTransactions` step responses share a process-wide
  `WAIT_DURATION` environment variable, which makes some `NextStepTime` unit-test assertions order-dependent.
- `AutoMapper` is referenced and mapping profiles exist, but `AddAutoMapper` is never called, so the mapper is
  not registered in DI.
- `Microsoft.AspNetCore.HealthChecks` 1.0.0 is referenced but the only consumer (`DbHealthCheckUseCase`) is
  not registered, and no health-checks middleware is wired up.

## Development workflow

- **Formatting** — code must pass `dotnet-format --check`; run `make lint` (or `dotnet format`) to fix
  formatting locally before pushing. CI fails on formatting drift.
- **Branches and review** — work on a feature branch and open a pull request; changes are reviewed and
  approved before merging. The PR template asks for a description of the problem and solution, a review
  checklist and any follow-up actions.
- **`make restart-db`** — recreates the local `test-database` container after a schema change.

### Data migrations

A good data migration:

- Records failure logs.
- Is automated.
- Is reliable.
- Runs as close to real time as possible.
- Has observable monitoring in place.
- Does not affect any existing databases.

### CI/CD and deployment

Deployment is via the **Serverless Framework** from CircleCI ([`.circleci/config.yml`](.circleci/config.yml)).
This repository has **no Terraform** — the Lambda, its schedules and its IAM resources are declared in
[`serverless.yml`](HousingFinanceInterimApi/serverless.yml)'s `resources:` block. CI gates deployment on a
code-formatting check (`dotnet-format --check`) and the Docker test suite.

**Stages and triggers:**

- **Any branch** — formatting check and tests.
- **`development`** — after the checks pass, the function is deployed to **development**.
- **`master`** — tests, then deployment to **staging**, then (after the `permit-production-release` manual
  approval) deployment to **production**.

Note that the development deployment is driven by the `development` branch, and that this repository has **no
pre-production stage**.

**Packaging and runtime:**

- [`HousingFinanceInterimApi/build.sh`](HousingFinanceInterimApi/build.sh) installs `Amazon.Lambda.Tools` and
  runs
  `dotnet lambda package --configuration release --framework net10.0 --output-package ./bin/release/net10.0/housing-finance-interim-api.zip`.
  (`build.cmd` is the Windows equivalent for local use.)
- The deploy job runs `sls deploy --stage <stage>`.
- [`serverless.yml`](HousingFinanceInterimApi/serverless.yml) declares `runtime: dotnet10` and consumes that
  artifact; it declares the HTTP API plus the 25 scheduled functions and 5 step functions.

[docker-download]: https://www.docker.com/products/docker-desktop
[aws-cli]: https://aws.amazon.com/cli/
[session-manager-install]: https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html
[host-networking-prereq]: https://docs.docker.com/network/network-tutorial-host/#prerequisites
