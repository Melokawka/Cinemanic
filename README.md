# Cinemanic
ASP .NET Core MVC app for an independent cinema.
The in-depth documentation created with the help of DocFX can be found here: https://melokawka.github.io/Cinemanic/

<br/>

The application focuses on the backend side. Its purpose is for me to learn ASP .NET MVC Framework and test: 
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
  * The Movie Database (TMDB) API
  * DocFX documentation generator
  <br/>

</details>

<details>
  <summary>Functionalities</summary>

  <br/>
  
  * The app homepage retrieves (on the client side) and displays posts from the Wordpress server. The Wordpress server is used as a Content Management System for a more convenient way to manage news about the cinema.
  
    <details>
        <summary>Posts at the bottom of the homepage look like this:</summary>
        <img src="./docs/images/home-admin-3.png" alt="ss" />
    </details>
  
    <details>
        <summary>Posts on the wordpress page look like this:</summary>
        <img src="./docs/images/posts-wordpress.png" alt="ss" />
    </details>
  
    <details>
        <summary>Posts on the wordpress page look like this:</summary>
        <img src="./docs/images/posts-wordpress-add.png" alt="ss" />
    </details>
  
  * The movies displayed on the homepage are randomly retrieved from the TMDB API (the amount of retrieved movies can be modified at the top of the `./Data/MovieService GetMovies()` method).
  
    <details>
        <summary>Movie informations are displayed like this:</summary>
        <img src="./docs/images/home-admin-movie-info.png" alt="ss" />
    </details>
  
  * The user can submit an order for multiple tickets in one order. The selected tickets can be checked in the shopping cart page.
  
    <details>
        <summary>Shopping cart view:</summary>
        <img src="./docs/images/shopping-cart-user.png" alt="ss" />
    </details>
  
  * The shopping cart is connected to Stripe API (test mode) and allows for testing payments by credit card.
  
    <details>
        <summary>Stripe payment by card looks like this:</summary>
        <img src="./docs/images/shopping-cart-user-stripe.png" alt="PDF Example" />
    </details>
  
    <details>
        <summary>Payment confirmation view:</summary>
        <img src="./docs/images/shopping-cart-user-payment-confirmation.png" alt="PDF Example" />
    </details>
  
    <details>
        <summary>Stripe dashboard looks like this:</summary>
        <img src="./docs/images/shopping-cart-user-stripe-dashboard.png" alt="PDF Example" />
    </details>
  
  * The number of tickets in the shopping cart can be checked on the navbar (it's retrieved on the client side from `/koszyk/liczba-produktow` endpoint).
  
    <details>
        <summary>Shopping cart icon:</summary>
        <img src="./docs/images/navbar-user-cart-items.png" alt="PDF Example" />
    </details>
  
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
  
</details>


<details>
  <summary>App structure on diagrams</summary>

  Dropdown menu content goes here.

</details>

<details>
  <summary>Installation</summary>

  Dropdown menu content goes here.

</details>
