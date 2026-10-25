# Specification – BookKeeper
## 1 The assignment in my own words
A library system for librarians that keeps track of which books the library has and which of them are loaned out.
The user can add, list, loan out, return and delete books.
The domain logic is built with TDD and then made available through a REST API.
Assignment 2: the API runs in Docker, stores the books in PostgreSQL through EF Core, and the whole system starts with Docker Compose (API, PostgreSQL, pgAdmin and Traefik), with a health check. No new functionality.
Assignment 3: the API now provides registration and login, and can create users, control passwords and generate JWT-tokens. The tokens are used to identify the user and limit data access. There are two types of users: librarians and library users.
Delivered as a GitHub repo with tests and a README.

## 2 Requirements
Main flow;
A user creates an account, chooses to be a librarian or library user, logs in.

If librarian;
Adds a book, sees it in the list, deletes it.

I library user;
Sees a book in the list, loans it, sees a list of their loans, returns it.

Core (MUST):
- Add a book        ✔ AddBook_ValidBook_AddsBookToLibrary, PostBooks_ValidBook_ReturnsCreatedBook
- List all books    ✔ AddBook_ValidBook_AddsBookToLibrary (via GetBooks), GetBooks_AfterBookAdded_ReturnsOkWithList
- List own loans 
- Loan a book       ✔ LoanBook_ExistingBook_BecomesLoaned, PatchBooksLoan_ExistingBook_ReturnsNoContent
- Return a book     ✔ ReturnBook_ExistingLoanedBook_BecomesNotLoaned, PatchBooksReturn_LoanedBook_ReturnsNoContent
- Delete a book     ✔ DeleteBook_ExistingBook_RemovesBookFromLibrary, DeleteBooks_ExistingBook_ReturnsNoContent
- REST API          ✔ BooksApiTests (every line in api.md)

Rules:
- Can't loan a book that's already loaned out  ✔ LoanBook_AlreadyLoaned_ThrowsException, PatchBooksLoan_AlreadyLoaned_ReturnsConflict
- Can't return a book that hasn't been loaned  ✔ ReturnBook_NotLoanedBook_ThrowsException, PatchBooksReturn_NotLoaned_ReturnsConflict
- More than one librarian can't be created
- Only a librarian can add or delete books
- Only a library user can loan and return books
- Can't delete a loaned out book 
- Both the librarian and library users can see the list of books available

Ownership rules;
- A user can only view their own loans
- A user can't retrieve, alter or delete other user's loans
- Endpoints that require authorization can't be used without a valid token (401)

## 3 Flow
![Flow](../Excalidraw/flow.png)

A user logs in, sees all books, picks an action, and the list is updated.

## 4 Interface
```
BookService
- IReadOnlyList<Book> GetBooks()            -> all books
- Book AddBook(string title, string author) -> new book, IsLoaned = false
- void LoanBook(Guid id)                    -> book is loaned (IsLoaned = true)
                                               already loaned: InvalidOperationException
- void ReturnBook(Guid id)                  -> book is available (IsLoaned = false)
                                               not loaned: InvalidOperationException
- void DeleteBook(Guid id)                  -> book is removed

Book: Id (Guid), Title (string), Author (string), IsLoaned (bool)
Example: 3f2c..., "The Great Gatsby", "F. Scott Fitzgerald", false
```

Dependencies: IBookRepository
- EfBookRepository (BookKeeper.Infrastructure) stores the books in PostgreSQL. Used by the API.
- FakeBookRepository (BookKeeper.Core) keeps the books in memory. Used by the domain tests and the API tests.

## 5 Test list
- [x] A new book can be added
- [x] A book can be marked as loaned
- [x] An already loaned book cannot be loaned
- [x] A book can be marked as returned/made available
- [x] A book cannot be returned if it wasn't already loaned
- [x] A book can be deleted

Later questions:
- [x] Can a user loan several books?
- [x] Can the library add multiple copies of the same book?

## 6 Questions, assumptions and decisions
- Clarified: The repo will be used for the coming assignments
- Assumption: "4–5 tests" in the grading means domain tests only (part 2 of the assignment asks for 4–5 tests for the domain logic)
- Decision: 6 domain tests instead of 4–5. The assignment allows more tests when needed for the main rules of the MVP, and each of the two rules has its own test
- Decision: Returning a book that is not loaned gives an error (InvalidOperationException)
- Decision: The option to loan several books and/or add multiple copies of the same book is not essential to the MVP. It will not be added at this time, but might be in the future
- Decision: Unknown id is not handled in the MVP, because the user always picks a book from the list
- Clarified: The instructor requires IBookRepository and FakeBookRepository before assignment 2, added as a refactor. BookService only depends on IBookRepository
- Decision: No handling of storage failure. The assignment does not require it. If the database is down, the API returns 500, and `/health` reports `Unhealthy`
- Decision (assignment 2): EF Core with PostgreSQL, in its own project (BookKeeper.Infrastructure), so BookKeeper.Core does not know about the database
- Decision (assignment 2): BookService calls IBookRepository.Update after loan and return, so the change is saved in the database
- Decision (assignment 2): The API runs the migrations when it starts in Docker Compose (`Database__MigrateOnStartup`), so the tables are created without manual steps
- Decision (assignment 2): The health check also checks the database connection, so `/health` is `Unhealthy` when the API cannot reach PostgreSQL
- Decision (assignment 2): Traefik is the ingress. It routes all traffic on port 80 to the API
- Clarified (assignment 2): The Dockerfile is in BookKeeper.Api/, and docker-compose.yaml is in the repository root, as in the instructor's example solution
- Decision (assignment 2): The database and pgAdmin passwords are in docker-compose.yaml. They are only for local development

- Clarified (assignment 3): Only the assignment text applies. The grading criteria at the bottom of the assignment, the guide and the deadline are not taken into account
- Decision (assignment 3): The JWT secret is in appsettings.json (the assignment asks for it)
- Decision (assignment 3): Another user's data gives 403 Forbidden (most accurately represents the user experience)
- Decision (assignment 3): Scalar stays as the API reference, also for the token (already used in the project)
- Decision (assignment 3): Two types of users, librarian and library user (roles). Data that belongs to a user still has a user id

- Limitation: An unknown id gives 409 Conflict on loan and return, and 500 on delete (no unknown id handling in the MVP)
- Limitation: An empty title or author is accepted
- Limitation: The system does not track who has borrowed a book
- Limitation: Every operation on one book loads all books from the database (`GetAll`), which is fine for a small library

- Decision (assignment 3): Docker stays as it is (not part of this assignment)
- Decision (assignment 3): No CI (not required)
