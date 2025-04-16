FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and restore as distinct layers

COPY ./HousingFinanceInterimApi.sln ./
COPY ./HousingFinanceInterimApi/HousingFinanceInterimApi.csproj ./HousingFinanceInterimApi/
COPY ./HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj ./HousingFinanceInterimApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./HousingFinanceInterimApi/HousingFinanceInterimApi.csproj
RUN dotnet restore ./HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build ./HousingFinanceInterimApi.Tests/HousingFinanceInterimApi.Tests.csproj

CMD dotnet test
