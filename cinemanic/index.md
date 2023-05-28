# Application Business Logic

## Ticket Buying

The process of purchasing tickets in the application involves the following steps:

1. The user visits the screening view (`/seanse`).
2. After clicking the button displaying the screening hour on the right side of each screening, the user is sent to the payment form.
3. The user can choose a seat and pricing type on the payment form. Any changes to the form trigger a client-side request to the server to retrieve the updated ticket price, which is then displayed at the bottom.
4. The select box for selecting a seat number checks if other users haven't already bought a ticket for that seat.
5. The ticket price depends on various factors, such as the release date of the movie, whether it is for mature audiences, whether it is in 3D, the proximity of the seat to the back of the room, and the remaining number of seats.
6. After accepting the form, the user is redirected back to the screening view, and the selected ticket is added to the shopping cart. The number of tickets in the cart is visible on the navbar cart icon. The number is retrieved on the client-side from the server.
7. Tickets can be removed from the shopping cart by clicking the "Remove Ticket" button.
8. If someone pays for a ticket for a specific seat, all unpaid tickets for that seat are removed from all users.
9. Orders have three states: PENDING, SUBMITTED, and COMPLETED.
10. If a user has no tickets in the shopping cart and adds a ticket, a new order is created with the PENDING status. Each user can only have one PENDING order.
11. Shopping cart displays the PENDING order for the user
12. If there is a movie for adults in the shopping cart, information about mature content will appear at the bottom.
13. The user's date of birth is stored in the authorization cookie `_auth`. If a minor tries to pay for an order containing mature content, they will be presented with a warning view.
14. After clicking "Submit" in the shopping cart, the order status changes to SUBMITTED, and the user is redirected to the Stripe payment page.
15. If the user doesn't complete the payment, they can finish the payment process in the accounts view, specifically the "Orders" accordion tab.
16. After successfully paying for the order, the user is redirected to the `/platnosc/potwierdzenie` view, which displays all the purchased tickets.
17. Upon redirecting to this view, the PaymentController retrieves the session ID from Stripe to safely retrieve the order ID, preventing unauthorized access to the endpoint or providing a custom order ID.
18. Active tickets are accessible in the accounts view, specifically the "Bilety" accordion tab.

## Ticket and Screening Archivization

The archivization process for tickets and screenings is as follows:

1. 15 seconds after the application starts, the `TicketArchiveHostedService` proceeds to archive all screenings and tickets with a `ScreeningDate` older than the current time.
2. Old tickets are first added to the archived tables, and after saving them (to avoid interference with related tables by Entity Framework), they are removed from the original table.
3. Tickets that were not purchased are deleted during this process by checking the `IsActive` property.
4. The service runs every 24 hours.

## Decrementing Seats Left on Ticket Creation

The `AlterTicketTriggerDecrementSeats` migration adds a trigger that decrements the number of available seats for a given screening. The trigger works for seeded tickets, but an update to the trigger is needed to handle cases when tickets are bought.

## Updating Order Total Price on Ticket Creation

The `AlterTicketTrigger` migration adds a trigger that calculates the total price of the order after a ticket is added to the shopping cart (on insert to the `tickets` table).
