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

as there is limited support on EF code.

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
├── docker-compose.yml # Orchestrates backend, frontend, and database services (don't use depricated)
└── README.md # Project documentation
```

---

## How to Run Locally

### Prerequisites

Ensure you have the following installed on your system:
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

> **Note**: Even though you see the Dockerfile, this project is not completely compatible with Docker Compose. Running the entire project on Docker Compose is deprecated or out of scope for this assessment. However, running Cassandra on Docker is supported. You can use the `docker-compose.yml` file in the `cassandra` folder for running Cassandra standalone using Docker.

### Steps to Run

1. Clone the repository:
     ```bash
     git clone https://github.com/surapuramakhil/ChatApp.git
     cd ChatApp
     ```

    2. Start the Cassandra service using Docker Compose:

        ```bash
        cd cassandra
        docker-compose up 
        ```
        Make sure port 9042 is not in use.

        Alternatively, you can use an existing Cassandra instance by setting the appropriate environment variables.

    3. Initialize your Cassandra database with the provided schema:
        ```bash
        cd ChatAppBackend
        cqlsh -f Db.sql
        ```

    4. Start the backend service:
       
    ```bash
    cd ChatAppBackend
    dotnet restore
    dotnet run
    ```

    5. Start the frontend service:
        ```bash
        cd chat-app-frontend
        npm install
        npm start
        ```

    4. Open your browser and navigate to `http://localhost:3000` to access the chat application.

---

## Environment Variables

Refer to the `.env` files added to each folder for configuration details. The `ChatAppBackend` and `chat-app-frontend` folders contain their respective `.env` files.

---

## How It Works

### When the user opens the chat application:
1. The frontend makes a REST call (`GET /api/chat/{chatId}/messages`) to fetch the last 50 messages from Cassandra.
2. A WebSocket connection is established (`ws://localhost:5000/api/chat/ws`) to receive new messages in real time.

### When a user sends a message:
1. The frontend sends an HTTP POST request (`POST /api/chat/send`) to the backend.
2. The backend stores the message in Cassandra and broadcasts it to relavant connected clients via WebSockets.

---

## Scalability Considerations

### Database Choice:
Cassandra was chosen for its high write throughput and ability to scale horizontally, making it ideal for storing chat messages.

### WebSocket Connections:
The backend uses an in-memory WebSocket manager to handle active connections. For production, consider using distributed solutions like Redis Pub/Sub or SignalR with backplane support.

## Evaluation Metrics

### Clean Code : Readable, maintainable, and well-structured code.

### Code Quality

The codebase is structured to ensure maintainability and readability. Key practices include:

- **Modular Design**: The application is divided into distinct modules, each responsible for a specific functionality. This separation of concerns makes the code easier to understand and maintain.
- **Consistent Naming Conventions**: Variables, functions, and classes follow consistent naming conventions, improving code readability.
- **Self-Explanatory Naming**: Variables, methods, and functions are named clearly and descriptively, making the code easier to understand without extensive comments.

### Security : Ensuring there are no major security issues, particularly in areas like authentication and sensitive data handling.
### Security Measures

Even though for development purposes the course has been set to allow all, this can be modified as per convenience. For HTTPS, the assumption is there would be an API gateway, such as Nginx, between the frontend and the backend server. The code also handles SQL injection before it updates the database.

- **API Gateway**: Using Nginx as an API gateway to handle HTTPS termination, load balancing, and routing requests to the backend services.
- **SQL Injection Prevention**: The application includes measures to prevent SQL injection attacks by using parameterized queries and input validation.
- **Data Encryption in Transit**: Data is encrypted during transit using HTTPS, ensuring that all communication between the client and server is secure, this can be easily achived when api gateway like ngnix is placed, with SSL keys.
- **Authorization Checks**: Proper authorization checks are implemented to ensure that users have access only to their respective chat rooms, enhancing security and privacy. These checks include:
    - Verifying access rights when fetching chat messages to ensure users can only retrieve messages from chat rooms they have access to.
    - Ensuring users can only send messages to chat rooms they are authorized to participate in.
    - Restricting WebSocket connections to authorized chat rooms, preventing unauthorized access to real-time messaging.

These security measures help ensure that the application is robust and secure, protecting user data and maintaining the integrity of the system.

### Performance: Efficient and optimized code and architecture.
### Performance Optimization

Performance is given utmost importance in this application to ensure extremely fast operations for writing, storing, and retrieving messages. The following measures contribute to the high performance:

- **Fast Database**: Cassandra is chosen for its high write throughput and low-latency read capabilities, making it ideal for storing and retrieving chat messages efficiently.
- **Efficient Data Access**: The application is optimized to quickly fetch the top 50 messages of a session, ensuring a seamless user experience when reopening the chat.
- **Real-Time Messaging**: WebSockets are used for real-time communication, minimizing latency and providing instant message delivery.

### Horizontal Scalability

Both the database and the backend server are designed to be horizontally scalable to handle increased load and ensure high availability.

#### Cassandra Database

Cassandra is inherently horizontally scalable, allowing the addition of more nodes to the cluster to handle increased read and write throughput. This ensures that the database can scale seamlessly as the number of users and messages grows.

#### .NET Backend Server

The .NET backend server is also designed for horizontal scalability. Multiple instances of the backend service can be deployed behind a load balancer to distribute incoming requests evenly. This setup ensures that the application can handle a large number of concurrent users without performance degradation.

Please note: Before horizontally scaling, make sure you add an external cache like Redis. Currently, the backend server uses an in-memory cache.

#### Caching Layer

To optimize performance and reduce unnecessary database calls, especially for authorization access checks, a caching layer is implemented. This cache stores frequently accessed data, such as user authorization information, to minimize the load on the database and improve response times.

The code is structured in a way that allows easy integration of different caching services, providing flexibility to choose the most suitable caching solution for the deployment environment.

These scalability measures ensure that the application can grow and adapt to increasing demands while maintaining high performance and reliability.


### UI


A simple UI is made for demonstrating the provided requirements.

1. **Sending Messages**: Users can type and send messages using an input field and a send button.
2. **Receiving Messages in Real Time**: Messages sent by other users appear instantly in the chat window.
3. **Retrieving Last 50 Messages**: When the chat application is opened, the last 50 messages are displayed in the chat window.

---

## Authors

Developed by Akhil Surapuram as part of the Capex Server-Side Engineer Take-Home Assignment.