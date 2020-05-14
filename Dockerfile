FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS builder
WORKDIR /sln

COPY . .

RUN dotnet build "./build" /nodeReuse:false
RUN dotnet run --project build --target Publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app

COPY --from=builder ./sln/output .

ENV EndpointRulesSettings__DbFilePath /middlerData/rules.db
ENV GlobalVariablesSettings__DbFilePath /middlerData/variables.db

EXPOSE 80
EXPOSE 443
EXPOSE 4444

ENTRYPOINT ["dotnet", "middlerApp.API.dll"]
