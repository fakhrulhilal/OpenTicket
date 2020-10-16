# OpenTicket

PoC (Proof of Concept) for converting email into support ticket. It's not fully working app. This app is made to demonstrate integration for Microsoft 365 email apps. This app is built as simple as possible for learning purpose, not for production use.

## Technology stack

This PoC app uses several libraries to make the code cleaner and simpler. One of the goal is using as little code as possible, make it more readable, maintainable, more SOLID. Some notable used libraries:

1. [MediatR](https://github.com/jbogard/MediatR): for CQRS pattern, makes the domain layer simpler and less code for the caller
2. [Automapper](https://automapper.org/): for mapping between domain & entity model and vice versa
3. [EF Core](https://docs.microsoft.com/en-us/ef/core/): fast development for DB layer, because it implements unit of work pattern by default
4. [FluentValidator](https://fluentvalidation.net/): used for domain layer validation, keep it separate for actual domain layer implementation
5. [MailKit](https://github.com/jstedfast/MailKit): for fetching email, it supports built-in OAuth2 authentication
6. [Hangfire](https://www.hangfire.io/): background job manager, used for core task for downloading emails and converts them into ticket

## Requirements

Building the project as normal .NET project will do. But registering the email account, we need app key for Microsoft 365 (M365). This app uses [OAuth2 code flow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow) for implementation. To get it, sign in to [Azure portal](https://portal.azure.com). Navigate to _Azure Active Directory \ App registrations_ and click _+ New registration_.