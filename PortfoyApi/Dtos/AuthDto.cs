namespace PortfoyApi.Dtos
{
    public class AuthDto
    {
        public class LoginDto
        {
            public string Email { get; set; } = default!;
            public string Password { get; set; } = default!;
        }

        public class AuthResponseDto
        {
            public string Token { get; set; } = default!;
            public DateTime Expiration { get; set; }
            public string Email { get; set; } = default!;
            public string? FullName { get; set; }
        }

        public class AuthRegisterDto
        {
            public string Email { get; set; } = default!;
            public string Password { get; set; } = default!;
            public string? FullName { get; set; }
        }
    }
}
