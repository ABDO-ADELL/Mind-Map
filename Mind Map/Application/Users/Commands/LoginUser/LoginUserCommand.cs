namespace Mind_Map.Application.Users.Commands.LoginUser
{
    using MediatR;

    public class LoginUserCommand : IRequest<string?>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
