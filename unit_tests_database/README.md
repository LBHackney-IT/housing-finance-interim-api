# What:
It's dockerised PostgreSQL database used for the HFS database object (views/functions/procedures) unit tests.

# Why:
We want a clean database, where we can mess with it's state worry-free.

# Assumptions
1. This database expects the `tables`, `views`, `functions`, and `stored_procedures` directories to be on the root `/` of this repository.
2. This database expects the `snake_case_named.sql` scripts to be within the corresponding folders' `/postgres` subdirectory. This is expected to change once we're done with the migration & can replace the MSSQL scripts with the PostgreSQL scripts.
3. Whenever any SQL script depends on another to be created _(e.g. `batch_log_error.sql` references `batch_log.sql`)_, then that dependency order needs to be reflected in the `*.order` file, where `*` is your database_object_type _(like 'views.order')_.
4. Order file entries are expected to match the `.sql` script file names.
5. SQL scripts categories (by object type) will get executed in the following order: tables, functions, views, stored_procedures. To change that modify the `sql_order_control.sh` script. 

# To run the database
1. You need to have `docker` installed on your system.
2. You can't use `Powershell` as it poorly interprets some `Makefile` syntax.
3. run `make unit_db` from the root of the repository.

# Connect
1. To connect use your GUI client, or `psql` cli:
   ```sh
      psql -h localhost -p 5432 -U housingadmin -d mydatabase 
   ```
   You'll need to use the password specified within the `Dockerfile` `ENV` command.
2. To connect from within an application plainly create your PostgreSQL connection string.
