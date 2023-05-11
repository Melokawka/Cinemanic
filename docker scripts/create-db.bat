@echo off

REM Start the container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=A&VeryComplex123Password" -p 1433:1433 --name cinemanic_mssql -d mcr.microsoft.com/mssql/server:2022-latest

REM Wait for 10 seconds
timeout /t 10

REM Create the database
docker exec -it cinemanic_mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "A&VeryComplex123Password" -Q "CREATE DATABASE cinemanic"
