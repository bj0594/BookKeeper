# Main flow

What a librarian and a library user can do, and where the rules stop them. Replaces Excalidraw/flow.png.

```mermaid
flowchart TD
    start([User opens BookKeeper]) --> register[/Registers with username, password and role/]
    register --> taken{Is the username taken?}
    taken -- yes --> errTaken[/Error: username is taken/]
    errTaken --> register
    taken -- no --> librarian{Librarian chosen, and one already exists?}
    librarian -- yes --> errLibrarian[/Error: a librarian already exists/]
    errLibrarian --> register
    librarian -- no --> login[/Logs in with username and password/]
    login --> correct{Right username and password?}
    correct -- no --> errLogin[/Error: wrong username or password/]
    errLogin --> login
    correct -- yes --> role{Role?}

    role -- Librarian --> allBooks[/Sees all books, loaned ones marked/]
    allBooks --> librarianAction{What action?}
    librarianAction -- Add --> added[Book is added]
    added --> allBooks
    librarianAction -- Delete --> loanedDelete{Is the book loaned?}
    loanedDelete -- yes --> errDelete[/Error: book must be returned first/]
    errDelete --> allBooks
    loanedDelete -- no --> deleted[Book is deleted]
    deleted --> allBooks

    role -- LibraryUser --> available[/Sees the available books/]
    available --> userAction{What action?}
    userAction -- Loan --> loanedLoan{Is the book already loaned?}
    loanedLoan -- yes --> errLoan[/Error: book is already loaned/]
    errLoan --> available
    loanedLoan -- no --> loaned[Book is loaned by the user]
    loaned --> available
    userAction -- My loans --> myLoans[/Sees own loans/]
    myLoans --> own{Loaned by this user?}
    own -- no --> errOwn[/Error: not your loan/]
    errOwn --> myLoans
    own -- yes --> returned[Book is returned]
    returned --> myLoans

    librarianAction -- Log out --> loggedOut([Logged out])
    userAction -- Log out --> loggedOut```
