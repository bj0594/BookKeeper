using Microsoft.AspNetCore.Mvc;
using BookKeeper.Core;
using System.ComponentModel;

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
    [EndpointDescription("Fill in title and author in the request body. The id in the response is used by loan, return and delete.")]
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
    [EndpointDescription("Returns 204 No Content. Send it twice for the same book to get 409 Conflict (book is already loaned).")]
    public IActionResult LoanBook([Description("Copy the id from the response to POST /books")] Guid bookId)
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
    [EndpointDescription("Returns 204 No Content. Send it for a book that is not loaned to get 409 Conflict (book is not loaned).")]
    public IActionResult ReturnBook([Description("Copy the id from the response to POST /books")] Guid bookId)
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
    public IActionResult DeleteBook([Description("Copy the id from the response to POST /books")] Guid bookId)
    {
        _bookService.DeleteBook(bookId);
        return NoContent();
    }
}