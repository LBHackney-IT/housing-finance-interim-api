.ONESHELL:

# Requires AWS CLI Profile matching housing-${ENVIRONMENT} to be set up
# Requires AWS Session Manager Plugin to be installed:
# 	https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html
# On Windows you will need to run these commands using Git Bash, NOT Powershell / CMD

# -- Configuration --
# Local port to connect to on your machine
LOCAL_PORT = 1433
# For Parameter store URL Paths
ENVIRONMENT = development
# Set to AWSCLI Profile name
PROFILE = housing-${ENVIRONMENT}

# -- Parameter Store paths --
INSTANCE_ID_PATH := " /platform-apis-jump-box-instance-name"
DB_ENDPOINT_PATH := " /housing-finance/${ENVIRONMENT}/db-host"
REMOTE_PORT_PATH := " /housing-finance/${ENVIRONMENT}/db-port"
DB_USERNAME_PATH := " /housing-finance/${ENVIRONMENT}/db-username"
DB_PASSWORD_PATH := " /housing-finance/${ENVIRONMENT}/db-password"

# -- Parameters --
INSTANCE_ID := $(shell aws ssm get-parameter --name ${INSTANCE_ID_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
REMOTE_PORT := $(shell aws ssm get-parameter --name ${REMOTE_PORT_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_ENDPOINT := $(shell aws ssm get-parameter --name ${DB_ENDPOINT_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_USERNAME := $(shell aws ssm get-parameter --name ${DB_USERNAME_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_PASSWORD := $(shell aws ssm get-parameter --name ${DB_PASSWORD_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_NAME := "sow2b"

DATABASE_PARAMS := '{"host":["${DB_ENDPOINT}"], "portNumber":["${REMOTE_PORT}"], "localPortNumber":["${LOCAL_PORT}"]}'

# -- Commands --

# Renews your login with your AWS CLI SSO profile
sso_login:
	if (aws sts get-caller-identity --profile ${PROFILE})
	then
		echo "Session still valid"
	else
		echo "Session expired, logging in"
		aws sso login --profile ${PROFILE}
	fi

# Port forwards through the EC2 Jumpbox / Bastion to the housing finance database
port_forwarding_to_hfs_db:
	echo "Connecting to ${DB_ENDPOINT} on local port ${LOCAL_PORT}\nUsername: ${DB_USERNAME}\nPassword: ${DB_PASSWORD}"
	aws ssm start-session \
		--target ${INSTANCE_ID} \
		--region=eu-west-2  \
		--profile=${PROFILE} \
		--document AWS-StartPortForwardingSessionToRemoteHost \
		--parameters ${DATABASE_PARAMS}

# This generates the connection string for connecting to the database through the port forwarding above
# Set this in your .env file
# Replace host.docker.internal with localhost if you are on Linux
local_connection_string_to_env:
	echo "CONNECTION_STRING='Server=host.docker.internal,${LOCAL_PORT};Initial Catalog=${DB_NAME};User Id=${DB_USERNAME};Password=${DB_PASSWORD};Encrypt=False;TrustServerCertificate=False;MultipleActiveResultSets=True;'"
