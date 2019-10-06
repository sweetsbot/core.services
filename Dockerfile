FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

COPY *.sln ./
COPY Core.Common/*.csproj ./Core.Common/
COPY Core.DataAccess/*.csproj ./Core.DataAccess/
COPY Core.Business/*.csproj ./Core.Business/
COPY Core.Services/*.csproj ./Core.Services/
COPY TestConsole/*.csproj ./TestConsole/

RUN dotnet restore
COPY . .

WORKDIR /app/Core.Common
RUN dotnet build -c Release -o /app/build

WORKDIR /app/Core.DataAccess
RUN dotnet build -c Release -o /app/build

WORKDIR /app/Core.Business
RUN dotnet build -c Release -o /app/build

WORKDIR /app/TestConsole
RUN dotnet build -c Release -o /app/build

WORKDIR /app/Core.Services
RUN dotnet build -c Release -o /app/build

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 as final
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS="https://*:5001"
EXPOSE 5001
ENTRYPOINT ["dotnet", "Core.Services.dll"]
