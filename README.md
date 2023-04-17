# Vibbra.Hourglass

The Hourglass project is a REST API that manages projects, users, and time records. It provides authentication and authorization to ensure that only authenticated users can access the API resources. Here are the main features based on the provided controllers:

## Features

- Authentication
Users can be authenticated. To customize user authentication, use the AuthenticateController and its methods.

To authenticate a user: POST /api/v1/authenticate

- Users
Users can be created, updated, and retrieved. To manage users, use the UsersController and its methods.

To create a user: POST /api/v1/users
To update a user: PUT /api/v1/users/{id}
To retrieve a user: GET /api/v1/users/{id}

- Projects
Projects can be created, updated, retrieved, and listed. To work with projects, use the ProjectsController and its methods.

To create a project: POST /api/v1/projects
To update a project: PUT /api/v1/projects/{id}
To retrieve a project: GET /api/v1/projects/{id}
To list all projects: GET /api/v1/projects

- Time Records
Time records can be created, updated, and retrieved based on the project they are associated with. To manage time records, use the TimesController and its methods.

To create a time record: POST /api/v1/times
To update a time record: PUT /api/v1/times/{timeID}
To retrieve time records for a project: GET /api/v1/times/{projectID}

## Technologies

- AutoMapper
- ASP.NET Core
- Entity Framework Core
- JWT Authentication
- SQL Server

## Domain-Driven Design (DDD) Architecture
The DDD architecture is a software development approach that focuses on the problem domain and collaboration between subject matter experts and developers to create a rich and efficient model. The architecture is divided into layers, including Domain, Application, Infrastructure, and Presentation.

Layers
- Application
- Services
- Domain
- CrossCutting
- Infra
- Test

## Installation and Execution

Follow the steps below to clone and set up the project in your local environment:

- 1. Open a terminal or command prompt.

- 2. Clone the project repository using the following command:

```
git clone https://git.vibbra.com.br/jonathan-1681221647/vibbra-ampulheta.git
```

- 3. Navigate to the cloned project directory:
```
cd vibbra-ampulheta
```

- 4. Restore the project dependencies:
```
dotnet restore
```

- 5. Build the project:
```
dotnet build
```

- 6. Configure the database connection string in the appsettings.json file. Make sure the database is installed and available.
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HourglassDB;User Id=dbuser;Password=dbpassword;"
  }
}
```

- 7. Run the Entity Framework Core migrations to create and update the database:
```
dotnet ef database update
```

- 8. Run the project locally:
```
dotnet run --project Vibbra.Hourglass.Api
```

## Tests

The Vibbra.Hourglass project includes a separate project called Vibbra.Hourglass.Test, which contains unit and integration tests. The test project uses the NUnit framework and the Moq library for unit and integration tests. Additionally, the project uses Coverlet for code coverage collection.

To run the project tests, follow the steps below:

- 1. Navigate to the project directory containing the solution (.sln) file using the terminal or command prompt.

- 2. Run the following command to restore the project dependencies, including test dependencies:
```
dotnet restore
```

- 3. Run the following command to execute all tests in the project:
```
dotnet test
```

NUnit will run all unit and integration tests in the Vibbra.Hourglass.Test project and display the results in the terminal or command prompt.

## Author

- I am Jonathan Caravaggio Acquesta, a highly qualified professional with extensive experience in software development and project management using agile methodologies, such as Scrum and Kanban. I have a solid academic background, with an MBA in Project Management and Organizational Processes and a degree in IT Business Management, as well as various certifications, such as PSM I, KMP II, KSI, KSD, and MCTS.

- My experience includes working for companies like CBYK, Unous Business Intelligence, Talent Four Consulting, Sistema Pri Engineering, CPM Braxis Capgemini, Adedo, CNPBrasil, and Prosegur Brasil, holding positions such as Agile Coach, Project Manager, Agile Master, Scrum Master, Development Analyst, and Programmer.

- I have knowledge in web and app development, using technologies like Vue JS, ASP.NET MVC, C#, VB.NET, and SQL Server, as well as skills in team management and conflict resolution. I am fluent in Portuguese, have advanced English skills, and basic knowledge of Italian and Spanish.

- My diverse experience and technical and managerial skills make me the right person to work on projects seeking to add value through the application of agile methodologies and the development of high-quality software.