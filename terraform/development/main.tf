# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  application_name = "housing-finance-interim-api" # The name to use for your application
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-housing-development"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/housing-finance-interim-api/state"
  }
}

resource "aws_db_subnet_group" "db_subnets" {
  arn         = "arn:aws:rds:eu-west-2:364864573329:subgrp:housing_finance_development-db-subnet-development"
  description = "Managed by Terraform"
  id          = "housing_finance_development-db-subnet-development"
  name        = "housing_finance_development-db-subnet-development"
  subnet_ids = [
    -"subnet-0140d06fb84fdb547",
    -"subnet-029aded4e4b739233",
    -"subnet-05ce390ba88c42bfd",
    -"subnet-0c522aafcb373a205",
  ]
}

data "aws_ssm_parameter" "sns_topic_arn" {
    name = "/housing-finance/${var.environment_name}/cloudwatch-alarms-topic-arn"
}

module "api-alarm" {
    source           = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/api-alarm"
    environment_name = var.environment_name
    api_name         = "housing-finance-interim-api"
    alarm_period     = "300"
    error_threshold  = "1"
    sns_topic_arn    = data.aws_ssm_parameter.sns_topic_arn.value
}
