# IGroceryStore
[![.NET](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Nairda015/IGroceryStore/actions/workflows/dotnet.yml)
### Loosely coupled monolith app where users will track personal basket price.

# Projects Structure:  
![IGroceryStore-Diagram](https://user-images.githubusercontent.com/44712992/184506690-066939f2-64c3-42c7-8be0-05f27fbc640e.png)

# Main features:
- Watching product prices over time
- Comparing basket price across stores
- Searching for allergens free products
- Rating products and stores
- Searching for similar products

# Modules:
## [Basket](https://github.com/Nairda015/IGroceryStore/tree/master/src/Baskets/Baskets.Core)
- Event Store for storing historical data about products prices and promotions
- MongoDb for storing projections and users baskets

### [Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Baskets/Baskets.Core/Features)
- Create Basket
- Consumers for product creation events and price changes events

## [Users](https://github.com/Nairda015/IGroceryStore/tree/master/src/Users/Users.Core)
- Auth with JWT
- Refresh tokens handling  (in future I'll move storing tokens to Redis or integrate project with OpenId)
- BCrypt for hashing passwords
- Postgres for storing users (I'm considering changing it to noSql - mongoDb, casandra or dynamodb)

### [Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Users/Users.Core/Features)
- Register
- Login
- Refresh Token
- GetAll/GetUser

## [Shops](https://github.com/Nairda015/IGroceryStore/tree/master/src/Shops/Shops.Core)
- Storing shops location and current price of product
- calculating best basket price in shops near me (coming soon)

### [Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Shops/Shops.Core/Features)
- Report new product price 

## [Products](https://github.com/Nairda015/IGroceryStore/tree/master/src/Products/Products.Core)
- Postgres for relational data
- Module publishes events for other modules about new products
- I'm considering using apache lucene to enable searching for similar products

### [Available actions:](https://github.com/Nairda015/IGroceryStore/tree/master/src/Products/Products.Core/Features)
- Create Product
- Mark Product As Obsolete
- Add Allergens To Product
- CRUD for Categories
- CRUD for Allergens

# Stack  
[.Net7.0 C#11 with Minimal API](https://github.com/dotnet)  
[Docker](https://github.com/docker) - containerization for tests aln local development  
[Terraform](https://www.terraform.io/) - for infrastructure as code (IaC)  
[Event Store](https://www.eventstore.com/) - storing historical data of products prices  
[MongoDb](https://www.mongodb.com/) - storing projections from historical data  
[DynamoDb](https://github.com/aws/aws-sdk-net) - storing data from shop module  
[AWS Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration/) - runtime configuration  
[JWT.Net](https://github.com/jwt-dotnet/jwt) - authorisation and authentication  
[BCrypt.net](https://github.com/BcryptNet/bcrypt.net) - password hashing  
[GitHubAction](https://github.com/Nairda015/IGroceryStore/blob/master/.github/workflows/dotnet.yml) - CI/CD  
[RabbitMQ](https://github.com/rabbitmq) - asynchronous messaging  
[MassTransit](https://masstransit-project.com) - asynchronous messaging library  
[PostgreSQL](https://github.com/postgres/postgres) - database with json support  
[Entity Framework Core](https://github.com/dotnet/efcore) - ORM used for product module  
[OpenTelemetry](https://opentelemetry.io) - Traces, Metrics  
[Jaeger](https://www.jaegertracing.io) - Exporter/UI for OpenTelemetry  
<img width="1511" alt="image" src="https://user-images.githubusercontent.com/44712992/189454184-7a044389-3dce-4e8e-ad8e-72675aaa09aa.png">

# For Contributors

## Git Branching Strategy
I'm using Feature Branch Workflow for simplicity and  
I encourage you to use it also in your fork.  
You can read more about flow [here](https://www.blog.techtious.com/tag/feature-branch-workflow/)

## Requirements
- docker
- terraform (optional)
- aws account (optional)

## How to run
- From the tools/docker directory run the command:  
```docker compose --profiles webapi,jaeger up -d``` (look at profiles section)
- if you want to run shop module you can create all required resources with terraform from tools/terraform/dev directory   
```terraform init```  
```terraform apply```
- Run ASP and/or Worker Project (add compound configuration in Rider)
![img](https://user-images.githubusercontent.com/44712992/202317883-be680353-4708-41c9-97c8-eda7c4105fdd.png)
- API endpoints: localhost:5000/swagger
- All ports for infrastructure available in docker-compose file
- To connect to pgadmin from docker use:  
```docker run -p 5050:80  -e "PGADMIN_DEFAULT_EMAIL=name@example.com" -e "PGADMIN_DEFAULT_PASSWORD=admin"  -d dpage/pgadmin4```

### docker compose profiles (optional):
- all - all containers
- webapi - all except observability
- shops - dynamodb
- users - postgres
- baskets - eventstore, mongodb
- products - postgres
- logs - elasticsearch, kibana
- kibana
- jaeger
- elastic

**rabbitmq and redis will run by default**
