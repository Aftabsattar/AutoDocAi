# AutoDocAi

AutoDocAi is a lightweight, RESTful document management API built with ASP.NET Core 8 and Entity Framework Core. It enables you to create, retrieve, and manage documents that store flexible key–value data, backed by PostgreSQL and documented with Swagger.

---

## Features ✨

- Dynamic document storage using flexible key–value pairs
- Clean REST API surface
- Entity Framework Core + PostgreSQL integration
- Built-in Swagger/OpenAPI for interactive docs and testing
- .NET 8-ready with a simple, scalable architecture

---

## Technologies Used 🛠️

- C# (.NET 8)
- ASP.NET Core (Web API)
- Entity Framework Core
- PostgreSQL
- Swagger / Swashbuckle

---

## Project Structure 📂

```
AutoDocAi/
│
├── Controllers/            # API Controllers
├── Database/               # DbContext and Entities
├── DTOs/                   # Data Transfer Objects
├── Migrations/             # EF Core Migrations
├── Properties/             # Project Properties
├── AutoDocAi.csproj        # Project file
├── AutoDocAi.http          # Example HTTP requests for local testing
├── .gitattributes
├── .gitignore
└── AutoDocAi.sln           # Solution file
```

---

## Getting Started ⚙️

### Prerequisites

1. .NET 8.0 SDK
2. PostgreSQL running locally or accessible remotely
3. (Optional) EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1) Clone the repository

```bash
git clone https://github.com/Aftabsattar/AutoDocAi.git
cd AutoDocAi
```

### 2) Configure the connection string

Update appsettings.json with your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=autodocai;Username=postgres;Password=postgres"
  }
}
```

Tip: You can also use user secrets or environment variables in production.

### 3) Apply database migrations

```bash
dotnet ef database update
```

### 4) Run the application

```bash
dotnet run
```

By default, Swagger UI is available at:

```
http://localhost:5235/swagger
```

---

## API Reference 🛠️

Base URL (local): `http://localhost:5235`

### 1. Seed Document
- Method: POST
- Endpoint: `/api/Document/seed-document`
- Description: Create and save a new document with flexible key–value data.
- Request Body:
  ```json
  {
    "FormName": "SampleForm",
    "Data": {
      "Key1": "Value1",
      "Key2": "Value2"
    }
  }
  ```
- Response:
  ```json
  {
    "id": 1,
    "message": "Document Saved Successfully"
  }
  ```

### 2. Get Document by Name
- Method: GET
- Endpoint: `/api/Document/by-name?formName={formName}`
- Description: Retrieve a single document by its form name.
- Example Response:
  ```json
  {
    "id": 1,
    "formName": "SampleForm",
    "data": {
      "Key1": "Value1",
      "Key2": "Value2"
    }
  }
  ```

### 3. Get All Documents
- Method: GET
- Endpoint: `/api/Document/get-all-documents`
- Description: Retrieve all documents.
- Example Response:
  ```json
  [
    {
      "id": 1,
      "formName": "SampleForm",
      "data": {
        "Key1": "Value1",
        "Key2": "Value2"
      }
    }
  ]
  ```

---

## Development 🖥️

- Swagger UI is enabled for testing and documentation.
- A sample HTTP file is provided for quick local testing:
  - Open `AutoDocAi.http` in VS Code with the REST Client extension.

Useful commands:
- Restore: `dotnet restore`
- Build: `dotnet build`
- Watch run: `dotnet watch`

---

## Contributing 🤝

Contributions are welcome! Please open an issue to discuss changes or submit a pull request.

---

## License 📝

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

## Contact 📧

For questions or feedback, reach out to [Aftabsattar](https://github.com/Aftabsattar).
