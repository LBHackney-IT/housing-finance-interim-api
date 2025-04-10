.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build housing-finance-interim-api

.PHONY: serve
serve:
	docker-compose build housing-finance-interim-api && docker-compose up housing-finance-interim-api

.PHONY: shell
shell:
	docker-compose run housing-finance-interim-api bash


.PHONY: clean
clean:
	docker rm $$(docker ps -a --filter "status=exited" | grep housing-finance-interim-api-test | grep -oE "^[[:xdigit:]]+")
	docker rmi $$(docker images --filter "dangling=true" -q)

.PHONY: test
test:
	-docker-compose build housing-finance-interim-api-test && docker-compose run housing-finance-interim-api-test
	-make clean

test-db:
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
	docker-compose up -d test-database

.PHONY: serve-local
serve-local:
	dotnet run --project HousingFinanceInterimApi/HousingFinanceInterimApi.csproj

.PHONY: remote-db
remote_db:
	-make -f HFSDatabaseObjects/database/ee/Makefile sso_login;
	make -f HFSDatabaseObjects/database/ee/Makefile ee_db;

# Update HFSDatabaseObjects submodule
update_submodule:
	git submodule update --init --recursive

unit_db:
	@make -f HFSDatabaseObjects/database/unit_tests_database/Makefile launch

unit_db_ee:
	@make -f HFSDatabaseObjects/database/unit_tests_database_ee/Makefile launch