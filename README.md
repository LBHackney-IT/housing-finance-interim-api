# Housing Finance Interim API

Housing Finance API will be used to serve data for the interim housing finance solution

## Stack

- .NET Core as a web framework.
- xUnit as a test framework.

## Development

### Requirements

1. [Docker][docker-download]
2. [Docker-Compose][docker-compose-download] (often installed automatically with Docker)
3. A recent version of [AWS CLI V2][aws-cli]
4. An AWS CLI profile for an environment the Finance DB is deployed in
5. A recent version of the [Session Manager Plugin][session-manager-install] for the AWS CLI
6. On Windows you will need to use Git Bash or WSL to run the Make commands
7. On Windows to run with Docker you will need Windows Subsystem for Linux v2 with Docker Engine installed on it (NOT Docker Desktop on base Windows - this [does not support the DB connection](https://docs.docker.com/network/network-tutorial-host/#prerequisites))

### Env variable setup
- See the [Serverless Configuration](HousingFinanceInterimApi/serverless.yml) under environment
for the environment variables to set from parameter store

- Copy `.env.sample` into the same directory and rename it to `.env`, then set the values
- You can use the Makefile from the port forwarding step below to help with generating the `CONNECTION_STRING` env variable
- Pay particular attention to the `GOOGLE_API_KEY` variable, which must be JSON wrapped in single quotes and on a single line

### Port forwarding to the finance DB
This is currently required in order to connect the API to a functional DB locally

#### Steps
- Open the [finance_database.mk](finance_database.mk) Makefile with the port forwarding commands

- There is a helper method for generating the connection string to the port forwarded db for a given AWS profile.
Run this then copy the output into your `.env` file:
```sh
make -f finance_database.mk local_connection_string_to_env
````

- Ensure you have a corresponding AWS CLI profile (default `housing-{stage}`) that matches the `PROFILE` variable
with the same AWS Profile that the credentials were sourced from.
- Edit the file to use the correct stage (development / staging / production)
- If you're using an SSO profile:
  - you can run `make -f finance_database.mk sso-login` to refresh your login and verify your profile
- Start the port forwarding with:
```sh
make -f finance_database.mk port_forwarding_to_hfs_db
````

#### Connecting to the database with a local client (optional)
If you want to connect the database through a graphical / other local client:
- Connect to localhost or 192.0.0.1 at port 1433
- Enter the username and password printed to the console after the port forwarding

### Running with Docker
Docker Compose will read the .env file in the root directory ( same directory as the `.env.sample` file), and will connect to the port forwarded database on localhost:1433

Run these Make commands from the root directory to trigger the docker compose build and up steps:
```sh
make build && make serve
```

### Running locally without Docker
The application will load an .env file in the root directory ( same directory as the `.env.sample` file) and will connect to the port forwarded database on localhost:1433

Run this command from the root directory to start the application:

```sh
dotnet run --project HousingFinanceInterimApi/HousingFinanceInterimApi.csproj
```

You can also configure your IDE of choice to run the `HousingFinanceInterimApi` project.


## Testing

### Run the tests

```sh
$ make test
```

To run database tests locally without Docker (e.g. via Visual Studio) the `CONNECTION_STRING` environment variable will need to be populated with:

`Host=localhost;Database=testdb;Username=postgres;Password=mypassword"`

Note: The Host name needs to be the name of the stub database docker-compose service, in order to run tests via Docker.

You will need to have the stub database running in order to run the tests outside of Docker

If changes to the database schema are made then the docker image for the database will have to be removed and recreated. The restart-db make command will do this for you.

### Agreed Testing Approach
- Use xUnit, FluentAssertions and Moq
- Always follow a TDD approach
- Tests should be independent of each other
- Gateway tests should interact with a real test instance of the database
- Test coverage should never go down
- All use cases should be covered by E2E tests
- Optimise when test run speed starts to hinder development
- Unit tests and E2E tests should run in CI
- Test database schemas should match up with production database schema
- Have integration tests which test from the PostgreSQL database to API Gateway

### Release process

We use a pull request workflow, where changes are made on a branch and approved by one or more other maintainers before the developer can merge into `master` branch.

![Circle CI Workflow Example](docs/circle_ci_workflow.png)

Then we have an automated six step deployment process, which runs in CircleCI.

1. Automated tests (xUnit) are run to ensure the release is of good quality.
2. The application is deployed to development automatically, where we check our latest changes work well.
3. We manually confirm a staging deployment in the CircleCI workflow once we're happy with our changes in development.
4. The application is deployed to staging.
5. We manually confirm a production deployment in the CircleCI workflow once we're happy with our changes in staging.
6. The application is deployed to production.

Our staging and production environments are hosted by AWS. We would deploy to production per each feature/config merged into  `master`  branch.

### Creating A PR

To help with making changes to code easier to understand when being reviewed, we've added a PR template.
When a new PR is created on a repo that uses this API template, the PR template will automatically fill in the `Open a pull request` description textbox.
The PR author can edit and change the PR description using the template as a guide.

## Static Code Analysis

### Using [FxCop Analysers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.FxCopAnalyzers)

FxCop runs code analysis when the Solution is built.

Both the API and Test projects have been set up to **treat all warnings from the code analysis as errors** and therefore, fail the build.

However, we can select which errors to suppress by setting the severity of the responsible rule to none, e.g `dotnet_analyzer_diagnostic.<Category-or-RuleId>.severity = none`, within the `.editorconfig` file.
Documentation on how to do this can be found [here](https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019).

`Host=localhost;Database=testdb;Username=postgres;Password=mypassword"`

Note: The Host name needs to be the name of the stub database docker-compose service, in order to run tests via Docker.

If changes to the database schema are made then the docker image for the database will have to be removed and recreated. The restart-db make command will do this for you.

### Agreed Testing Approach
- Use nUnit, FluentAssertions and Moq
- Always follow a TDD approach
- Tests should be independent of each other
- Gateway tests should interact with a real test instance of the database
- Test coverage should never go down
- All use cases should be covered by E2E tests
- Optimise when test run speed starts to hinder development
- Unit tests and E2E tests should run in CI
- Test database schemas should match up with production database schema
- Have integration tests which test from the PostgreSQL database to API Gateway

## Data Migrations
### A good data migration
- Record failure logs
- Automated
- Reliable
- As close to real time as possible
- Observable monitoring in place
- Should not affect any existing databases

## Contacts

### Active Maintainers

- **Faisal Gazi**, Lead Software Engineer at London Borough of Hackney (faisal.gazi@hackney.gov.uk)
- **Humairaa Mulla**, Senior Software Engineer at London Borough of Hackney (humairaa.mulla@hackney.gov.uk)
- **George Schena**, Senior Software Engineer at London Borough of Hackney (george.schena@hackney.gov.uk)

### Other Contacts

- **Selwyn Preston**, Head of Engineering at London Borough of Hackney (selwyn.preston@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[docker-compose-download]: https://docs.docker.com/compose/install/
[universal-housing-simulator]: https://github.com/LBHackney-IT/lbh-universal-housing-simulator
[aws-cli]: https://aws.amazon.com/cli/
[session-manager-install]: https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html

