.ONESHELL:

# Requires AWS CLI Profile matching housing-${ENVIRONMENT} to be set up
# Requires AWS Session Manager Plugin to be installed:
# 	https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html
# On Windows you will need to run these commands using Git Bash, NOT Powershell / CMD

# -- Configuration --
# Local port to connect to on your machine
LOCAL_PORT = 5432
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
DB_DATABASE_PATH := " /housing-finance/${ENVIRONMENT}/hfs-postgres-database"

# -- Secrets Manager paths --
DB_CREDENTIALS_PATH := " /housing-finance/${ENVIRONMENT}/master-db-credentials"

# -- Parameters --

INSTANCE_ID := $(shell aws ssm get-parameter --name ${INSTANCE_ID_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_USERNAME := $(shell aws ssm get-parameter --name ${DB_USERNAME_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_PASSWORD := $(shell aws ssm get-parameter --name ${DB_PASSWORD_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_DATABASE := $(shell aws ssm get-parameter --name ${DB_DATABASE_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_ENDPOINT := housing-finance-master-db-development.cqqi6ipffe40.eu-west-2.rds.amazonaws.com
REMOTE_PORT := 5432

DATABASE_PARAMS := '{"host":["${DB_ENDPOINT}"], "portNumber":["${REMOTE_PORT}"], "localPortNumber":["${LOCAL_PORT}"]}'

# -- Commands --

# Login to the AWS SSO service to be able to use the ssm commands
sso_login:
	if (aws sts get-caller-identity --profile ${PROFILE})
	then
		echo "Session still valid"
	else
		echo "Session expired, logging in"
		aws sso login --profile ${PROFILE}
	fi

# Connect to the Postgres database
pg_db:
	echo "Connecting to ${DB_ENDPOINT}\n local port ${LOCAL_PORT}\n Username: ${DB_USERNAME}\n Password: ${DB_PASSWORD}\n Database: ${DB_DATABASE}"
	aws ssm start-session \
		--target ${INSTANCE_ID} \
		--region=eu-west-2  \
		--profile=${PROFILE} \
		--document AWS-StartPortForwardingSessionToRemoteHost \
		--parameters ${DATABASE_PARAMS}

reset:
	fuser -n tcp -k ${LOCAL_PORT} && echo "Port ${LOCAL_PORT} cleared" || echo "Port ${LOCAL_PORT} not in use"
