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

.PHONY: remote_db
remote_db:
	make -f finance_database.mk port_forwarding_to_hfs_db
