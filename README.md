# Developer Evaluation Project - Sales Management API

This project involves building a robust Sales Management API that performs CRUD operations while adhering to Domain-Driven Design (DDD) principles. The API is designed to manage sales transactions, calculate discounts dynamically based on business rules, and handle event-driven communication using RabbitMQ.
The implementation leverages modern technologies like .NET 8, Entity Framework Core, and MediatR for request handling, ensuring scalability and maintainability. Containerized with Docker Compose, the solution is easily deployable and offers seamless integration with databases and message brokers.
# Core Features
**Sales Management**
- Record and update sales transactions with details like:
- Customer information
- Branch information
- Date
- Products sold, including quantity, unit price, and discounts
- Cancellation

# Discount Logic #
**Automatically calculate discounts based on quantity thresholds:**
 - 4–9 items: 10% discount
 - 10–20 items: 20% discount
 - No sales allowed above 20 items of the same product.
 - Purchases below 4 items are not eligible for discounts.

# Event-Driven Design #
**Publish events to RabbitMQ for system-wide notifications:**
- SaleCreatedEvent
- SaleModifiedEvent
- SaleCancelledEvent
- SaleItemCancelledEvent

**Event publishing is modular and can be adapted for external integration or logging.**

# API Documentation And Postman Collection #
- Comprehensive RESTful endpoints with clear documentation generated via Swagger/OpenAPI.
- This collection contains the requests for the API.
- *File: [Postman](./doc/AmbevPostmanCollection.json)*

# Error Handling #
**Unified error response structure for:**
- Validation errors
- Missing resources
- General system exceptions

# Technical Stack #
**The project uses the following tools and frameworks:**
- **Backend**: .NET 8
- **Database**: PostgreSQL
- **Messaging**: RabbitMQ
- **Containerization**: Docker Compose
- **Documentation**: Swagger/OpenAPI

# Technologies Used
- **.NET 8 Web API:** For building the API.
- **PostgreSQL:** Relational database for data storage.
- **RabbitMQ:** Messaging system for service communication.
- **Swagger:** API documentation and versioning.
- **Microsoft Extensions:** For application configuration and extension.
- **MediatR:** Facilitates business logic handling and ensures a decoupled architecture.
- **AutoMapper:** Simplifies object-to-object mapping for improved code readability.
- **xUnit:** Provides a robust framework for unit testing.
- **Serialog:** A library that provides efficient logging and diagnostic tools for .NET applications, helping developers track and analyze system behavior.
- **Docker Compose:** Enables easy setup and management of a containerized environment.
- **Testcontainers:** Used for integration testing, specifically configuring a PostgreSQL container for SaleRepositoryTest to validate repository behavior in an isolated database environment.

# Note Environments #
**DB_CONNECTION_STRING:** A variable configured to be used both in Docker Compose and when running through Visual Studio. The connection string is defined in the docker-compose.yml and launchSettings.json files to ensure consistency across environments.

# Docker Installation and Clone #
- **Make sure Docker is installed on your machine.**
You can download and install Docker [here](https://www.docker.com/get-started).
- **Clone the repository:**
git clone https://github.com/henriqueandradesilva/ambev.git

# Setup and Execution (Command Prompt Only)#
**To run the project from the terminal, follow the steps below:**
1. **Open the terminal and navigate to the project directory**
Make sure you're in the root directory where the .sln file and docker-compose.yml file are located. Example: **cd /path/to/project**
2. **Build the project using dotnet build**
Before running the containers, build the solution to ensure there are no errors in the code. 
Example: **dotnet build**
3. **Run Docker Compose**
Use the following command to start the Docker environment. 
Example: **docker-compose up -d**
4. **Access the Swagger documentation:**
Open your browser and go to: https://localhost:7181/swagger/index.html.

# Setup and Execution (With Visual Studio) #
1. **Open the solution in Visual Studio:**
Locate and open the file Ambev.DeveloperEvaluation.sln.
2. **Set the docker-compose project as the startup project:**
Right-click on the **docker-compose** project in the Solution Explorer and select "Set as Startup Project."
3. **Run the application:**
Press F5 or click "Start" to run the application.
4. **Access the Swagger documentation:**
Open your browser and go to: https://localhost:7181/swagger/index.html.

## Entity Relationship Diagram
![Entity Relationship Diagram](./doc/der.png)

# Tests
**Unit Test:** A set of automated tests that verify the behavior of specific units of the application (such as functions or methods) to ensure they work correctly in isolation.

# Tests Notes #
- Testcontainers: Only the SaleRepository is using Testcontainers, so it will be necessary to have Docker installed on your machine to run the tests that depend on it.

## Test Explorer
![Test Explorer](./doc/tests.png)

# Future Enhancements #
- In the future, it would be beneficial to extend the system's capabilities by integrating additional features, such as a shopping cart and other essential components that enhance the overall sales experience. The integration with a shopping cart would allow for a smoother and more intuitive customer journey, enabling users to add, modify, and review their items before proceeding to checkout.

- Furthermore, adding support for features like promotions, discounts, order history, and payment processing would elevate the functionality of the sales system, making it even more powerful and adaptable to real-world business needs. These enhancements would not only improve the user experience but also provide a more comprehensive and scalable solution for managing sales and customer interactions.

- As the system evolves, incorporating these resources will be crucial to keeping it competitive, efficient, and aligned with modern e-commerce standards.

---------------------------------------------------------------
# Developer Evaluation Project

`READ CAREFULLY`

## Instructions
**The test below will have up to 7 calendar days to be delivered from the date of receipt of this manual.**

- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)


## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)