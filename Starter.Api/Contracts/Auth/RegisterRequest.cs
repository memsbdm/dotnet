using System.ComponentModel.DataAnnotations;

namespace Starter.Api.Contracts.Auth;

public sealed record RegisterRequest(
    [Required]
    [EmailAddress]
    [StringLength(254)]
    string Email,

    [Required]
    [StringLength(32, MinimumLength = 8)]
    string Password);
