# <ins>Design considerations</ins>

## **Acquiring bank response**

When the acquiring bank processes our payment, whether it is authorised or not, it is returning a **200 OK**. I have decided to return a **201 Created** from our API to signal the creation of a new resource, since we are adding the payment to our internal data store, regardless of authorisation status.

When the acquiring bank rejects our payment request, possible due to a malformed request, a **400 Bad Request** is returned. In our API response, I have decided to create a CustomHttpError that we return which can be used to provide some more information to the client, rather than just a status code.

## Validation
I have decided to use the FluentValidation library to perform validation of the PostPaymentRequest. FluentValidation allows you to create simple rules which can be placed in a separate class to de-clutter the controller class. The FluentValidation syntax is also very readable and makes it easy to modify exists rules or add new ones.

I have decided to wrap the validation in a validation middleware which will wrap any validation errors together and provide a **400 Bad Request** response with a neat body that can be easily parsed by any consuming APIs. 

## Further improvements

1. **Logging**:
   - Due to time constraints, logging was not added. In a production scenario, however it would be necessary to include logging to help debug issues that may arise. I have wrapped calls to external resources, such as the call to the acquiring bank, in a try catch, allowing the catch block to log the inner exception.
   

2. **Security**:
   - There is currently no authentication required to access these endpoints. In a production scenario, we would likely require some sort of authentication, such as a JWT token that would be passed in the Authorization header. This JWT token could then have roles included with it to provide further checks on whether a user has the right permissions to perform a specific action.
   - Currently, we are storing the whole card number in our internal data store, which can pose as a large compliance risk. Best practice would be to encrypt sensitive details, such as credit card details, before storing them into our data store.


3. **Performance**:
   - In order to improve the performance of our API, we could incorporate a cache, particular on our GET GetPaymentAsync endpoint. This would reduce the need to check our data store for the payment details if the specific ID exists in our cache.