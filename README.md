# Chat Application

This is a sample browser-based chat application built as part of the Capex Take-Home Assignment for the Server-Side Engineer role. The application provides real-time chat functionality, persists user data, and restores the last 50 messages upon reopening.

---

## Features

- **Real-Time Messaging**: Users can send and receive messages in real time using WebSockets.
- **Message Persistence**: Messages are stored in a Cassandra database for scalability and performance.
- **Session Restoration**: The last 50 messages are retrieved and displayed when the user reopens the chat.
- **Scalable Architecture**: Designed for high performance and scalability with a NoSQL database and containerized services.

---

## Tech Stack

### Backend
- **Language**: C# (.NET 9)
- **Framework**: ASP.NET Core Web API
- **Database**: Cassandra (NoSQL)
- **Communication Protocols**:
    - REST API for fetching historical messages.
    - WebSockets for real-time messaging.

### Frontend
- **Language**: JavaScript
- **Framework**: React.js
- **Build Tool**: Nginx (for serving production builds).

### Infrastructure
- **Containerization**: Docker Compose to orchestrate backend, frontend, and database services.

## Technical Decisions

As performance and scalability are critical aspects of this assessment

### Performance vs. Developer Experience

In this project, performance is prioritized over developer experience. This trade-off ensures that the application can handle high loads and provide real-time messaging without significant latency. While this may introduce some complexity in the development process, it is essential for maintaining the application's responsiveness and scalability.

### Database Choice : Cassandra

trade offs: Performance vs. Developer Experience

Cassandra is chosen because of its fast read and write performance, which is ideal for handling large volumes of chat messages efficiently. Its ability to scale horizontally ensures that the application can handle increased load without compromising on performance.

While Cassandra offers significant advantages in terms of performance and scalability, it also comes with some drawbacks, when it come to

- **Lack of ORM Support**: Unlike relational databases, Cassandra lacks mature ORM (Object-Relational Mapping) support for .NET. This means developers need to write CQL (Cassandra Query Language) directly, which can be more complex and error-prone.
- **Limited Tooling**: The ecosystem around Cassandra is not as mature as that of relational databases. There are fewer tools available for database migrations, schema management, and other common database tasks.
- **Learning Curve**: Developers familiar with SQL databases may find Cassandra's data model and query language challenging to learn and use effectively.
- **Community Support**: The community and resources available for Cassandra are not as extensive as those for more established relational databases, which can make finding solutions to problems more difficult.

Despite these drawbacks, the benefits of using Cassandra for high-performance, scalable applications often outweigh the challenges, especially for use cases like real-time messaging where speed and scalability are critical.

as there is limited support on EF code - Evolve has added for migrations

### Database Migrations with Evolve

To manage database schema changes, we use the Evolve database migration tool. Evolve is a lightweight migration tool that helps to evolve your database schema, keeping it synchronized with your application model.

#### Setting Up Evolve

1. **Install Evolve**:
    Add the Evolve dependency to your project. For .NET projects, you can install it via NuGet:
    ```bash
    dotnet add package Evolve
    ```

2. **Configuration**:
    Configure Evolve in your `appsettings.json` or environment variables:
    ```json
    {
         "Evolve": {
              "Locations": "db/migrations",
              "Command": "migrate",
              "ConnectionString": "Server=cassandra_db;Port=9042;Keyspace=chatapp",
              "Driver": "Cassandra"
         }
    }
    ```

3. **Create Migration Scripts**:
    Place your migration scripts in the specified `Locations` directory (e.g., `db/migrations`). Each script should be named sequentially (e.g., `V1__Create_table.sql`).

4. **Run Migrations**:
    Execute Evolve migrations as part of your application startup or as a separate step in your deployment pipeline:
    ```csharp
    var evolve = new Evolve.Evolve("ConnectionString", msg => Console.WriteLine(msg))
    {
         Locations = new[] { "db/migrations" },
         IsEraseDisabled = true
    };
    evolve.Migrate();
    ```

Using Evolve ensures that your Cassandra database schema is always up-to-date with the latest changes, reducing the risk of inconsistencies and simplifying the deployment process.

---

---

## Folder Structure

```zsh
/project-root/
│
├── ChatAppBackend/ # Backend (ASP.NET Core Web API)
│   ├── Controllers/ # API endpoints (e.g., ChatController.cs)
│   ├── Services/ # Business logic (e.g., ChatService.cs)
│   ├── Repositories/ # Database interaction (e.g., CassandraRepository.cs)
│   ├── Program.cs # Application entry point
│   └── Dockerfile # Dockerfile for backend service
│
├── chat-app-frontend/ # Frontend (React.js)
│   ├── src/ # React components (e.g., App.js)
│   ├── public/ # Static assets
│   ├── package.json # Frontend dependencies
│   └── Dockerfile # Dockerfile for frontend service
│
├── docker-compose.yml # Orchestrates backend, frontend, and database services
└── README.md # Project documentation
```

---

## How to Run Locally

### Prerequisites

Ensure you have the following installed on your system:
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Steps to Run

1. Clone the repository:
     ```bash
     git clone <repository-url>
     cd project-root/
     ```

2. Build and start all services using Docker Compose:
     ```bash
     docker-compose up --build
     ```

3. Access the application:
     - **Frontend**: Open your browser and navigate to `http://localhost:3000`.
     - **Backend API**:
         - REST API endpoint: `http://localhost:5000/api/chat/{chatId}/messages`.
         - WebSocket endpoint: `ws://localhost:5000/api/chat/ws`.

4. Stop all services:
     ```bash
     docker-compose down
     ```

---

## Environment Variables

The following environment variables are used in the project:

### Backend (ChatAppBackend)

| Variable              | Description                          | Default Value |
|-----------------------|--------------------------------------|---------------|
| ASPNETCORE_ENVIRONMENT | Environment mode (Development/Production) | Development   |
| CASSANDRA_HOST        | Hostname of the Cassandra service    | cassandra_db  |
| CASSANDRA_PORT        | Port number for Cassandra            | 9042          |
| CASSANDRA_KEYSPACE    | Keyspace name in Cassandra           | chatapp       |

### Frontend (chat-app-frontend)

| Variable              | Description                          | Default Value |
|-----------------------|--------------------------------------|---------------|
| REACT_APP_API_URL     | Base URL for backend API             | http://backend/api |

---

## API Endpoints

### REST Endpoints

#### Retrieve Last 50 Messages

```http
GET /api/chat/{chatId}/messages
```

**Description**: Fetches the last 50 messages from a specific chat session.

**Response**:
```json
[
    {
        "chatId": "uuid",
        "senderId": "uuid",
        "body": "Hello, world!",
        "timestamp": "2024-11-21T12:00:00Z"
    }
]
```

#### Send a Message

```http
POST /api/chat/send
```

**Description**: Sends a new message to a chat session.

**Request Body**:
```json
{
    "chatId": "uuid",
    "senderId": "uuid",
    "body": "Hello!"
}
```

### WebSocket Endpoint

#### Real-Time Messaging via WebSockets

```ws
ws://localhost:5000/api/chat/ws
```

**Description**: Establishes a WebSocket connection for receiving new messages in real time.

---

## How It Works

### When the user opens the chat application:
1. The frontend makes a REST call (`GET /api/chat/{chatId}/messages`) to fetch the last 50 messages from Cassandra.
2. A WebSocket connection is established (`ws://localhost:5000/api/chat/ws`) to receive new messages in real time.

### When a user sends a message:
1. The frontend sends an HTTP POST request (`POST /api/chat/send`) to the backend.
2. The backend stores the message in Cassandra and broadcasts it to all connected clients via WebSockets.

---

## Scalability Considerations

### Database Choice:
Cassandra was chosen for its high write throughput and ability to scale horizontally, making it ideal for storing chat messages.

### WebSocket Connections:
The backend uses an in-memory WebSocket manager to handle active connections. For production, consider using distributed solutions like Redis Pub/Sub or SignalR with backplane support.

### Containerization:
The application is fully containerized using Docker Compose, making it easy to scale individual services (e.g., scaling backend instances).

---

## Authors

Developed by Akhil Surapuram as part of the Capex Server-Side Engineer Take-Home Assignment.