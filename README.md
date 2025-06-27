# Audit Logging Microservice

A minimal audit logging service built with ASP.NET Core 8.0, following clean architecture principles.

---

## Features

- `POST /events` — Accepts a new audit event
- `GET /events` — Retrieves events with optional filters + pagination
- `POST /events/replay` — Simulates replay of events (logs them)

---

## Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/gdluckpeh/SingularHealthAssignment.git
cd SingularHealthAssignment
```

### 2. Run the app

```bash
dotnet run --project SingularHealthAssignment --launch-profile https
```

or in Visual Studio:  
- Open `SingularHealthAssignment.sln`
- Set `SingularHealthAssignment` as Startup Project. There are 3 modes, but https is recommended
- Run the project

Then, visit:  `[https://localhost:7208/swagger]`

---

## API Endpoints

### `POST /events`
Create an audit event.

```json
{
  "serviceName": "string",
  "eventType": "string",
  "payload": "{"key": "value"}"
}
```

### `GET /events`
Query audit events.

**Optional query parameters**:
- `serviceName`
- `eventType`
- `from` / `to` (timestamps)
- `page`, `pageSize`

### `POST /events/replay`
Simulate replay of audit events.

```json
["<eventId1>", "<eventId2>"]
```

---

## Running Tests

```bash
dotnet test SingularHealthAssignmentTests
```
