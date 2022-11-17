terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.39"
    }
  }
}

locals {
  environment = "dev"
  table_name  = "shops"
  region      = "eu-central-1"
}

provider "aws" {
  access_key                  = "mock_access_key"
  region                      = local.region
  secret_key                  = "mock_secret_key"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    dynamodb = "http://localhost:8000"
  }
}

resource "aws_dynamodb_table" "shops-table" {
  name           = local.table_name
  billing_mode   = "PROVISIONED"
  read_capacity  = 2
  write_capacity = 2
  hash_key       = "pk"
  range_key      = "sk"

  attribute {
    name = "pk"
    type = "S"
  }

  attribute {
    name = "sk"
    type = "S"
  }
  
  tags = {
    Name        = "${local.table_name}-${local.environment}"
    Environment = local.environment
  }
}

