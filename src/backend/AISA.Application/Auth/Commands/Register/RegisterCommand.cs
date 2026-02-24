using MediatR;
using AISA.Application.Auth.DTOs;

namespace AISA.Application.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
