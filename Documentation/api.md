# API

```
POST /auth/register
Request: { "username": "anna", "password": "Secret-123", "role": "LibraryUser" }
Response: 201 Created + { "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7", "username": "anna" }
Errors: 400 Bad Request (role is not Librarian or LibraryUser)
        409 Conflict (username is taken)
        409 Conflict (a librarian already exists)

POST /auth/login
Request: { "username": "anna", "password": "Secret-123" }
Response: 200 OK + { "token": "eyJ…" }
Errors: 401 Unauthorized (wrong username or password)

GET /books
Auth: Bearer token
Request: none
Response: 200 OK + array of books, each with the same fields as in the POST response. Librarian: all books. LibraryUser: only the available books
Errors: 401 Unauthorized (no valid token)

POST /books
Auth: Bearer token (Librarian)
Request: { "title": "Dune", "author": "Frank Herbert" }
Response: 201 Created + { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "title": "Dune", "author": "Frank Herbert", "isLoaned": false, "loanedByUserId": null }
Errors: 401 Unauthorized (no valid token)
        403 Forbidden (not Librarian) 

GET /books/loans
Auth: Bearer token (LibraryUser)
Request: none
Response: 200 OK + array of the books the logged-in user has loaned, with the same fields as in the POST response
Errors: 401 Unauthorized (no valid token)
        403 Forbidden (not LibraryUser)

PATCH /books/{bookId}/loan
Auth: Bearer token (LibraryUser)
Request: none
Response: 204 No Content
Errors: 401 Unauthorized (no valid token)
        403 Forbidden (not LibraryUser)
        409 Conflict (book is already loaned)

PATCH /books/{bookId}/return
Auth: Bearer token (LibraryUser)
Request: none
Response: 204 No Content
Errors: 401 Unauthorized (no valid token)
        403 Forbidden (not LibraryUser)
        403 Forbidden (book is loaned by another user)
        409 Conflict (book is not loaned)

DELETE /books/{bookId}
Auth: Bearer token (Librarian)
Request: none
Response: 204 No Content
Errors: 401 Unauthorized (no valid token)
        403 Forbidden (not Librarian)
        409 Conflict (book is loaned)

GET /health
Request: none
Response: 200 OK + Healthy (the API can reach the database)
Errors: 503 Service Unavailable + Unhealthy (the API cannot reach the database)
```
