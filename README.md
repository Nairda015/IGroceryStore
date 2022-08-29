# IGroceryStore
[![.NET](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml)
### Loosely coupled monolith app where users will track pesronal basket price.

# Projects Structure:  
![IGroceryStore-Diagram](https://user-images.githubusercontent.com/44712992/184506690-066939f2-64c3-42c7-8be0-05f27fbc640e.png)

# Modules:
### [Users.Core](https://github.com/Nairda015/IGroceryStore/tree/master/src/Users/Users.Core)
[Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Users/Users.Core/Features)
- Register
- Login
- Refresh Token
- GetAll/GetUser

### [Products.Core](https://github.com/Nairda015/IGroceryStore/tree/master/src/Products/Products.Core)
[Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Products/Products.Core/Features)
- Create Product
- Mark Product As Obsolate
- Add Alergens To Product
- CRUD for Categories
- CRUD for Alergens

### [Basket.Core](https://github.com/Nairda015/IGroceryStore/tree/master/src/Baskets/Baskets.Core)
[Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Baskets/Baskets.Core/Features)
- Create Basket  

### [Shops.Core](https://github.com/Nairda015/IGroceryStore/tree/master/src/Shops/Shops.Core)
[Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Shops/Shops.Core/Features)


# Stack  
[.Net7.0 C#11 with Minimal API](https://github.com/dotnet)  
[Docker](https://github.com/docker)  
[AWS Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration/) - runtime configuration  
[JWT.Net](https://github.com/jwt-dotnet/jwt) - authorisation and authentication  
[BCrypt.net](https://github.com/BcryptNet/bcrypt.net) - password hasing  
[GitHubAction](https://github.com/Nairda015/IGroceryStore/blob/master/.github/workflows/dotnet.yml) - CI/CD  
[RabbitMQ](https://github.com/rabbitmq) - asynchronous messaging  
[PostgreSQL](https://github.com/postgres/postgres) - database with json support  
[Entity Framework Core](https://github.com/dotnet/efcore) - ORM  
[OpenTelemetry](https://opentelemetry.io) - Traces, Metrics  
[Jaeger](https://www.jaegertracing.io) - UI for OpenTelemetry  
<img width="1511" alt="image" src="https://user-images.githubusercontent.com/44712992/187305398-143d94e0-4b33-4667-b367-aae2c9789a2d.png">


# Main features:  
- Comparing basket price across stores  
- Searching for alergens free products  
- Rating products and stores  
- Searching for similar products  
- Watching product prices over time

# How to run
- From the tools directory run the command:  
```docker-compose up -d```  
- Run ASP and Worker Project (add compound configuration in Rider)
- API endpoints: localhost:5000/swagger
- All ports for infrastructure available in docker-compose file
- To connect to pgadmin from docker use:  
```docker run -p 5050:80  -e "PGADMIN_DEFAULT_EMAIL=name@example.com" -e "PGADMIN_DEFAULT_PASSWORD=admin"  -d dpage/pgadmin4```  
