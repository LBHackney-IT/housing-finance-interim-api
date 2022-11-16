FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./HousingFinanceInterimApi.sln ./
COPY ./HousingFinanceInterimApi/HousingFinanceInterimApi.csproj ./HousingFinanceInterimApi/
COPY ./HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj ./HousingFinanceInterimApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./HousingFinanceInterimApi/HousingFinanceInterimApi.csproj
RUN dotnet restore ./HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj

CMD dotnet test
