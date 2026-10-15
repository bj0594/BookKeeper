# Specification – BookKeeper
## 1 The assignment in my own words
A small system made into a REST API. TDD used to establish domain logic, then made available through REST API. 

## 2 Requirements
Main flow: A user adds a book, sees it in the list, loans it, returns it and deletes it.

Core (MUST):
- Add a book        ✔ AddBook_ValidBook_AddsBookToLibrary, PostBooks_ValidBook_ReturnsCreatedBook
- List all books    ✔ AddBook_ValidBook_AddsBookToLibrary (via GetBooks), GetBooks_AfterBookAdded_ReturnsOkWithList
- Loan a book       ✔ LoanBook_ExistingBook_BecomesLoaned, PatchBooksLoan_ExistingBook_ReturnsNoContent
- Return a book     ✔ ReturnBook_ExistingLoanedBook_BecomesNotLoaned, PatchBooksReturn_LoanedBook_ReturnsNoContent
- Delete a book     ✔ DeleteBook_ExistingBook_RemovesBookFromLibrary, DeleteBooks_ExistingBook_ReturnsNoContent
- REST API          ✔ BooksApiTests (every line in api.md)

Rules:
- Can't loan a book that's already loaned out  ✔ LoanBook_AlreadyLoaned_ThrowsException, PatchBooksLoan_AlreadyLoaned_ReturnsConflict
- Can't return a book that hasn't been loaned  ✔ ReturnBook_NotLoanedBook_ThrowsException, PatchBooksReturn_NotLoaned_ReturnsConflict

## 3 Flow
![Flow](../Excalidraw/flow.png)

A user sees all books, picks an action, and the list is updated.

- A loaned book cannot be loaned again
- A book that is not loaned cannot be returned

## 4 Interface
```
BookService
- List<Book> GetBooks()                     -> all books
- Book AddBook(string title, string author) -> new book, IsLoaned = false
- void LoanBook(Guid id)                    -> book is loaned (IsLoaned = true)
                                               already loaned: InvalidOperationException
- void ReturnBook(Guid id)                  -> book is available (IsLoaned = false)
                                               not loaned: InvalidOperationException
- void DeleteBook(Guid id)                  -> book is removed
                          
Book: Id (Guid), Title (string), Author (string), IsLoaned (bool)
Example: 3f2c..., "The Great Gatsby", "F. Scott Fitzgerald", false
```

Dependencies: none (books are kept in an in-memory list)

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
I have chosen to make a small system for managing a library. The user can add, loan out, return, delete and list books. Delivered as a GitHub repo with tests and a README.

- Clarified: The repo will be used for the coming assignments
- Clarified: "4–5 tests" in the grading means domain tests only
- Decision: 6 domain tests instead of 4–5. The assignment allows more tests when needed for the main rules of the MVP, and each of the two rules has its own test
- Decision: Returning a book that is not loaned gives an error (InvalidOperationException)
- Decision: The option to loan several books and/or add multiple copies of the same book is not essential to the MVP. It will not be added at this time, but might be in the future
- Decision: Unknown id is not handled in the MVP, because the user always picks a book from the list
- Decision: No repository. The assignment does not require one, and the service's in-memory list is enough for the MVP
- Decision: No handling of storage failure. In-memory storage cannot fail, and the assignment does not require it

- Limitation: Data is lost when the API restarts (books are stored in memory)
- Limitation: An unknown id gives 409 Conflict on loan and return, and 500 on delete (no unknown id handling in the MVP)
- Limitation: An empty title or author is accepted
- Limitation: The system does not track who has borrowed a book

- New project (no existing code)
- Web API: yes (parts 3–5)
- Database: no (not part of the MVP)
- CI: no (not required)
