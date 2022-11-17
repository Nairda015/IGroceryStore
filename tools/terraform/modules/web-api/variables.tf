
variable "ami-version" {
  description = "version of ami for eu-central-1"
  type = string
  default = "ami-0233214e13e500f77"
}

variable "ami-instance-type" {
  description = "instance type of ami"
  type = string
  default = "t2.micro"
}

variable "environment-name" {
  description = "environment [dev, test, prod]"
  type = string
  default = "dev"
}

variable "region" {
  description = "default region"
  type = string
  default = "eu-central-1"
}

variable "app-table-name-shops" {
  description = "name of the dynamodb table"
  type = string
  default = "Shops"
}