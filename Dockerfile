FROM node:22-alpine AS client
WORKDIR /client
COPY client/package*.json ./
RUN npm ci
COPY client/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS api
WORKDIR /src
COPY global.json ./
COPY VehicleLookup.Api/*.csproj VehicleLookup.Api/
RUN dotnet restore VehicleLookup.Api/VehicleLookup.Api.csproj
COPY VehicleLookup.Api/ VehicleLookup.Api/
RUN dotnet publish VehicleLookup.Api/VehicleLookup.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=api /app/publish ./
COPY --from=client /client/dist/client/browser ./wwwroot
EXPOSE 8080
ENTRYPOINT ["dotnet", "VehicleLookup.Api.dll"]
