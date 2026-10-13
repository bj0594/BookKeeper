# Specification – BookKeeper
## 1 The assignment in my own words
A small system made into a REST API. TDD used to establish domain logic, then made available through REST API. 

## 2 Requirements
Main flow: A user adds a book, sees it in the list, loans it, returns it and deletes it.
Core (MUST):
- Add a book
- List all books
- Loan a book
- Return a book
- Delete a book
- REST API
Rules:
- Can't loan a book that's already loaned out

## 3 Flow
![Flow](Excalidraw/flow.png)

A user sees all books, picks an action, and the list is updated.
A loaned book cannot be loaned again

## 4 Interface
BookService
- List<Book> GetBooks()         -> all books
- Book AddBook(string title, 
  string author)                -> new book, IsLoaned = false
- void LoanBook(Guid id)        -> book is loaned (IsLoaned = true)
                                   already loaned: InvalidOperationException
                                   unknown id: KeyNotFoundException
- void ReturnBook(Guid id)      -> book is available (IsLoaned = false)
                                   not loaned: InvalidOperationException
                                   unknown id: KeyNotFoundException
- void DeleteBook(Guid id)      -> book is removed
                                   unknown id: KeyNotFoundException
                                          
Book: Id (Guid), Title (string), Author (string), IsLoaned (bool)
Example: 3f2c..., "The Great Gatsby", "F. Scott Fitzgerald", false

Dependencies: storage

## 5 Test list
- [x] A new book can be added
- [ ] A book can be marked as loaned
- [ ] An already loaned book cannot be loaned
- [ ] A book can be marked as returned/made available
- [ ] A book can be deleted
Later questions:
- [ ] Can a user loan several books?
- [ ] Can the library add multiple copies of the same book?

## 6 Questions, assumptions and decisions
I have chosen to make a small system for managing a library. The user can add, loan out, return, delete and list books. Delivered as a GitHub repo with test and a README.
- Clarified: The repo will be used for the coming assignments
- Decision: Returning a book that is not loaned gives an error (InvalidOperationException)
- Clarified: "4–5 tests" in the grading means domain tests only

- New project (no existing code)
- Web API: yes (parts 3–5)
- Database: no (not part of the MVP)
- CI: no (not required)