using System.ComponentModel.DataAnnotations;

namespace BookKeeper.Api.Dtos;

// The JSON body for POST /auth/register. Role is Librarian or LibraryUser (see Roles).
public record RegisterRequest([Required] string Username, [Required] string Password, [Required] string Role);
