using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Api.Dtos;

public class RegisterRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = "";
}
