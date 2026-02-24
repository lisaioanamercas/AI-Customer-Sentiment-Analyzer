using MediatR;
using AISA.Application.Auth.DTOs;

namespace AISA.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
