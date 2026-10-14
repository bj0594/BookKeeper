# API

GET /books
Request: none
Response: 200 OK + array of all books, each with the same fields as in the POST response

POST /books
Request: { "title": "Dune", "author": "Frank Herbert" }
Response: 201 Created + { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "title": "Dune", "author": "Frank Herbert", "isLoaned": false }

PATCH /books/{id}/loan
Request: none
Response: 204 No Content
Errors: 409 Conflict (book is already loaned)

PATCH /books/{id}/return
Request: none
Response: 204 No Content
Errors: 409 Conflict (book is not loaned)

DELETE /books/{id}
Request: none
Response: 204 No Content