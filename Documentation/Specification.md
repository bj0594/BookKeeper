# Specification – BookKeeper

## 1 The assignment in my own words
A library system for librarians that keeps track of which books the library has and which of them are loaned out.
The user can add, list, loan out, return and delete books.
The domain logic is built with TDD and then made available through a REST API.
Assignment 2: the API runs in Docker, stores the books in PostgreSQL through EF Core, and the whole system starts with Docker Compose (API, PostgreSQL, pgAdmin and Traefik), with a health check. No new functionality.
Assignment 3: the API now provides registration and login, and can create users, control passwords and generate JWT-tokens. The tokens are used to identify the user and limit data access. There are two types of users: librarians and library users.
Delivered as a GitHub repo with tests and a README.

## 2 Requirements
Main flow (assignment 3):
A user creates an account, chooses to be a librarian or library user, logs in.

If librarian:
Adds a book, sees it in the list, deletes it.

If library user:
Sees an available book in the list, loans it, sees a list of their loans, returns it.

Core (MUST):
- Add a book        ✔ AddBook_ValidBook_AddsBookToLibrary, PostBooks_ValidBook_ReturnsCreatedBook
- List all books    ✔ AddBook_ValidBook_AddsBookToLibrary (via GetBooks), GetBooks_AfterBookAdded_ReturnsOkWithList
- List all available books (assignment 3)
- List own loans (assignment 3)
- Loan a book       ✔ LoanBook_ExistingBook_BecomesLoaned, PatchBooksLoan_ExistingBook_ReturnsNoContent
- Return a book     ✔ ReturnBook_ExistingLoanedBook_BecomesNotLoaned, PatchBooksReturn_LoanedBook_ReturnsNoContent
- Delete a book     ✔ DeleteBook_ExistingBook_RemovesBookFromLibrary, DeleteBooks_ExistingBook_ReturnsNoContent
- REST API          ✔ BooksApiTests (every line in api.md)

Rules:
- Can't loan a book that's already loaned out  ✔ LoanBook_AlreadyLoaned_ThrowsException, PatchBooksLoan_AlreadyLoaned_ReturnsConflict
- Can't return a book that hasn't been loaned  ✔ ReturnBook_NotLoanedBook_ThrowsException, PatchBooksReturn_NotLoaned_ReturnsConflict
- More than one librarian can't be created (assignment 3)
- Only a librarian can add or delete books (assignment 3)
- Only a library user can loan and return books (assignment 3)
- Can't delete a loaned out book (assignment 3)
- A librarian can see the whole list of every book (assignment 3)
- A library user can only see available books (assignment 3)

Ownership rules:
- A user can only view their own loans (assignment 3)
- A user can't return another user's loan (assignment 3)
- Endpoints that require authorization can't be used without a valid token (401) (assignment 3)

## 3 Flow
![Flow](../Excalidraw/flow.png)

A user registers with a role and logs in. A librarian sees all books and adds or deletes one. A library user sees the available books, loans one, sees their loans and returns it.

## 4 Interface
```
BookService
- IReadOnlyList<Book> GetBooks()                   -> all books
- IReadOnlyList<Book> GetAvailableBooks()          -> books that are not loaned (LoanedByUserId is empty)
- Book AddBook(string title, string author)        -> new book, not loaned
- void LoanBook(Guid id, Guid userId)              -> book is loaned by the user (LoanedByUserId = userId)
                                                      already loaned: InvalidOperationException
- IReadOnlyList<Book> GetLoans(Guid userId)        -> the books the user has loaned
- void ReturnBook(Guid id, Guid userId)            -> book is available (LoanedByUserId is empty)
                                                      not loaned: InvalidOperationException
                                                      another user's loan: UnauthorizedAccessException
- void DeleteBook(Guid id)                         -> book is removed
                                                      book is loaned: InvalidOperationException

Book: Id (Guid), Title (string), Author (string), LoanedByUserId (Guid?, empty when the book is available)
Example: 3f2c..., "The Great Gatsby", "F. Scott Fitzgerald", 2a6f...
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
- [x] A loan registers loanee
- [x] A loaned book can't be deleted
- [x] List only your own loans
- [x] The list of available books won't show loaned books
- [x] User can't return other's loans
- [ ] A returned book is no longer in the user's loan

Manual checks:
- [ ] Check if data is stored properly in PostgreSQL

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
- Decision (assignment 3): The role is chosen at registration, and only one librarian can be created (the system is a single library with one librarian)
- Decision (assignment 3): A book stores the id of the user who loaned it (LoanedByUserId). There is no separate loan type, because the flow only needs to know which user has each book
- Decision (assignment 3): LoanedByUserId replaces IsLoaned. A book is loaned when LoanedByUserId has a value, so the information is only stored one place
- Decision (assignment 3): BookService still only depends on IBookRepository

- Limitation: An unknown id gives 409 Conflict on loan and return, and 500 on delete (no unknown id handling in the MVP)
- Limitation: An empty title or author is accepted
- Limitation: Every operation on one book loads all books from the database (`GetAll`), which is fine for a small library

- Decision (assignment 3): Docker stays as it is (not part of this assignment)
- Decision (assignment 3): No CI (not required)
