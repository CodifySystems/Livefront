# Carton Caps Sample Web API Aplication

Sample application for the LiveFront Carton Caps coding challenge.
Implemented in ASP.NET Core for maxium compatibility and can be compiled and run on Windows, Linux and MacOS
Uses a simplified Clean Architecture [ref Uncle Bob](https://www.goodreads.com/book/show/18043011-clean-architecture)

## Projects
### CartonCaps.Application
- Provides Application Layer functionality
### CartonCaps.Domain
- Responsible for Domain Layer
  - Common classes
  - Domain Entities
  - Enums
### CartonCaps.Infrastructure
- Mock Infrastructure Layer
  - Provides repository objects and Entity Framework logic for persisting an In-Memory database
  - Also implements database seeding to allow for fully testable code while offline
### CartonCaps.WebAPI
- API Implementation

- Referrer: Account (Invite Friends via Share link)
  - Generates Referral Code and link to share
- Referee opens Link (First Run Experience)
  - Link tap/click opens app and registers with referral code
### CartonCaps.Tests
- Unit tests