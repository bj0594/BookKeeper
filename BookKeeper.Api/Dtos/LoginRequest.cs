using System.ComponentModel.DataAnnotations;

namespace BookKeeper.Api.Dtos;

// The JSON body for POST /auth/login.
public record LoginRequest([Required] string Username, [Required] string Password);
