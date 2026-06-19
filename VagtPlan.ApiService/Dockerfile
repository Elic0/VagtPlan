FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VagtPlan.Web/VagtPlan.Web.csproj VagtPlan.Web/
COPY VagtPlan.ServiceDefaults/VagtPlan.ServiceDefaults.csproj VagtPlan.ServiceDefaults/

RUN dotnet restore VagtPlan.Web/VagtPlan.Web.csproj

COPY . .
WORKDIR /src/VagtPlan.Web
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*
COPY --from=build /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]