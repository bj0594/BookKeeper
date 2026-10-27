using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using BookKeeper.Core;

namespace BookKeeper.Api.Controllers;

// Connects the /books endpoints to BookService. The contract is in Documentation/api.md.
[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private const string BookIdDescription = "Copy the id from the response to POST /books";
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    [EndpointDescription("Fill in title and author in the request body. The id in the response is used by loan, return and delete.")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    public IActionResult AddBook(CreateBookRequest request)
    {
        var book = _bookService.AddBook(request.Title, request.Author);
        return Created($"/books/{book.Id}", book);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Book>), StatusCodes.Status200OK)]
    public IActionResult GetBooks()
    {
        return Ok(_bookService.GetBooks());
    }

    [HttpPatch("{bookId:guid}/loan")]
    [EndpointDescription("Send it twice for the same book to get 409 Conflict (book is already loaned).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public IActionResult LoanBook([Description(BookIdDescription)] Guid bookId)
    {
        try
        {
            _bookService.LoanBook(bookId, Guid.Empty); // replaced by the logged-in user's id in later commit
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // An already loaned book gives 409. An unknown id also ends up here, because it throws the same exception type.
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("{bookId:guid}/return")]
    [EndpointDescription("Send it for a book that is not loaned to get 409 Conflict (book is not loaned).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public IActionResult ReturnBook([Description(BookIdDescription)] Guid bookId)
    {
        try
        {
            _bookService.ReturnBook(bookId, Guid.Empty);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // A book that is not loaned gives 409. An unknown id also ends up here, because it throws the same exception type.
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{bookId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteBook([Description(BookIdDescription)] Guid bookId)
    {
        try
        {
            _bookService.DeleteBook(bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
