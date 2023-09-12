FROM mcr.microsoft.com/dotnet/sdk:6.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy everything else and build
COPY . ./

# Copy csproj and restore as distinct layers
#COPY *.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore

RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

EXPOSE ${PORT:-5000}
CMD ASPNETCORE_URLS=http://+:${PORT:-5000} dotnet ./out/HousingFinanceInterimApi.dll
