# Wait for database to be ready
DBSTATUS=1
ERRCODE=1

while [ $ERRCODE -ne 0 ] || [ -z $DBSTATUS ] || [ $DBSTATUS -ne 0 ]; do
	echo "[db_setup] Waiting for database to start up..."
    DBSTATUS=$(/opt/mssql-tools/bin/sqlcmd -h -1 -t 1 -U sa -P $SA_PASSWORD -Q "SET NOCOUNT ON; Select SUM(state) from sys.databases")
	ERRCODE=$?
	sleep 1s
done

echo "[db_setup] Initialising database state..."

# Create database
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "CREATE DATABASE sow2b"

# Set to default
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -Q "EXEC sp_defaultdb 'sa', 'sow2b'"

# Create tables
for file in /usr/config/tables/*.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -i $file
done

# Create functions
for file in /usr/config/functions/*.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -i $file
done

# Create stored procedures
for file in /usr/config/stored_procedures/*.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -i $file
done

# Create views
for file in /usr/config/views/*.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -i $file
done
