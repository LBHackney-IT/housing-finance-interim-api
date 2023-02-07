FROM mcr.microsoft.com/dotnet/core/sdk:3.1

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./HousingFinanceInterimApi/HousingFinanceInterimApi.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./HousingFinanceInterimApi.csproj

# Copy everything else and build
COPY ./HousingFinanceInterimApi ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/HousingFinanceInterimApi.dll
