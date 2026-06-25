# Vehicle Lookup

A web application that allows users to search car makes, select a manufacture year, and view available vehicle types and models. Built with .NET 8 and Angular 21.

**Live:** [http://34.244.242.137](http://34.244.242.137)

## Tech Stack

- **Backend:** ASP.NET Core 8 Minimal API with in-memory caching
- **Frontend:** Angular 21 (standalone components, signals)
- **Data Source:** [NHTSA vPIC API](https://vpic.nhtsa.dot.gov/api/)
- **Containerization:** Multi-stage Docker build (Node 22 + .NET 8 SDK → .NET 8 runtime)
- **Hosting:** AWS EC2 (Amazon Linux 2023, t3.micro)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 22+](https://nodejs.org/)
- [Angular CLI](https://angular.dev/) (`npm install -g @angular/cli`)
- [Docker](https://www.docker.com/products/docker-desktop/) (for containerized run)

## Run Locally (without Docker)

1. **Clone the repository**

   ```bash
   git clone https://github.com/almashtooli/vehicle-lookup.git
   cd vehicle-lookup
   ```

2. **Start the API**

   ```bash
   dotnet run --project VehicleLookup.Api
   ```

   The API starts at `https://localhost:7181`.

3. **Start the Angular client** (in a second terminal)

   ```bash
   cd client
   npm install
   ng serve
   ```

   The client starts at `http://localhost:4200` and proxies API calls to the backend.

4. **Open** `http://localhost:4200` in your browser.

## Run with Docker

1. **Build the image**

   ```bash
   docker build -t vehicle-lookup .
   ```

2. **Run the container**

   ```bash
   docker run --rm -p 8080:8080 vehicle-lookup
   ```

3. **Open** `http://localhost:8080` in your browser.

## Project Structure

```
vehicle-lookup/
├── VehicleLookup.Api/          # .NET 8 Minimal API
│   ├── Models/                 # vPIC response DTOs
│   ├── Services/               # Typed HttpClient with caching
│   └── Program.cs              # Endpoints and middleware
├── client/                     # Angular 21 SPA
│   ├── src/app/
│   │   ├── vehicle.service.ts  # API client
│   │   ├── app.ts              # Cascade component (signals)
│   │   └── app.html            # Template with typeahead + filters
│   └── proxy.conf.json         # Dev proxy → API
├── Dockerfile                  # Multi-stage: Node → .NET SDK → runtime
├── .dockerignore
└── global.json                 # Pins .NET 8 SDK
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/makes` | All car makes (cached 24h) |
| GET | `/api/makes/{makeId}/vehicle-types` | Vehicle types for a make |
| GET | `/api/makes/{makeId}/models?year={year}&vehicleType={type}` | Models filtered by year and optional vehicle type |

## Design Decisions

- **Server-side caching:** The 12,000+ makes list is cached for 24 hours via `IMemoryCache` to avoid repeated calls to the external API.
- **Client-side filtering:** The makes dropdown filters the cached list locally and caps results at 50 to keep the DOM lightweight.
- **Single container:** Angular's production build is served from `wwwroot` by the same .NET process — one image, one port, no CORS.
- **Dev proxy:** Angular's dev server proxies `/api` requests to the .NET backend, matching the production same-origin topology.