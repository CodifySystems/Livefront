# Carton Caps Sample Web API Aplication

Sample application for the LiveFront Carton Caps coding challenge.
Implemented in ASP.NET Core for maxium compatibility and can be compiled and run on Windows, Linux and MacOS
Uses a simplified version of Clean Architecture (ref: [Robert 'Uncle Bob' Martin](https://www.goodreads.com/book/show/18043011-clean-architecture)).

This sample project uses Entity Framework to mock a simple database for use in testing the API, this would be replaced in a real project with the proper backend implemented via the Infrastructure project.

The application also assume the deferred deep links will be provided with a 3rd part service. This too is mocked out in the Application project under /Services. In a fully production ready app, this would be further built out to include proper service references and shared secrets manager for authenticating into the 3rd party API and exchanging the necessary data to generate the shareable links.

Final assumption is that authentication and authorization will be handled by an existing API and passed via a token or shared secret. This example API uses no-auth to simplify testing for an unknown authentication provider.

## Quick Start
To quickly get up and running, clone repo and run the CartonCaps.WebAPI project

Open up a new terminal and change to the directory you wish to run the application
```shell
git clone https://github.com/CodifySystems/Livefront.git
cd ./src/CartonCaps.WebAPI
dotnet run
```
Once the application is up and running, you can test the API via Swagger [http://localhost:5155/swagger](http://localhost:5155/swagger/)

## Unit Tests
To run the unit test open up a new terminal where you have the repo cloned
```shell
cd ./tests
dotnet test
```
Sit back and watch the tests run

If you would like to run the tests with code coverage use the following command instead:

`dotnet test /p:CollectCoverage=true`

## OpenAPI Specification
The OpenApi spec can be found in the project root folder [openapi_spec.json](./openapi_spec.json) and can be used with Postman or any other tool for testing or implementing the API. It also provides examples for the API methods based on the sample data included in the mocked database objects.

## Projects
### CartonCaps.Application
[./src/CartonCaps.WebAPI/CartonCaps.Application.csproj](./src/CartonCaps.Application/CartonCaps.Application.csproj)
- Provides Application Layer functionality
- Defines custom exception types
- Interfaces for the repositories
- External service references
### CartonCaps.Domain
[./src/CartonCaps.Domain/CartonCaps.Domain.csproj](./src/CartonCaps.Domain/CartonCaps.Domain.csproj)
- Responsible for Domain Layer
  - Common classes
  - Domain Entities
  - Enums
### CartonCaps.Infrastructure
[./src/CartonCaps.Infrastructure/CartonCaps.Infrastructure.csproj](./src/CartonCaps.Infrastructure/CartonCaps.Infrastructure.csproj)
- Mock Infrastructure Layer
  - Implements repository interfaces as concrete classes for use by the WebAPI project
  - Provides mock EFCore repository objects logic for persisting and retreiving data objects to an In-Memory database
  - Also implements database seeding to allow for fully testable code without the need for a physical database
### CartonCaps.WebAPI
[./src/CartonCaps.WebAPI/CartonCaps.WebAPI.csproj](./src/CartonCaps.WebAPI/CartonCaps.WebAPI.csproj)
- API Implementation
  - ASP.NET Core Web API project for handling referral management.
  - Swagger UI implementation also available at [http://localhost:5155/swagger/index.html](http://localhost:5155/swagger/index.html)
    - `cd ./src/CartonCaps.WebAPI/`
    - `dotnet run`
  - HTTP test file can also be used for testing once app is running [./src/CartonCaps.WebAPI/CartonCaps.WebAPI.http](./src/CartonCaps.WebAPI/CartonCaps.WebAPI.http)

### CartonCaps API Methods
| Method | Route | Description |
| :-----: | ------- | --------- |
| GET | /api/status | Gets the server status |
| GET | /api/referral/statuses | Gets a list of possible status values for referrals |
| GET | /api/referral/{userId} | Gets a list of referrals for the specified user |
| POST | /api/referral/{userId} | Creates a new referral for the specified user and returns a shareable link |
| PATCH | /api/referral/{referralId}/status/{status} | Updates the status of an existing referral |
| POST | /api/referral/{referralId}/claim/{userId} | Claims a referral for the specified new user |

### CartonCaps.Tests
[./tests/CartonCaps.Tests.csproj](./tests/CartonCaps.Tests.csproj)
- Unit tests
  - Repository tests for Referrals and Users
  - WebAPI Controller tests for all API methods
  
Tests can be run within Visual Studio or via the dotnet CLI in the ./tests folder

`dotnet test` 

*or with code coverage*

`dotnet test /p:CollectCoverage=true`
