FROM mcr.microsoft.com/mssql/server:2019-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=password123!

# Create a config directory
RUN mkdir -p /usr/config
WORKDIR /usr/config

# Move SQL
COPY tables/ /usr/config/tables
COPY functions/ /usr/config/functions
COPY stored_procedures/ /usr/config/stored_procedures
COPY views/ /usr/config/views
COPY tests/test_data/ /usr/config/test_data

# Move initialiser scripts
COPY database/unit_tests_database_ee/db_setup.sh /usr/config/db_setup.sh
COPY database/unit_tests_database_ee/entrypoint.sh /usr/config/entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]
