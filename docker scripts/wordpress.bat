@echo off

REM Remove old volumes
docker volume rm db_data wp_data

REM Wait for 2 seconds
timeout /t 2

REM Create the Docker network
docker network create localdev2

REM Start the database container
docker run -d --name db --network localdev2 -e MYSQL_ROOT_PASSWORD=admin -e MYSQL_DATABASE=cinemaposts -v db_data:/var/lib/mysql mysql:8.0.27 --default-authentication-plugin=mysql_native_password

REM Start the WordPress container
docker run -d --name wordpress --network localdev2 -e WORDPRESS_DB_HOST=db -e WORDPRESS_DB_USER=root -e WORDPRESS_DB_PASSWORD=admin -e WORDPRESS_DB_NAME=cinemaposts -v wp_data:/var/www/html -p 8080:80 wordpress:latest

REM Create the Docker volumes
docker volume create db_data
docker volume create wp_data


REM define( 'WP_ENVIRONMENT_TYPE', 'staging' );
