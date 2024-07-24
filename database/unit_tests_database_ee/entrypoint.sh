#!/bin/bash

# Start the script to create the DB and user
/usr/config/db_setup.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr
