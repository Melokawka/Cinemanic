# Cinemanic
ASP .NET Core MVC app for an independent cinema.

This application serves more as a tech-demo rather than a full-fledged cinema management system.

Business logic documentation and API documentation created with the help of DocFX can be found here: https://melokawka.github.io/Cinemanic/

<br/>

This application focuses on backend. Its purpose is for me to learn ASP .NET MVC Framework and to test: 
* Wordpress API (managing and creating news about the cinema), 
* Stripe API (payment system),
* The Movie Database API (randomly retrieving movie information),
* Docker (simplifying app deployment)

<br/>

<details>
  <summary>Used technologies</summary><br/>

  * ASP .NET 6.0 MVC Framework
  * MSSQL
  * Docker 
  * Stripe payment system API
  * Wordpress server
  * The Movie Database (TMDb) API
  * DocFX documentation generator
  <br/>

</details>

<details>
    <summary>Use-case UML diagram</summary>
    <img src="./docs/images/usecase-final.png" alt="ss" />
</details>

<details>
    <summary>ERD diagram</summary>
    <img src="./docs/images/ERD-final.png" alt="ss" />
</details>

<details>
  <summary>Functionalities</summary>

  <br/>
  
  * The app homepage retrieves (on the client side) and displays posts from the Wordpress server. The Wordpress server is used as a Content Management System for a more convenient way to manage news about the cinema.
  
  * The movies displayed on the homepage are randomly retrieved from the TMDB API (the amount of retrieved movies can be modified at the top of the `./Data/MovieService GetMovies()` method).
  
  * The user can submit an order for multiple tickets in one order. The selected tickets can be checked in the shopping cart page.
  
  * The shopping cart is connected to Stripe API (test mode) and allows for testing payments by credit card.
  
  * The number of tickets in the shopping cart can be checked on the navbar (it's retrieved on the client side from `/koszyk/liczba-produktow` endpoint).
  
  * The user can sign up for the newsletter by clicking Newsletter tab on the navbar after logging in.
  
  * The admin can generate and download a CSV of newsletter clients' emails.
  
    <details>
      <summary>The CSV looks like this:</summary>
      <img src="./docs/images/csv-example.png" alt="CSV Example" />
    </details>
  
  * The admin can generate and download a PDF with a report on the past year's financial results.
  
    <details>
        <summary>The PDF looks like this:</summary>
        <img src="./docs/images/pdf-example.png" alt="PDF Example" />
    </details>
  
<br/>

</details>

<details>
  <summary>Views</summary>

  <br/>
  
  * Posts at the bottom of the homepage:
  
  ![ss](./docs/images/home-admin-3.png)
  
  * Posts on the Wordpress page:
  
  ![ss](./docs/images/posts-wordpress.png)
  
  * Creating posts on the Wordpress server:
  
  ![ss](./docs/images/posts-wordpress-add.png)
  
  * Movie information displayed on the homepage:
  
  ![ss](./docs/images/home-admin-movie-info.png)
  
  * Screening creation for an admin:
  
  ![ss](./docs/images/screenings-admin-create.png)
  
  * Shopping cart view:
  
  ![ss](./docs/images/shopping-cart-user.png)
  
  * Stripe payment by card:
  
  ![ss](./docs/images/shopping-cart-user-stripe.png)
  
  * Payment confirmation view:
  
  ![ss](./docs/images/shopping-cart-user-payment-confirmation.png)
  
  * Stripe dashboard:
  
  ![ss](./docs/images/shopping-cart-user-stripe-dashboard.png)
  
  * Shopping cart icon:
  
  ![ss](./docs/images/navbar-user-cart-items.png)
  
</details>


<details>
  <summary>Installation</summary>

  <br/>

  1. You require Docker Desktop app in Linux Containers mode.
  
  2. Make sure your client has free 1433 (database) and 8080 (Wordpress) ports.
  
  3. Run the `create-db.bat` script from `./docker scripts`. It will create a container for MSSQL database for this app.
  
  4. Run `wordpress.bat`. It creates containers for the Wordpress server and the MySQL database for it too.
  
  5. After waiting for around 30 seconds, visit `localhost:8080` in order to install Wordpress. The installation is trivial.
  
  6. Run `wordpress-after-install.bat` and then `wordpress-after-install2.bat`.
  
  7. Go to `localhost:8080/wp-admin` -> `Settings` -> `Permalinks`. Make sure `%postname%` tag is selected (otherwise wp-json endpoint won't work).
  
  8. Install `WordPress REST API Authentication` by miniOrange from the `Plugins` tab.
  
  9. Select `Configure` under the newly installed plugin in the `Plugins` tab. Activate JWT token authorization and save the generated token.
  
  10. Insert the token for Wordpress API in `./cinemanic/appsettings.secrets.json` of the project. This is required for accessing the Wordpress API for retrieving posts and uploading media.
  
  11. Ensure the containers are running, then you can go ahead and run the project in Visual Studio (use the default `cinemanic` launch option, as the Docker launch option may not work).
  
</details>

