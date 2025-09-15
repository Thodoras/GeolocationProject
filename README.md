# IP Geolocation API

A .NET 8 Web API for IP address geolocation that provides both single IP lookup and batch processing capabilities using a domain driven architecture.

## Features

- **Single IP Geolocation**: Get geographical information for a single IP address
- **Batch Processing**: Process multiple IP addresses asynchronously with progress tracking
- **Background Processing**: Asynchronous background processing of batch jobs
- **RESTful API**: Clean API endpoints with proper HTTP status codes
- **SQL Server Integration**: Entity Framework with SQL Server Express
- **Docker Support**: Containerized deployment with Docker Compose
- **Domain Driven Approach**: Following the DDD paradigm.

## Technology Stack

- **Framework**: .NET 8
- **Database**: SQL Server Express 2022
- **ORM**: Entity Framework Core
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing

## API Endpoints

### 1. Get Single IP Geolocation
```http
GET /api/geolocationip/{ipAddress}
```
**Response:**

```json
{
  "ip": "8.8.8.8",
  "countryCode": "US",
  "countryName": "United States",
  "timeZone": "America/New_York",
  "latitude": 37.4056,
  "longitude": -122.0775
}
```
### 2. Create Batch Process
```http
POST /api/batch
```
**Request:**
```json
{
  "ipAddresses": ["8.8.8.8", "1.1.1.1", "192.168.1.1"]
}
```
**Response:**
```json
{
  "batchId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "statusUrl": ".../api/batch/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status"
}
```
### 3. Get Batch Status
```http
GET /api/batch/{processId}/status
```
**Response:**
```json
{
  "batchId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "status": "Processing",
  "processed": 15,
  "total": 100,
  "progressPercentage": 15.0,
  "estimatedCompletionTime": "2023-12-01T10:30:00Z",
  "items": [
    {
      "ipAddress": "8.8.8.8",
      "status": "Completed",
      "countryCode": "US",
      "countryName": "United States"
      "latitude": 37.4056,
      "longitude": -122.0775
    },
    ...
  ]
}
```
### More API info can be found in the link below (dev env)
```http
/swagger
```

## Getting Started
### Prerequisites

- .NET 8 SDK
- Docker Desktop
- SQL Server Express

## Local Development
### 1. Clone the repository:
```bash
git clone <repository-url>
cd GeolocationProject
```
### 2. Apply Database Migrations
```bash
cd src
cd GeolocationAPI
dotnet ef database update
```
### 3. Run application
```bash
dotnet run
```
### 4. Base url port
Refer to the logs generated.
## Docker run
### 1. Build and run
```
docker compose up (optionally --build)
```
### 2. Base url
```
http://localhost:8080
```
## Testing

In this version, a small subset of functionality was tested solely to demonstrate testing patterns. Full test coverage was not the objective.

### 1. Run tests
```bash
cd tests
cd GeolocationAPI.Tests
dotnet test
```

## Alternative solution Using Queuing Mechanisms
An alternative approach that could result in cleaner, more maintainable code�albeit with greater infrastructural effort�would involve introducing a queuing mechanism, such as Amazon SQS. The process would work as follows:
1. **Initial Request Handling**:
The GeolocationAPI would receive a request containing a list of IP addresses. It would generate a process UUID, store metadata about the request, and enqueue each IP address along with the associated UUID. It would then return the UUID along with a URL that clients can use to retrieve the results.
2. **Queue Processing**:
An internal would consume messages from the queue asyncronously one by one, each containing a UUID-IP pair. For each IP, it would make a synchronous call to FreeGeoIP to retrieve geolocation data.
3. **Data Storage**:
The geolocation results would then be stored in the database. An internal counter would track processed entries. Since this uses an SQL database, incrementing the counter would be atomic and thread-safe.
### Benefits:
- Throttling can be managed via the queue's configuration (e.g. controlling message delivery rate).
- Concurrency and parallelism can be handled by the framework as well as the underlying infrastructure (e.g. Kubernetes, EC2 autoscaling, etc.).
- Atomic operations (such as counter increments) are supported natively by SQL, avoiding the need for manual locking.
- Cleaner code with reduced complexity in handling concurrent logic, improving readability and maintainability.

### Drawbacks
- Requires additional infrastructure setup (queues, workers, deployment configuration).
- May introduce unnecessary complexity and cost for a small-scale or demo assignment.