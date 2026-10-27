using System.ComponentModel.DataAnnotations;

namespace BookKeeper.Api.Auth;

public record RegisterRequest([Required] string Username, [Required] string Password);
