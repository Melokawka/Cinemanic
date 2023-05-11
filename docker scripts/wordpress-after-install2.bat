docker exec wordpress wp rewrite structure %postname%/ --allow-root

REM Install and enable the Application Passwords plugin
REM docker exec -it wordpress wp plugin install application-passwords --activate --allow-root

REM visit to enable wordpress api http://localhost:8080/wp-admin/options-permalink.php