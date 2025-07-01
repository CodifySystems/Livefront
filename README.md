# Carton Caps Sample Web API Application

Sample application for the Livefront Carton Caps coding challenge.
Implemented in ASP.NET Core for maximum compatibility and can be compiled and run on Windows, Linux and MacOS.

Uses a simplified version of Clean Architecture (ref: [Robert 'Uncle Bob' Martin](https://www.goodreads.com/book/show/18043011-clean-architecture)).

This sample project uses Entity Framework to mock a simple database for use in testing the API, this would be replaced in a real project with a proper backend implemented via the Infrastructure project. Initial data is seeded by the [MockDbContext.cs](./src/CartonCaps.Infrastructure/MockDbContext.cs) which can be referenced for testing the API.

The application also assumes that the deferred deep links will be provided by a 3rd party service. This too is mocked out in the Application project in [DeepLinkService.cs](./src/CartonCaps.Application/Services/DeepLinkService.cs). In a fully production ready app, this would be further built out to include proper service references and shared secrets manager for authenticating into the 3rd party API and exchanging the necessary data to generate the shareable links.

Final assumption is that authentication and authorization will be handled by an existing API and passed via a token or shared secret. This example API uses no-auth to simplify testing for an unknown authentication provider.

## Prerequisites

[.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

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

## Deliverable #1 -  API Specification
The OpenAPI spec can be found in the project root folder [openapi_spec.yaml](./openapi_spec.yaml) 

Postman collection also available in the project root folder [CartonCaps.postman_collection.json](.CartonCaps.postman_collection.json) for testing or implementing the API. It also provides examples for the API methods based on the sample data included in the mocked database objects.

[Redocly HTML view of the API spec documentation](./redoc-static.html)

## Deliverable #2 - Mock API .NET Core Solution
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
  - Implements the repository interfaces as concrete classes for use by the WebAPI project
  - Provides mock EFCore repository objects and logic for persisting and retreiving entities to an In-Memory database
  - Also implements database seeding to allow for fully testable code without the need for a physical database
### CartonCaps.WebAPI
[./src/CartonCaps.WebAPI/CartonCaps.WebAPI.csproj](./src/CartonCaps.WebAPI/CartonCaps.WebAPI.csproj)
- Mock API Implementation
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

## Mock Data for API testing

### Users
| UserId | First Name | Last Name | Referral Code |
| ------ | ---------- | --------- | ------------- |
| 29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a | Alice | Bag| AL1C3B |
| b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6 | Kathleen | Hannah | KT5Y8B |
| c1d2e3f4-a5b6-7c8d-9e0f-1a2b3c4d5e6f | Debbie | Harry | DE3H4R |
| a80afede-b590-4c5a-a449-10d6c65d091c | Joan | Jett  | JO4J3T |
| fd9e60df-7b1c-41fa-8e96-ee561a7ee870 | Poly | Styrene  | PO9S7R |

### Referrals
| ReferralId | UserId | Status | Claimed by UserId | Claimed by User Display Name |
| ---------- | ------ | ------ | ----------------- | ---------------------------- |
| 714c572a-4ff7-4801-8684-2672ade84c1b | 29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a | InProgress  |
| 13d2920e-f5c4-4ee6-a97a-1ff50d55eda8 | 29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a | Completed | b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6 | Kathleen H. |
| 5c3f2acc-cabb-422d-89fa-d7d616ed382c | 29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a | Abandoned  |
| 32e765a3-7cb7-4872-a1fe-1cebbe313300 | b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6 | Completed | c1d2e3f4-a5b6-7c8d-9e0f-1a2b3c4d5e6f |Debbie H. |
| 37889b4f-6b59-46ef-87ac-4b5b9862414d | b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6 | InProgress  |
