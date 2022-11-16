terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.39"
    }
  }
}

provider "aws" {
  region = "eu-central-1"
  profile = "devops"
}

locals {
  environment = "test"
}