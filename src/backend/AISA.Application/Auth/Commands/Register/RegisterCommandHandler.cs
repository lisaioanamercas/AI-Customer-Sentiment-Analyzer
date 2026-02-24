using MediatR;
using AISA.Application.Auth.DTOs;
using AISA.Application.Common.Interfaces;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;

namespace AISA.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Verifică dacă email-ul există deja
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Un cont cu acest email există deja.");
        }

        // Creează utilizatorul cu parola hash-uită
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmailConfirmed = true // Demo — nu avem SMTP
        };

        await _userRepository.CreateAsync(user);

        // Generează JWT
        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            BusinessProfileId = user.BusinessProfileId
        };
    }
}
