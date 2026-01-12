dotnet dev-certs https --trust# HireFire

A distributed job-matching platform designed to streamline recruitment by enabling fast applications, recruiter decision-making, and real-time communication between matched candidates and recruiters.

HireFire was built as a full-stack, three-tier system with a strong focus on **clear separation of concerns**, **secure communication**, and **real-time interaction**.

---

## Overview

HireFire rethinks traditional job portals by reducing friction on both sides of the hiring process. Applicants can quickly browse and apply to relevant job listings without lengthy forms, while recruiters can efficiently review candidates, accept or decline applications, and immediately communicate with matched applicants via real-time chat.

The system is implemented as a **distributed 3-tier architecture** consisting of:

- A **Blazor-based client** (presentation layer)
- A **.NET application server** (logic tier)
- A **Java Spring Boot data server** (persistence tier)

This architecture allows the system to remain scalable, maintainable, and secure, while supporting future feature expansion.

---

## Key Features

- Role-based system (Applicant, Recruiter, Company Representative, Admin)
- Job listing creation, editing, closing, and removal
- Skill- and location-based job recommendations
- Application tracking with clear status updates
- Real-time recruiter–applicant chat using WebSockets
- Secure authentication with hashed passwords
- Strict separation between UI, business logic, and data access

---

## System Architecture

HireFire follows a **three-tier distributed architecture**:

    Client (Blazor Server)
       |
       |  REST (HTTPS) + SignalR
       v
    Logic Server (ASP.NET Core)
       |
       |  gRPC (TLS)
       v
    Data Server (Java Spring Boot + PostgreSQL)

---

## Repository Structure

    Client/WebApp              # Blazor Server application (UI & client-side logic)
    LogicTier/LogicServer      # ASP.NET Core application (business logic & APIs)
    DataTier/DatabaseServer    # Java Spring Boot application (database & gRPC services)
    Docs/                      # Project Description, Project Report and Process Report

Each tier is fully isolated and communicates only through defined interfaces.

---

## Tech Stack

### Frontend
- Blazor Server (C#)
- SignalR (WebSockets)
- HTML / CSS

### Backend
- ASP.NET Core (.NET)
- Java Spring Boot
- gRPC (Protocol Buffers)

### Database
- PostgreSQL
- JPA / Hibernate

### Security
- HTTPS
- gRPC over TLS
- SHA-256 password hashing
- Role-based access control (client-side)

---

## Communication Overview

- **Client ⇔ Logic Server**
  - REST (JSON over HTTPS)
  - SignalR for real-time chat

- **Logic Server ⇔ Data Server**
  - gRPC with Protocol Buffers
  - TLS-secured channels

---

## Running the Project Locally (Sample Setup)

---

### Prerequisites

- .NET SDK 6.0+
- Java JDK 17+
- PostgreSQL
- Git

---

### 1. Clone the Repository

    git clone https://github.com/baczek113/SEP3.git
    cd SEP3

---

### 2. Database Setup

1. Create a PostgreSQL schema by running HireFire.sql found under DatabaseServer/Database.

2. Configure database credentials in:

       DatabaseServer/src/main/resources/application.properties

   Example:

       spring.datasource.url=jdbc:postgresql://localhost:5432/postgres?currentSchema=HireFire
       spring.datasource.username=postgres
       spring.datasource.password=your_password

3. Ensure PostgreSQL is running.

---

### 3. HTTPS setup

1. Navigate to DatabaseServer/src/main/resources using the Terminal.

2. Generate a 2048-bit RSA key pair by entering the following command and replacing [your-password] with your designated password.
   
       keytool -genkeypair -alias hirefire-data -keyalg RSA -keysize 2048 -storetype PKCS12 -keystore keystore.p12 -validity 3650 -storepass [your_passoword]

3. Follow through with the key generation procedure.

4. Configure password in:

       DatabaseServer/src/main/resources/application.properties

   Replace [your-password] with the password entered previously:

       grpc.server.security.key-store-password=[your-password]

5. Navigate to LogicTier using the Terminal and enter the following:

       dotnet dev-certs https --trust

6. If a pop-up window appears - Click "Yes".

---

### 4. Start the Data Server (Java)

    cd DataTier/DatabaseServer
    ./mvnw spring-boot:run

- Hosts database access
- Exposes gRPC services

---

### 5. Start the Logic Server (.NET)

    cd LogicTier/LogicServer
    dotnet restore
    dotnet run

- Exposes REST APIs
- Hosts SignalR hubs
- Communicates with the Data Server via gRPC

---

### 6. Start the Client (Blazor Server)

    cd Client/WebApp
    dotnet restore
    dotnet run

- Runs the UI
- Connects to the Logic Server via HTTPS
- Enables real-time chat via SignalR

---

### 7. Access the Application

Open your browser at:

    https://localhost:7292

---

## Security Notes

- Passwords are hashed client-side using SHA-256
- Plain-text passwords never leave the browser
- All inter-service communication is encrypted
- Authentication state is stored in browser session storage
- Authorization is enforced through role-based UI rendering

---

## Design Patterns Used

- **DTO Pattern** – Structured data exchange between layers
- **Dependency Injection** – Loose coupling and testability
- **Proxy Pattern** – Abstracted HTTP communication in the client
- **Observer Pattern** – Real-time messaging via SignalR

---

## Testing Overview

- Black-box testing based on use cases
- Unit testing with JUnit 5 for core domain entities
- Integration testing across all tiers
- Manual verification of real-time chat functionality

---

## Known Limitations & Future Improvements

- No CV upload functionality
- Limited filtering for job listings and applications
- Authorization enforced mainly at UI level
- Admin feature set could be expanded
- Matching algorithm can be refined

---

## Academic Context

This project was developed as part of a **Software Technology Engineering** course.  
While created in an academic setting, the system follows production-style architecture and practices.
Full project and process reports are available in the `/Docs` directory.

---

## Authors

- Hamsa Abdullah Sheikhdon  
- Tymoteusz Krzysztof Żydkiewicz  
- Caranfil Cristian  
- Damian Michał Choina  
- Jakub Maciej Bączek  
