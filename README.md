# Setup

<strong>ON WINDOWS: You must use Git Bash or WSL to run the Make commands due to syntax</strong>

## AWS Hosted Databases:
Can be launched from **the root of the repository via its Makefile commands**. To do so:
- Edit the `PROFILE` variable in the Makefile to match your AWS CLI profile name
- Refresh your AWS CLI session with `aws sso login` or `make sso_login` if needed

- Port forward to the EE database using the Makefile command `make ee_db` (see comments in database/ee/Makefile for details).
- Either:
    - Run `make unit_db` to spin up a local Postgres database using Docker and ensure the `POSTGRES_LOCAL_MODE` is True in the `conftest.py` file. See further details about how this works [here](unit_tests_database/README.md)
    - Port forward to the Postgres database using the Makefile command `make pg_db` (see comments in database/posgres/Makefile for details) and ensure the `POSTGRES_LOCAL_MODE` is False in the `conftest.py` file
- While connected to both databases in separate processes, run `make test` to run the tests
You can use a test runner extension in VSCode or other IDE. With the Pytest CLI you can pass the `-k` flag to run tests matching string patterns, e.g. `pytest -k charges`.

NOTE: The CLI profile in the Makefile must be the same one Pytest uses to connect to the database as in the `make test` command.

# Context

## What:
This directory contains the setup scripts for 3 kinds of databases:
1. The MSSQL Enterprise Edition database hosted on AWS RDS _(one we're looking to migrate away from)_.
2. The PostgreSQL database hosted on AWS RDS _(migration target)_.
3. The PostgreSQL database hosted locally as a docker container.

The first two databases _(1. and 2.)_ are hosted on the AWS cloud and need to be connected to via port forwarding.
The last one (3.) is meant to be built, run _(and seeded if when needed)_ locally on any given system that has this repository cloned.

## Why:
What each database is expected to be used for?
1. MSSQL EE - It's used for developing the unit _(and later state snapshot)_ automated tests for the views/functions/stored procedures database objects. It acts as the source of truth for behaviour correctness.
2. PSQL - It's used for testing the migrated versions of the previously mentioned database objects. Given this is an AWS RDS hosted database with loads of data on it, the kinds of tests expected to be ran against it are primarily the snapshot tests. That way the outputs on EE and PSQL can be compared for sameness.
3. PSQL docker - Unlike the previous 2 databases, this one is not migrations focused _(whilst can be used for that)_. This one serves a long-term vision of having Unit Tests run against it that will give developers feedback on our feature and maintenance related changes to the database objects.
