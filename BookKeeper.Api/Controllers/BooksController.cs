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
    public IActionResult Create(CreateBookRequest request)
    {
        var book = _bookService.AddBook(request.Title, request.Author);
        return Created($"/books/{book.Id}", book);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_bookService.GetBooks());
    }

    [HttpPatch("{bookId:guid}/loan")]
    public IActionResult LoanBook(Guid bookId)
    {
        _bookService.LoanBook(bookId);
        return NoContent();
    }

    [HttpPatch("{bookId:guid}/return")]
    public IActionResult ReturnBook(Guid bookId)
    {
        _bookService.ReturnBook(bookId);
        return NoContent();
    }
}