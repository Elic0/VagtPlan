FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY VagtPlan.ApiService/ApiService.csproj VagtPlan.ApiService/
COPY VagtPlan.ServiceDefaults/VagtPlan.ServiceDefaults.csproj VagtPlan.ServiceDefaults/

RUN dotnet restore VagtPlan.ApiService/ApiService.csproj

# Copy everything else and publish
COPY . .
WORKDIR /src/VagtPlan.ApiService
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:3000
EXPOSE 3000
ENTRYPOINT ["dotnet", "ApiService.dll"]
