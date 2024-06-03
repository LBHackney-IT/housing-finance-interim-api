define setup_env
    $(eval ENV_FILE := .env)
    $(eval include .env)
    $(eval export)
endef

.PHONY: setup
setup:
	docker compose build

.PHONY: build
build:
	docker compose build housing-finance-interim-api

.PHONY: serve
serve:
	docker compose build housing-finance-interim-api && docker compose up housing-finance-interim-api

.PHONY: shell
shell:
	docker compose run housing-finance-interim-api bash


.PHONY: clean
clean:
	docker rm $$(docker ps -a --filter "status=exited" | grep housing-finance-interim-api-test | grep -oE "^[[:xdigit:]]+")
	docker rmi $$(docker images --filter "dangling=true" -q)

.PHONY: test
test:
	-docker compose build housing-finance-interim-api-test && docker compose run housing-finance-interim-api-test
	-make clean

.PHONY: test-db
test-db:
	# Ensure you have the CONNECTION_STRING env var set correctly in an .env file and an AWS profile accessed in the finance_database.mk
	make -f finance_database.mk sso_login ENVIRONMENT=development;
	make -f finance_database.mk port_forwarding_to_hfs_db ENVIRONMENT=development & \
	$(call setup_env)
	dotnet test --filter FullyQualifiedName~Infrastructure.DatabaseContext

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker compose up -d test-database

.PHONY: serve-local
serve-local:
	dotnet run --project HousingFinanceInterimApi/HousingFinanceInterimApi.csproj

.PHONY: remote-db
remote-db:
	-make -f finance_database.mk sso_login PROFILE=housing-development;
	make -f finance_database.mk port_forwarding_to_hfs_db PROFILE=housing-development;
