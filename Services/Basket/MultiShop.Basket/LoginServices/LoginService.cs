namespace MultiShop.Basket.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserId => _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value; // sub is the claim type for user id in JWT token
    }

}
