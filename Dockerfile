FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Teachio.sln ./
COPY Directory.Build.props ./
COPY Teachio.BLL/Teachio.BLL.csproj Teachio.BLL/
COPY Teachio.DAL/Teachio.DAL.csproj Teachio.DAL/
COPY Teachio.WebApi/Teachio.WebApi.csproj Teachio.WebApi/
COPY Teachio.XUnitTests/Teachio.XUnitTests.csproj Teachio.XUnitTests/
COPY Teachio.XIntegrationTests/Teachio.XIntegrationTests.csproj Teachio.XIntegrationTests/

RUN dotnet restore Teachio.sln -p:NuGetAudit=false

COPY . .
RUN dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Teachio.WebApi.dll"]
