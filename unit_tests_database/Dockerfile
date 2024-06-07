FROM postgres:16-alpine

ENV POSTGRES_USER=housingadmin
ENV POSTGRES_PASSWORD=mypassword
ENV POSTGRES_DB=mydatabase

COPY tables/postgres/* ./docker-entrypoint-initdb.d/tables/
COPY functions/postgres/* ./docker-entrypoint-initdb.d/functions/
COPY views/postgres/* ./docker-entrypoint-initdb.d/views/
COPY stored_procedures/postgres/* ./docker-entrypoint-initdb.d/stored_procedures/

EXPOSE 5432
