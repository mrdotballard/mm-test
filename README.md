### Brief
Produce a RESTful Web API that could be called by a frontend or backend
application to handle basic calculations and return the result in the response.
Basic methods for calculations should include:
- Add
- Subtract
- Multiply
- Divide
### Minimum Requirements
- Supply a Postman collection for your API
- Demonstrate an understanding of Dependency Injection
- Must be developed using Microsoft .Net 6 or above / C#
- Use JSON requests/responses
- Upload your API to GitHub or supply a zip file
### Bonus Features
- Add basic Token auth with API key to secure your API
- Add EF Core with SQL Lite for logging requests
- Add a unit test

# MediaMaker API Test
A simple calculator API with four basic arithmetic operations.

## Endpoints
- Add (POST, GET)
- Subtract (POST, GET)
- Multiply (POST, GET)
- Divide (POST, GET)

## Setup
1. Clone repository
2. Create SQLite database:
   - Path: ``` src/CalcAPI.Infrastructure/Data/calc.db```
3. One table is created to contain the log of each POST request sent.
Apply the provided migraiton. From the CLI (Linux) within the ``` src/CalcAPI.Infrastructure ``` directory:
```
dotnet ef database update  --startup-project ../CalcAPI.Web/
```

### Configuration Files
**appsettings.json**
```json
{
  "ConnectionStrings": {
    "SQLiteDefault": "Data Source=/mm-test/src/CalcAPI.Infrastructure/Data/calc.db"
  },
  "ApiKey": "your-api-key"
}
 ```

**appsettings.test.json**
```json
 {
  "ConnectionStrings": {
    "SQLiteDefault": "Data Source=:memory:"
  },
  "ApiKey": "your-api-key"
}
```

## Testing
A Postman collection is provided in the root directory with example requests for each endpoint. Replace the placeholder ApiKey with your own key before testing.

## Architecture Decisions
### Multiple Endpoints vs Single Endpoint
1. The solution could have been implemented with only one API endpoint which receives an *operation* value. A switch statement would determine the operation and logging service information. This would have reduced the repeated code in each controller, though in the nature of a real web service, I thought it better to handle each operation via its own endpoint.
### Project Structure
2. The solution could also have had a simpler architecture using class-based seperation and DI without project layers. While risking an overkilled solution, I've been practicing some Clean Architecture approaches, so it seemed cleaner and scalable for a real-life API
### Integration Testing
3. A fair bit of config was required for the integration testing. This was a big extra and perhaps not required; I've done it mainly because I am currently looking to implement integration testing in another project, and this was a small and simple project to set it up in for understanding and practice.
