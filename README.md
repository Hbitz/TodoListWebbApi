# TodoWebApi Project

## Overview
This is a Todo Web API built with ASP.NET Core. It supports user authentication, todo and category management with user-specific data, and uses Entity Framework Core for data access. It includes JWT authentication and validation.

## Getting Started

### Prerequisites
- .NET 8 SDK or newer
- SQL Server Express or any SQL Server instance
- Postman (to run the included collection)
- (Optional) Visual Studio 2022 or VS Code for development

### Setup

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/TodoWebApi.git
cd TodoWebApi
```

2. **Update the connection string**

Edit `appsettings.json` and update the `DefaultConnection` string to point to your SQL Server instance. For example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TodoAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Make sure your server name matches your local SQL Server instance name.

3. **Run the application**

Run the app using CLI or your IDE:

```bash
dotnet run
```

By default, the API will be available at:

```
https://localhost:7103/api
```

4. **Seed data**

On first run, if the database is empty, Bogus will seed fake users, categories, and todos automatically.

---

## Using Postman Collection

- Import the `TodoWebApi.postman_collection.json` file found in the `Postman` folder.
- Create a new environment in Postman.
- Add the following environment variable:

| Variable | Initial Value                 | Current Value                 |
|----------|------------------------------|------------------------------|
| baseUrl  | https://localhost:7103/api   | https://localhost:7103/api   |

- Disable SSL certificate verification in Postman settings (if needed) to avoid SSL errors with localhost.
- Run the requests in order. The collection handles storing and using authentication tokens and IDs between requests.

---

## Notes

- Remember to **never commit real secrets or production connection strings** to the repo.
- You may want to create your own `appsettings.json` locally or use user secrets for production.
- CORS is currently disabled; you can enable it later if you plan to call the API from a web front-end.

---

## Contact

For questions or issues, please open an issue on the repo or contact me directly.


