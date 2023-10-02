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
ifeq (${ENVIRONMENT}, development)
	INSTANCE_ID_PATH := " /housing-finance/${ENVIRONMENT}/instance-id"
else
	INSTANCE_ID_PATH := " /platform-apis-jump-box-instance-name"
endif
GOOGLE_API_KEY_PATH := " /housing-finance/${ENVIRONMENT}/google-application-credentials-json"
WAIT_DURATION_PATH := " /housing-finance/${ENVIRONMENT}/step-function-wait-duration"
BATCH_SIZE_PATH := " /housing-finance/${ENVIRONMENT}/bulk-insert-batch-size"
CASH_FILE_REGEX_PATH := " /housing-finance/${ENVIRONMENT}/cash-file-regex"
HOUSING_FILE_REGEX_PATH := " /housing-finance/${ENVIRONMENT}/housing-benefit-file-regex"
ACCEPTED_ORIGINS_PATH := " /housing-finance/${ENVIRONMENT}/cors-accepted-origins"
CHARGES_BATCH_YEARS_PATH := " /housing-finance/${ENVIRONMENT}/charges-batch-years"
S3_BUCKET_NAME_PATH := " /housing-finance/${ENVIRONMENT}/s3-bucket-name"
S3_OBJECT_PREFIX_PATH := " /housing-finance/${ENVIRONMENT}/s3-object-prefix"
HOUSING_FINANCE_ALLOWED_GROUPS_PATH := " /housing-finance/${ENVIRONMENT}/housing-finance-allowed-groups"

# -- Parameters --
GOOGLE_API_KEY := $(shell aws ssm get-parameter --name ${GOOGLE_API_KEY_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
BATCH_SIZE := $(shell aws ssm get-parameter --name ${BATCH_SIZE_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
CASH_FILE_REGEX := $(shell aws ssm get-parameter --name ${CASH_FILE_REGEX_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
HOUSING_FILE_REGEX := $(shell aws ssm get-parameter --name ${HOUSING_FILE_REGEX_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
ACCEPTED_ORIGINS := $(shell aws ssm get-parameter --name ${ACCEPTED_ORIGINS_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
CHARGES_BATCH_YEARS := $(shell aws ssm get-parameter --name ${CHARGES_BATCH_YEARS_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
S3_BUCKET_NAME := $(shell aws ssm get-parameter --name ${S3_BUCKET_NAME_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
S3_OBJECT_PREFIX := $(shell aws ssm get-parameter --name ${S3_OBJECT_PREFIX_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
HOUSING_FINANCE_ALLOWED_GROUPS := $(shell aws ssm get-parameter --name ${HOUSING_FINANCE_ALLOWED_GROUPS_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)

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

trigger_lambda_on_AWS $(LAMBDA_FUNCTION_NAME):
	echo "Connecting to Lambda function ${LAMBDA_FUNCTION_NAME}"
	aws lambda invoke \
		--function-name housing-finance-interim-api-${ENVIRONMENT}-${LAMBDA_FUNCTION_NAME} out --log-type Tail
		--region eu-west-2 
		--profile ${PROFILE}
