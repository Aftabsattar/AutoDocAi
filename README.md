# AutoDocAi

AutoDocAi is an advanced document management system built with ASP.NET Core and Entity Framework. This project provides an API for creating, retrieving, and managing documents with dynamic data storage capabilities. Its modular structure and use of modern technologies make it scalable and easy to integrate into existing systems.

---

## Features ✨

- **Dynamic Document Management**: Create and manage documents with flexible key-value data storage.
- **RESTful API**: Provides endpoints for interacting with documents.
- **Entity Framework Core**: Seamless integration with PostgreSQL for database operations.
- **Swagger Integration**: API documentation and testing built-in.
- **Scalability**: Ready for modern .NET 8.0 applications.

---

## Technologies Used 🛠️

- **C#** (100%)
- **ASP.NET Core**: Backend framework.
- **Entity Framework Core**: Database ORM.
- **PostgreSQL**: Relational database.
- **Swagger**: API documentation and testing.

---

## Project Structure 📂

The repository is organized as follows:

```
AutoDocAi/
│
├── Controllers/            # API Controllers
├── Database/               # Database Context and Entities
├── DTOs/                   # Data Transfer Objects
├── Properties/             # Project Properties
├── Migrations/             # Entity Framework Migrations
├── AutoDocAi.csproj        # Project File
├── AutoDocAi.http          # HTTP Request Samples
├── .gitignore              # Git Ignore Rules
├── .gitattributes          # Git Attributes
└── AutoDocAi.sln           # Visual Studio Solution File
```

---

## Installation and Setup ⚙️

### Prerequisites

1. **.NET 8.0 SDK**: Make sure to install the latest .NET 8.0 SDK.
2. **PostgreSQL**: Ensure PostgreSQL is installed and running.

### Steps to Run

1. Clone the repository:

   ```bash
   git clone https://github.com/Aftabsattar/AutoDocAi.git
   cd AutoDocAi
   ```

2. Update the database connection string in `appsettings.json`.

3. Apply migrations to the database:

   ```bash
   dotnet ef database update
   ```

4. Run the application:

   ```bash
   dotnet run
   ```

5. Access the Swagger API documentation at:

   ```
   http://localhost:5235/swagger
   ```

---

## API Endpoints 🛠️

Here are some of the key API endpoints:

### 1. **Seed Document**
   - **Endpoint**: `POST /api/Document/seed-document`
   - **Description**: Create a new document.
   - **Request Body**:
     ```json
     {
       "FormName": "SampleForm",
       "Data": {
         "Key1": "Value1",
         "Key2": "Value2"
       }
     }
     ```
   - **Response**:
     ```json
     {
       "id": 1,
       "message": "Document Saved Successfully"
     }
     ```

### 2. **Get Document by Name**
   - **Endpoint**: `GET /api/Document/by-name?formName={formName}`
   - **Description**: Retrieve a document by its name.
   - **Response**:
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

### 3. **Get All Documents**
   - **Endpoint**: `GET /api/Document/get-all-documents`
   - **Description**: Retrieve all documents.
   - **Response**:
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

## Development Setup 🖥️

### HTTP Request Testing

A sample `.http` file (`AutoDocAi.http`) is included to test your API endpoints locally. Use tools like [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) in VS Code.

---

## Contributing 🤝

Contributions are welcome! Feel free to submit a pull request or open an issue to discuss improvements or new features.

---

## License 📝

This project is licensed under the [MIT License](LICENSE).

---

## Contact 📧

For any inquiries or feedback, feel free to contact the repository owner at [Aftabsattar](https://github.com/Aftabsattar).

---

This README provides an overview of the project, setup instructions, API details, and contribution guidelines. Let me know if you’d like to make any adjustments or additions!
