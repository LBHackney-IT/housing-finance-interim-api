.ONESHELL:

# Requires AWS CLI Profile matching housing-${ENVIRONMENT} to be set up
# Requires AWS Session Manager Plugin to be installed:
# 	https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html

# On WINDOWS OS you will need to run these commands using Git Bash, NOT Powershell / CMD

THIS_FILE := $(lastword $(MAKEFILE_LIST))
DOCKERFILE := ./$(shell dirname $(THIS_FILE))/Dockerfile

build:
	@docker build -t hfs-ee-tests-db -f $(DOCKERFILE) .

run-internal:
	@docker run --rm -it --name hfs-ee-tests-db-container -p 127.0.0.1:1433:1433 hfs-ee-tests-db

clean-images:
	@docker rmi $$(docker images --filter "dangling=true" -q --no-trunc)

launch:
	@$(MAKE) -f $(THIS_FILE) build
	@$(MAKE) -f $(THIS_FILE) run-internal
	@$(MAKE) -f $(THIS_FILE) clean-images

# Use if the container didn't stop
clean-containers:
	docker stop $$(docker ps -aq --filter "name=hfs-psql-tests-db-container")
