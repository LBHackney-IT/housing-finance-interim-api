FROM postgres:16-alpine

ENV POSTGRES_USER=housingadmin
ENV POSTGRES_PASSWORD=mypassword
ENV POSTGRES_DB=sow2b

COPY schemas/postgres/* ./docker-entrypoint-initdb.d/schemas/
COPY tables/postgres/* ./docker-entrypoint-initdb.d/tables/
COPY functions/postgres/* ./docker-entrypoint-initdb.d/functions/
COPY views/postgres/* ./docker-entrypoint-initdb.d/views/
COPY stored_procedures/postgres/* ./docker-entrypoint-initdb.d/stored_procedures/
COPY database/seed_data/* ./docker-entrypoint-initdb.d/seed_scripts/

COPY database/unit_tests_database/* ./docker-entrypoint-initdb.d/

RUN ./docker-entrypoint-initdb.d/sql_order_control.sh

EXPOSE 5432
