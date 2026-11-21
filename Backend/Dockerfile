FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrganizationalStructure.API/OrganizationalStructure.API.csproj", "OrganizationalStructure.API/"]
COPY ["OrganizationalStructure.Application/OrganizationalStructure.Application.csproj", "OrganizationalStructure.Application/"]
COPY ["OrganizationalStructure.Domain/OrganizationalStructure.Domain.csproj", "OrganizationalStructure.Domain/"]
COPY ["OrganizationalStructure.Infrastructure/OrganizationalStructure.Infrastructure.csproj", "OrganizationalStructure.Infrastructure/"]

RUN dotnet restore "OrganizationalStructure.API/OrganizationalStructure.API.csproj"

COPY . .
WORKDIR "/src/OrganizationalStructure.API"
RUN dotnet publish "OrganizationalStructure.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5197
ENV ASPNETCORE_HTTP_PORTS=5197
ENV ASPNETCORE_URLS=http://+:5197
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrganizationalStructure.API.dll"]
