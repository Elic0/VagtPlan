FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY VagtPlan.Web/VagtPlan.Web.csproj VagtPlan.Web/
COPY VagtPlan.ServiceDefaults/VagtPlan.ServiceDefaults.csproj VagtPlan.ServiceDefaults/

RUN dotnet restore VagtPlan.Web/VagtPlan.Web.csproj

# Copy everything else and publish
COPY . .
WORKDIR /src/VagtPlan.Web
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "VagtPlan.Web.dll"]
