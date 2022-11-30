terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.39"
    }
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "1.17.1"
    }
  }
}

locals {
  environment = "dev"
  table_name  = "shops"
  region      = "eu-central-1"
  pg_db_name  = "IGroceryStoreDb"
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
}

provider "postgresql" {
  host             = "localhost"
  port             = 5432
  database         = "postgres"
  username         = "postgres"
  password         = "admin"
  connect_timeout  = 15
  sslmode          = "disable"
  expected_version = "14.4"
}

resource "postgresql_database" "postgres_db" {
  name              = local.pg_db_name
  owner             = "postgres"
  connection_limit  = -1
  allow_connections = true
}

resource "postgresql_schema" "postgres_products_schema" {
  database = local.pg_db_name
  name     = "IGroceryStoreDb.Products"
  owner    = "postgres"
  depends_on = [postgresql_database.postgres_db]
}

