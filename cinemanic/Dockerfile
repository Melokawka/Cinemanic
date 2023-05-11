FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["cinemanic/cinemanic.csproj", "cinemanic/"]
RUN dotnet restore "cinemanic/cinemanic.csproj"
COPY . .
WORKDIR "/src/cinemanic"
RUN dotnet build "cinemanic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cinemanic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cinemanic.dll"]
