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

On Windows:
- You will need to use Git Bash or Windows Subsystem for Linux to run the Make commands
- To run with Docker you will need Windows Subsystem for Linux v2 with Docker Engine installed on it (NOT Docker Desktop on base Windows - this [does not support the host networking driver](https://docs.docker.com/network/network-tutorial-host/#prerequisites))

On MacOS (Monterey - possibly others):
- You will need to [turn off AirPlay receiver](https://developer.apple.com/forums/thread/682332) from your sharing settings due to a port 5000 conflict

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
  - you can run `make -f finance_database.mk sso_login` to refresh your login and verify your profile
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

On Windows you'll need Docker Desktop running with WSL2 integration enabled

Replace the AWS CLI profile name and run this command to log in to AWS Elastic Container Registry and allow fetching the base Docker container:
```sh
aws ecr get-login-password --profile {profile}
```

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

## Architecture

### File Processing Pipeline

The diagram below shows how cash files and housing benefit files flow through the system (C4 Container view).

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

AddElementTag("lambda", $shape=RoundedBoxShape(), $bgColor="#FF9900", $fontColor="white", $borderColor="#CC7700", $legendText="AWS Lambda")
AddElementTag("sfn", $bgColor="#E87722", $fontColor="white", $borderColor="#B85500", $legendText="AWS Step Function")
AddRelTag("cashflow", $textColor="#1565C0", $lineColor="#1565C0", $legendText="Cash file flow")
AddRelTag("hbflow", $textColor="#2E7D32", $lineColor="#2E7D32", $legendText="Housing benefit flow")

title Housing Finance Interim API — Container View

Person_Ext(civica, "Civica Pay", "External payment processor")
Person_Ext(academy, "Academy System", "Deposits weekly HB files\ninto Google Drive")

System_Ext(gdriveCash, "Google Drive", "CashFile folder\n(handoff: check-cash-files writes,\nimportCashFile reads)")
System_Ext(gdriveAcademy, "Google Drive", "AcademyFileFolder\n(HB source files)")
System_Ext(gdriveHB, "Google Drive", "HousingBenefitFile folder\n(handoff: checkHousingFiles writes,\nimportHousingFile reads)")

System_Boundary(infra, "mtfh-finance-infrastructure  [Terraform]") {
  Container(s3, "S3 Bucket", "AWS S3", "Receives raw .dat cash files from Civica via SFTP (cashfiles/ prefix)")
}

System_Boundary(api, "housing-finance-interim-api  [Serverless Framework]") {

  Container(checkCash, "check-cash-files", "Lambda / C#", "Triggered by S3 ObjectCreated. Reformats filename, deposits CashFileYYYYMMDD.dat into Google Drive, renames S3 object to OK_*.", $tags="lambda")
  Container(cashSFN, "HFCashFileStateMachine", "AWS Step Functions", "Scheduled: daily 02:00 AM.\nImportCashFile → Wait → ImportCashFileTransactions", $tags="sfn")
  Container(importCash, "importCashFile", "Lambda / C#", "Picks up file from Google Drive. Validates filename, dedup check, bulk-inserts raw lines to UPCashDump, renames Drive file OK_*/NOK_*.", $tags="lambda")
  Container(cashTrans, "importCashFileTransactions", "Lambda / C#", "Reads raw lines from UPCashDump, writes normalised Transaction records.", $tags="lambda")

  Container(hbSFN, "HFHousingFileStateMachine", "AWS Step Functions", "Scheduled: Mondays 06:00 AM.\nCheckHousingFiles → Wait → ImportHousingFile → Wait → ImportHousingFileTransactions", $tags="sfn")
  Container(moveHB, "checkHousingFiles", "Lambda / C#", "Reads most recent Academy file (last 7 days), renames to HousingBenefitFile{nextMonday}.dat, copies to HB folder.", $tags="lambda")
  Container(importHB, "importHousingFile", "Lambda / C#", "Picks up file from Google Drive. Dedup check, bulk-inserts raw lines to UPHousingCashDump, renames Drive file OK_*/NOK_*.", $tags="lambda")
  Container(hbTrans, "importHousingFileTransactions", "Lambda / C#", "Reads raw lines from UPHousingCashDump, writes normalised Transaction records.", $tags="lambda")

  ContainerDb(cashDb, "UPCashDump tables", "SQL Server", "Handoff: importCashFile writes,\nimportCashFileTransactions reads")
  ContainerDb(hbDb, "UPHousingCashDump tables", "SQL Server", "Handoff: importHousingFile writes,\nimportHousingFileTransactions reads")
  ContainerDb(txDb, "Transaction", "SQL Server", "Normalised payment records\n(shared by both pipelines)")
}

' ── Cash file flow ──────────────────────────────────────────────
Rel_D(civica, s3, "Upload CashFileYYYYMMDD.dat", "SFTP / cross-account IAM", $tags="cashflow")
Rel_D(s3, checkCash, "s3:ObjectCreated* notification", "Terraform-wired", $tags="cashflow")
Rel_D(checkCash, gdriveCash, "1. Write: CashFileYYYYMMDD.dat", "Google Drive API", $tags="cashflow")
Rel(checkCash, s3, "Rename to OK_* + delete original", "AWS S3 API", $tags="cashflow")
Rel_D(cashSFN, importCash, "Invoke (02:00 AM)", "Step Functions", $tags="cashflow")
Rel_Back(importCash, gdriveCash, "2. Read: CashFileYYYYMMDD.dat", "Google Drive API", $tags="cashflow")
Rel(importCash, gdriveCash, "3. Rename to OK_*/NOK_*", "Google Drive API", $tags="cashflow")
Rel_D(importCash, cashDb, "4. Write: raw lines", "SQL Server", $tags="cashflow")
Rel_D(cashSFN, cashTrans, "Invoke (after Wait)", "Step Functions", $tags="cashflow")
Rel_Back(cashTrans, cashDb, "5. Read: raw lines", "SQL Server", $tags="cashflow")
Rel_D(cashTrans, txDb, "6. Write: Transaction records", "SQL Server", $tags="cashflow")

' ── Housing benefit flow ────────────────────────────────────────
Rel_D(academy, gdriveAcademy, "Deposit weekly file", "Google Drive", $tags="hbflow")
Rel_D(hbSFN, moveHB, "Invoke (06:00 AM Mon)", "Step Functions", $tags="hbflow")
Rel_Back(moveHB, gdriveAcademy, "1. Read: most recent Academy file", "Google Drive API", $tags="hbflow")
Rel_D(moveHB, gdriveHB, "2. Write: HousingBenefitFile{nextMon}.dat", "Google Drive API", $tags="hbflow")
Rel_D(hbSFN, importHB, "Invoke (after Wait)", "Step Functions", $tags="hbflow")
Rel_Back(importHB, gdriveHB, "3. Read: HousingBenefitFile{nextMon}.dat", "Google Drive API", $tags="hbflow")
Rel(importHB, gdriveHB, "4. Rename to OK_*/NOK_*", "Google Drive API", $tags="hbflow")
Rel_D(importHB, hbDb, "5. Write: raw lines", "SQL Server", $tags="hbflow")
Rel_D(hbSFN, hbTrans, "Invoke (after Wait)", "Step Functions", $tags="hbflow")
Rel_Back(hbTrans, hbDb, "6. Read: raw lines", "SQL Server", $tags="hbflow")
Rel_D(hbTrans, txDb, "7. Write: Transaction records", "SQL Server", $tags="hbflow")

SHOW_LEGEND()
@enduml
```

## Data Migrations
### A good data migration
- Record failure logs
- Automated
- Reliable
- As close to real time as possible
- Observable monitoring in place
- Should not affect any existing databases

[docker-download]: https://www.docker.com/products/docker-desktop
[docker-compose-download]: https://docs.docker.com/compose/install/
[universal-housing-simulator]: https://github.com/LBHackney-IT/lbh-universal-housing-simulator
[aws-cli]: https://aws.amazon.com/cli/
[session-manager-install]: https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html

