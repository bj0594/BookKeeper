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
}