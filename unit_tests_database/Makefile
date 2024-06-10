.ONESHELL:

# Do not run using Powershell, it doesn't interpret all syntax equally well!

THIS_FILE := $(lastword $(MAKEFILE_LIST))
DOCKERFILE := ./$(shell dirname $(THIS_FILE))/Dockerfile

build:
	@docker build -t hfs-psql-tests-db -f $(DOCKERFILE) .

run:
	@docker run --rm --name hfs-psql-tests-db-container -p 127.0.0.1:5432:5432 hfs-psql-tests-db

launch:
	@$(MAKE) -f $(THIS_FILE) build
	@$(MAKE) -f $(THIS_FILE) run
	@$(MAKE) -f $(THIS_FILE) clean-images

clean-images:
	@docker rmi $$(docker images --filter "dangling=true" -q --no-trunc)

# Use if the container didn't stop
clean-containers:
	docker stop $$(docker ps -aq --filter "name=hfs-psql-tests-db-container")
