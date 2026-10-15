using Microsoft.AspNetCore.Mvc;
using BookKeeper.Core;

namespace BookKeeper.Api.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    public IActionResult AddBook(CreateBookRequest request)
    {
        var book = _bookService.AddBook(request.Title, request.Author);
        return Created($"/books/{book.Id}", book);
    }

    [HttpGet]
    public IActionResult GetBooks()
    {
        return Ok(_bookService.GetBooks());
    }

    [HttpPatch("{bookId:guid}/loan")]
    public IActionResult LoanBook(Guid bookId)
    {
        try
        {
            _bookService.LoanBook(bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("{bookId:guid}/return")]
    public IActionResult ReturnBook(Guid bookId)
    {
        try
        {
            _bookService.ReturnBook(bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{bookId:guid}")]
    public IActionResult DeleteBook(Guid bookId)
    {
        _bookService.DeleteBook(bookId);
        return NoContent();
    }
}