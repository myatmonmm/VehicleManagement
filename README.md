# Vehicle Management

ASP.NET Core MVC application for managing vehicles and weight-based vehicle categories.

## Technology

- .NET 10
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- xUnit
- Bootstrap / Bootstrap Icons

## Features

### Vehicles

- Add, view, edit and delete vehicles
- Select manufacturers from database-backed manufacturer data
- Automatically determine a vehicle's category from its weight
- Sort by owner, manufacturer, year or weight
- Sort in ascending or descending order
- Server-side validation for vehicle data

### Categories

- Add, edit and delete vehicle categories
- Configure category names, weight ranges and icons
- Prevent gaps and overlaps between weight ranges
- Automatically adjust adjacent ranges when category boundaries change
- Existing vehicles immediately reflect category configuration changes
- Category names are unique

## Category Assignment

A vehicle does not store a `CategoryId`.

Its category is calculated from `WeightKg` using the current category configuration. This avoids storing duplicated state and means that changing category ranges immediately affects existing vehicles without updating each vehicle record.

Category ranges use a lower-inclusive and upper-exclusive rule:

```text
[MinWeight, MaxWeight)
```

The initial configuration is:

```text
Light   [0, 500)
Medium  [500, 2500)
Heavy   [2500, no upper limit)
```

For example:

- `499.99 kg` is Light
- `500 kg` is Medium
- `2499.99 kg` is Medium
- `2500 kg` is Heavy

Vehicle weights must be greater than zero.

## Category Range Integrity

Category configuration must provide continuous coverage without gaps or overlaps.

When a category is created, an existing range is split at the new category's starting weight.

When a boundary is edited, the adjacent category boundary is adjusted to preserve continuous coverage.

When a category is deleted, its range is merged into an adjacent category.

The final category is open-ended.

## Database Design

The main entities are:

### Vehicle

- Id
- OwnerName
- ManufacturerId
- YearOfManufacture
- WeightKg

### Manufacturer

- Id
- Name

### VehicleCategory

- Id
- Name
- MinWeight
- MaxWeight
- Icon

Manufacturers and the initial vehicle categories are seeded through Entity Framework Core.

The initial manufacturers are:

- Mazda
- Mercedes
- Honda
- Ferrari
- Toyota

Manufacturers are stored in the database rather than hard-coded throughout the application, allowing the list to be extended in the future.

Category names have a unique database index.

Vehicle weight and category boundaries use decimal precision appropriate for values with up to two decimal places.

## Validation

Validation is performed server-side.

Vehicle validation includes:

- Owner is required
- Manufacturer must exist
- Year must be within a sensible range
- Weight must be greater than zero
- Weight must contain no more than two decimal places

Category validation includes:

- Name is required and unique
- Icon is required
- Minimum weight cannot be negative
- Ranges cannot overlap
- Ranges cannot contain gaps
- The first range starts at 0
- The final range has no upper limit

## Setup

### Prerequisites

- .NET 10 SDK
- SQL Server 2019 or later
- Entity Framework Core CLI (`dotnet-ef`)

SQL Server Express or a SQL Server Docker container can also be used.

If `dotnet-ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### Configure the Database

The application expects a connection string named:

```text
DefaultConnection
```

For local development, configure it using .NET User Secrets rather than committing credentials to source control:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project src/VehicleManagement.Web
```

For example, a local SQL Server connection string can be configured according to the SQL Server instance available on the developer's machine.

No database credentials are stored in the repository.

### Apply Database Migrations

From the repository root:

```bash
dotnet ef database update \
  --project src/VehicleManagement.Web \
  --startup-project src/VehicleManagement.Web
```

The migrations create the database schema and seed the initial manufacturers and vehicle categories.

### Build

From the repository root:

```bash
dotnet build
```

### Run

```bash
dotnet run --project src/VehicleManagement.Web
```

Open the localhost address displayed in the terminal.

The application opens on the vehicle list page.

### Run Tests

From the repository root:

```bash
dotnet test
```

## Tests

Automated tests cover the core business rules, including:

- Weight/category boundary behaviour
- Exact category boundary values
- Gap detection
- Overlap detection
- Category changes affecting category determination
- Invalid vehicle years
- Non-positive vehicle weights
- Weights with more than two decimal places
- Valid vehicle data

The category boundary tests include the initial configuration boundaries at `500 kg` and `2500 kg`.

## Design Decisions

The application uses a straightforward MVC structure with Entity Framework Core for persistence and a focused `CategoryService` for category business rules.

A separate repository layer was not introduced because Entity Framework Core already provides the required data-access abstraction for the scope of this application. Additional architectural layers would add complexity without providing a clear benefit for the current requirements.

### Calculated Vehicle Category

Category is calculated rather than stored on a vehicle.

`WeightKg` and the current category configuration are the sources of truth. This prevents stale category assignments when category ranges change and allows existing vehicles to immediately reflect the latest configuration.

### Manufacturers

Manufacturers are stored in the database rather than represented as an enum or duplicated throughout the application.

The initial manufacturer records are seeded through Entity Framework Core. Additional manufacturers can be introduced without changing the Vehicle model or category assignment logic.

### Category Boundaries

Category ranges follow the rule:

```text
MinWeight <= weight < MaxWeight
```

The final category has no maximum weight.

This provides deterministic behaviour for vehicles whose weights fall exactly on a category boundary.

## Assumptions

- Vehicle weights are expressed in kilograms.
- Vehicle weight must be greater than zero.
- A weight exactly equal to a category's upper boundary belongs to the next category.
- The first category begins at 0 kg to ensure complete positive-weight coverage.
- The final category has no upper weight limit.
- Category icons are selected from the Bootstrap Icon options provided by the application.
- Manufacturers are predefined data and are expected to change infrequently.
- Authentication and authorization are outside the scope of this assignment.

## Possible Improvements

With more time, possible improvements include:

- Integration tests using a test database
- Additional automated tests for sorting and controller behaviour
- More configurable icon options
- Pagination and filtering for large vehicle lists
- Improved UI styling and accessibility
- Concurrency handling for simultaneous category configuration changes
- Administrative management of manufacturers if it becomes a business requirement