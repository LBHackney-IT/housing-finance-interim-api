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

# Insert seed data
for file in /usr/config/seed_data/*.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d sow2b -i $file
done
