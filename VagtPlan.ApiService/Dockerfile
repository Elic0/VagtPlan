FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VagtPlan.ApiService/ApiService.csproj VagtPlan.ApiService/
COPY VagtPlan.ServiceDefaults/VagtPlan.ServiceDefaults.csproj VagtPlan.ServiceDefaults/

RUN dotnet restore VagtPlan.ApiService/ApiService.csproj

COPY . .
WORKDIR /src/VagtPlan.ApiService
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ApiService.dll"]