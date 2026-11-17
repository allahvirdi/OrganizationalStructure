FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=5197
ENV ASPNETCORE_URLS=http://+:5197
ENV ASPNETCORE_ENVIRONMENT=Production

COPY publish/ .

USER $APP_UID

EXPOSE 5197

ENTRYPOINT ["dotnet", "OrganizationalStructure.API.dll"]
