# IGroceryStore
[![.NET](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml)
### Loosely coupled monolith app where users will track pesronal basket price.
Work in progress...

## Alredy working (API)
### [Users.Core](https://github.com/Nairda015/IGroceryStore/tree/master/src/Users/Users.Core)
Available actions:
1. Register
2. Login
3. Refresh Token
4. GetAll/GetUser

## Stack  
[.Net6.0 C#10 with Minimal API](https://github.com/dotnet)  
[Docker](https://github.com/docker)  
[AWS Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration/) - runtime configuration  
[JWT.Net](https://github.com/jwt-dotnet/jwt) - authorisation and authentication  
[BCrypt.net](https://github.com/BcryptNet/bcrypt.net) - password hasing  
[GitHubAction](https://github.com/Nairda015/IGroceryStore/blob/master/.github/workflows/dotnet.yml) - CI/CD  
[RabbitMQ](https://github.com/rabbitmq) - asynchronous messaging  
[PostgreSQL](https://github.com/postgres/postgres) - database with json support  
[Entity Framework Core](https://github.com/dotnet/efcore) - ORM  

## Main features will be:  
-Comparing basket price across stores  
-Searching for alergens free products  
-Rating products and stores  
-Searching for similar products  
-Watching product prices over time
