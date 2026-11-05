# Restaurant System

<p align="center">
  <strong>A full-stack restaurant ordering and operations platform built with ASP.NET Core MVC.</strong>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/">.NET 9</a> ·
  <a href="https://www.postgresql.org/">PostgreSQL</a> ·
  <a href="https://learn.microsoft.com/aspnet/core/fundamentals/websockets">WebSockets</a> ·
  <a href="https://www.rabbitmq.com/">RabbitMQ</a>
</p>

[![XERJ Corpus Hub contributor](https://hub.xerj.org/contributors/KristiyanDinev/badge.svg)](https://hub.xerj.org/contributors/KristiyanDinev)

Restaurant System brings customer ordering, reservations, delivery tracking, and restaurant staff operations together in one application. It combines a role-based MVC interface with PostgreSQL persistence, live WebSocket updates, authentication, email verification, and structured logging.

## What this project demonstrates

- Designing a multi-area business application around real restaurant workflows
- Building authenticated customer and staff experiences with role-based access
- Modeling orders, dishes, reservations, deliveries, coupons, addresses, roles, and permissions
- Delivering real-time order status updates over WebSockets
- Integrating PostgreSQL through Entity Framework Core and Npgsql
- Structuring an ASP.NET Core application into controllers, services, middleware, models, and views
- Preparing the application for production with configuration-based secrets, logging, and self-contained publishing

## Highlights

- Restaurant selection based on the user's location
- Account registration, login, logout, profile management, and password recovery
- Menu browsing by dish type
- Cart, coupon validation, and online ordering
- Order lifecycle management with live WebSocket updates
- Restaurant reservations with staff approval and cancellation workflows
- Address management and delivery tracking
- Staff dashboards for cooks, waitresses, delivery staff, and managers
- PostgreSQL persistence through Entity Framework Core
- RabbitMQ configuration support for integrations that require messaging
- File-based structured logging with Serilog

## Technology stack

| Area              | Technology                                       |
| ----------------- | ------------------------------------------------ |
| Runtime           | .NET 9 / ASP.NET Core MVC                        |
| Database          | PostgreSQL                                       |
| Data access       | Entity Framework Core and Npgsql                 |
| Authentication    | ASP.NET Core Identity cookie authentication       |
| Real-time updates | WebSockets                                       |
| Messaging         | RabbitMQ.Client                                  |
| Logging           | Serilog with rolling file output                 |
| Frontend          | Razor views, CSS, and JavaScript                 |

## Authentication approach

ASP.NET Core Identity provides password hashing, cookie authentication, password reset tokens, and role management. Staff access remains constrained by the existing database-backed service permissions, while role membership is stored in Identity's `AspNetUsers`, `AspNetRoles`, and `AspNetUserRoles` tables.

## Application flow

1. Register or sign in.
2. Select a restaurant at [`/restaurants`](./Controllers/RestaurantController.cs).
3. Browse dishes at [`/dishes`](./Controllers/DishController.cs).
4. Add dishes to [`/cart`](./Controllers/CartController.cs), optionally validate a coupon, and submit an order.
5. Follow the order at [`/orders`](./Controllers/OrderController.cs).
6. Make a reservation at [`/reservation`](./Controllers/ReservationsController.cs).

The selected restaurant is stored in the `restaurant_id` cookie. Restaurant selection is required before browsing a menu, using the cart, starting an order, or opening the reservation form.

## Order lifecycle

Orders move through these statuses:

`pending` → `preparing` → `ready` → `delivering` → `delivered`

An order can only be canceled while it is `pending`. Orders for in-restaurant tables have a `TableNumber`; online orders may leave it empty.

### Live order tracking

The application exposes the `/ws/orders` WebSocket endpoint. A client subscribes by sending JSON containing the order IDs it wants to track:

```json
{
  "orders": [1, 2, 3]
}
```

The server then sends order updates for those IDs. Access to order data is still governed by the application's authentication and authorization rules.
WebSocket connections require an authenticated Identity cookie. Customer
subscriptions are limited to the customer's orders; staff subscriptions are
limited to orders belonging to the staff member's restaurant. The server
supports fragmented text frames, rejects messages larger than 64 KiB, ignores
malformed JSON, and removes listeners and distributed mappings on disconnect.
Browser clients reconnect after an unexpected disconnect.

## Reservations

Authenticated users can view reservations and create a reservation after selecting a restaurant. Reservations use the following statuses:

- `pending` — waiting for staff review
- `accepted` — approved by staff
- `canceled` — canceled by the user or staff

Users cannot cancel an already canceled reservation or cancel it within two hours of its scheduled time. Staff must cancel a reservation before deleting it.

## Staff roles and areas

| Role | Main responsibility | Area |
| --- | --- | --- |
| `cook` | View and update dishes that need preparation | `/staff/dishes` |
| `waitress` | Manage reservations and in-restaurant orders | `/staff/reservations`, `/staff/orders` |
| `delivery` | Claim, deliver, or cancel delivery orders | `/staff/delivery` |
| `manager` | Manage employees, dishes, coupons, deliveries, and restaurant data | `/staff/manager` |
| `staff` | Shared staff permissions assigned by the application | Role configuration |

Delivery availability is based on the delivery worker's profile location. Restaurants are matched by city, state, and country; the street and building number are used as delivery-address details.

## Project structure

```text
Controllers/       MVC controllers and staff endpoints
Database/          EF Core DbContext and PostgreSQL model configuration
Enums/             Dish, order, reservation, and role enums
Extensions/        Application and infrastructure service registration
Middleware/        Authentication and WebSocket middleware
Models/            Database, form, query, view, and WebSocket models
Services/          Domain services for users, dishes, orders, delivery, and reservations
Views/             Razor UI views and shared partials
wwwroot/           Static assets, stylesheets, scripts, and uploaded content
ProjectImages/     Application screenshots used in this README
```

## Requirements

- .NET 9 SDK
- PostgreSQL
- Optional: RabbitMQ, when messaging features are enabled
- Optional: Gmail SMTP app password, when email delivery is enabled

## Configuration

Use local configuration, environment variables, user secrets, or a deployment secret store for sensitive values. Do not commit database passwords, SMTP passwords, or RabbitMQ credentials.

The application expects these configuration areas:

```json
{
  "Uri": "http://127.0.0.1:7278",
  "ConnectionString": "Host=127.0.0.1;Port=5432;Database=Restaurant;Username=RestaurantUser;Password=<secret>",
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "From_Email": "<sender>",
    "App_Password": "<app-password>",
    "OTP_Expiration_Minutes": 5
  },
  "RabbitMQ": {
    "HostName": "<host>",
    "Port": 5672,
    "Username": "<username>",
    "Password": "<password>"
  }
}
```

For Gmail SMTP, enable two-step verification and create an app password. Never use a normal account password in application configuration.

## Getting started

1. Clone the repository and enter its directory.
2. Create a PostgreSQL database and user.
3. Set the required configuration values using a local secret mechanism.
4. Restore dependencies:

   ```bash
   dotnet restore
   ```

5. Apply the Entity Framework Core migrations:

   ```bash
   dotnet ef database update
   ```

6. Start the application with hot reload:

   ```bash
   dotnet watch run
   ```

7. Open the configured URL, which defaults to `http://127.0.0.1:7278`.

If the Entity Framework CLI is not installed, install it once with:

```bash
dotnet tool install --global dotnet-ef
```

## Container deployment

The repository includes a production-oriented [Dockerfile](./Dockerfile) and [Docker Compose configuration](./docker-compose.yml). Compose runs the application with PostgreSQL and RabbitMQ, persists both services in named volumes, waits for their health checks, and exposes the application on port `7278` by default.

### 1. Create deployment secrets

Copy [`.env.example`](./.env.example) to `.env` beside `docker-compose.yml`, then replace every placeholder. Keep `.env` outside source control:

```dotenv
POSTGRES_PASSWORD=replace-with-a-long-database-password
RABBITMQ_DEFAULT_PASS=replace-with-a-long-rabbitmq-password

# Optional
# APP_PORT=7278
# POSTGRES_DB=Restaurant
# POSTGRES_USER=RestaurantUser
# SMTP_FROM_EMAIL=
# SMTP_APP_PASSWORD=
```

The committed example is a template only; the Compose stack will reject missing required values.

Generate strong values with a password manager or a cryptographically secure secret generator. Do not commit `.env` or put production credentials in `appsettings.json`.

### 2. Build and start the release stack

```bash
docker compose up --build -d
```

Open `http://localhost:7278` or the host and port configured by `APP_PORT`. Follow application logs with:

```bash
docker compose logs -f app
```

Stop the stack without deleting persistent data:

```bash
docker compose down
```

To remove the database and RabbitMQ data as well, use the destructive volume option only when the data is disposable:

```bash
docker compose down --volumes
```

### Database initialization

The application calls `EnsureCreated()` during startup so a fresh showcase database receives its tables and PostgreSQL enum types automatically. This is convenient for a CV/demo deployment, but it is not a substitute for versioned migrations in a production system. For a real deployment, add and apply EF Core migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

For a containerized deployment, run the schema initialization against the PostgreSQL service before relying on the application:

```bash
docker compose exec postgres pg_isready
```

The application also supports `StartUpSQLFile` for an explicitly configured SQL file. Mount the approved file and set `STARTUP_SQL_FILE` in `.env` only when that initialization script is safe to run on application startup.

## Database and migrations

Create a migration after changing the models:

```bash
dotnet ef migrations add <MigrationName>
```

Apply migrations:

```bash
dotnet ef database update
```

The database context defines PostgreSQL enum mappings and the relationships between users, restaurants, dishes, orders, reservations, roles, permissions, addresses, deliveries, coupons, and WebSocket order mappings.

The Identity migration changes the authentication schema. Existing deployments should create and apply an EF Core migration before starting the updated application; `EnsureCreated()` only initializes a new database.

## Build and publish

Build the application:

```bash
dotnet build
```

Run the business-logic and integration-focused unit tests:

```bash
dotnet test Tests/RestaurantSystem.Tests/RestaurantSystem.Tests.csproj
```

The test suite currently contains 18 tests covering Identity registration,
password hashing and reset, role-based service permissions, location
validation, ownership protection for orders and addresses, coupon discounts,
order status transitions and creation, delivery eligibility, reservation
capacity and ownership, ordered-dish status updates, and cart parsing.
WebSocket lifecycle behavior is implemented defensively at the
middleware/service boundary and client subscription layer.

Location lookups are served from the existing PostgreSQL country/state/city tables.
The API caches each hierarchy level for six hours and accepts `GET /location` with
optional `Country` and `State` query parameters. The legacy `POST /location` method
is also retained as a read-only compatibility alias for existing clients. The
location tables are not generated by EF Core;
import the provided location dump through `StartUpSQLFile` before using the
selectors. If the tables are missing, the endpoint returns `503` rather than
silently returning an empty list.

The application automatically initializes an empty database with the pinned
CountryStateCity PostgreSQL world dataset through the same PostgreSQL connection
used by the application. It creates and populates `countries`,
`states`, and `cities`, with states referencing countries and cities referencing
states. Existing non-empty location tables are preserved.

The dataset is published under the Open Database License (ODbL-1.0). This
application uses the CountryStateCity dataset and provides attribution here:
<https://github.com/dr5hn/countries-states-cities-database>.

You can provide a different approved SQL dump by mounting it into the app
container and setting `STARTUP_SQL_FILE` in `.env`. The custom script must be
idempotent and create/populate the same three tables.

The external dataset is intentionally downloaded at deployment time rather than
committed to the repository. If the download or import fails, application startup
fails with the import error, rather than serving an unusable location feature.
After adding or replacing a custom dump, restart the app so the script runs:

```bash
docker compose up -d --build app
```

The built-in dataset is verified against a pinned SHA-256 checksum before it is
executed. Identity cookies follow the request scheme, so they are secure when
served over HTTPS and remain usable with the local HTTP Compose binding. In
production, expose the application through HTTPS or a TLS-terminating reverse
proxy.

The startup script runs against the existing database on every application
startup, so it should be idempotent (`CREATE ... IF NOT EXISTS` and safe
upserts) or be replaced with a one-time import procedure for large dumps.

Profile images are stored under `wwwroot/assets/images/user` and served as static
files. The container creates these directories and grants the application user
write access during image build.

Publish a self-contained Windows build:

```bash
dotnet publish -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish
```

Publish a self-contained Linux x64 build:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish
```

## Logging and operations

Serilog writes application logs to `logs/app.0.log`, rolls files when they reach the configured size limit, and keeps a bounded number of retained files. Keep the `logs/` directory out of source control and review it for database, SMTP, and WebSocket diagnostics during development.

## Screenshot gallery

All screenshots are stored in [`ProjectImages/`](./ProjectImages/) and referenced with repository-relative paths. This keeps the gallery working when the repository is pushed to GitHub; keep the `ProjectImages/` folder beside this README when publishing changes.

<details>
<summary><strong>View the complete gallery (51 screenshots)</strong></summary>

### 20 November 2025

<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20211240.png" alt="Application screenshot 01" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20211252.png" alt="Application screenshot 02" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20211836.png" alt="Application screenshot 03" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20212036.png" alt="Application screenshot 04" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20212201.png" alt="Application screenshot 05" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20212735.png" alt="Application screenshot 06" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20212750.png" alt="Application screenshot 07" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20212805.png" alt="Application screenshot 08" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20212820.png" alt="Application screenshot 09" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20212840.png" alt="Application screenshot 10" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20212853.png" alt="Application screenshot 11" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215155.png" alt="Application screenshot 12" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20215210.png" alt="Application screenshot 13" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215236.png" alt="Application screenshot 14" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215249.png" alt="Application screenshot 15" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20215303.png" alt="Application screenshot 16" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215648.png" alt="Application screenshot 17" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215750.png" alt="Application screenshot 18" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20215915.png" alt="Application screenshot 19" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20215947.png" alt="Application screenshot 20" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220018.png" alt="Application screenshot 21" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20220145.png" alt="Application screenshot 22" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220222.png" alt="Application screenshot 23" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220320.png" alt="Application screenshot 24" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20220341.png" alt="Application screenshot 25" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220354.png" alt="Application screenshot 26" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220422.png" alt="Application screenshot 27" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20220445.png" alt="Application screenshot 28" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220457.png" alt="Application screenshot 29" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-20%20220603.png" alt="Application screenshot 30" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-20%20220729.png" alt="Application screenshot 31" width="32%">
</p>

### 27 November 2025

<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20105545.png" alt="Application screenshot 32" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20105742.png" alt="Application screenshot 33" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20150748.png" alt="Application screenshot 34" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20151001.png" alt="Application screenshot 35" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20182241.png" alt="Application screenshot 36" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20182505.png" alt="Application screenshot 37" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20183521.png" alt="Application screenshot 38" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20183822.png" alt="Application screenshot 39" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20184404.png" alt="Application screenshot 40" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20184916.png" alt="Application screenshot 41" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20185609.png" alt="Application screenshot 42" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20185741.png" alt="Application screenshot 43" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20185851.png" alt="Application screenshot 44" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20185931.png" alt="Application screenshot 45" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20190248.png" alt="Application screenshot 46" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20190313.png" alt="Application screenshot 47" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20191014.png" alt="Application screenshot 48" width="32%">
<img src="./ProjectImages/Screenshot%202025-11-27%20191120.png" alt="Application screenshot 49" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20191427.png" alt="Application screenshot 50" width="32%">
</p>
<p>
<img src="./ProjectImages/Screenshot%202025-11-27%20191435.png" alt="Application screenshot 51" width="32%">
</p>

</details>

## License

This project is released under the [MIT License](./LICENSE).
