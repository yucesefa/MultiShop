using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MultiShop.DtoLayer.IdentityDtos.LoginDtos;
using MultiShop.WebUI.Services.Interfaces;
using MultiShop.WebUI.Settings;
using System.Security.Claims;

namespace MultiShop.WebUI.Services.Concrete
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value; // artık adresleri manuel olarak vermeyeceğiz
        }

        public async Task<bool> GetRefreshToken()
        {
            var discoveryEndPoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityServerUrl,
                Policy =
                    {
                        RequireHttps = false
                    }
            });


            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest
            {
                Address = discoveryEndPoint.TokenEndpoint,
                ClientId = _clientSettings.MultiShopManagerClient.ClientId,
                ClientSecret = _clientSettings.MultiShopManagerClient.ClientSecret,
                RefreshToken = refreshToken
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            var authenticationToken = new List<AuthenticationToken>()
                {

                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.AccessToken,
                        Value=token.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.RefreshToken,
                        Value=token.RefreshToken
                    },
                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.ExpiresIn,
                        Value=DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn).ToString()
                    }
                };

            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(); //tekrar authenticate ediyoruz

            var properties = result.Properties;
            properties.StoreTokens(authenticationToken);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Principal, properties); //yeni tokeni set ediyoruz
            return true;
        }

        public async Task<bool> SignIn(SignInDto signInDto)
        {
            var discoveryEndPoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityServerUrl,
                Policy =
                    {
                        RequireHttps = false
                    }
            });
            // Check if the discovery document was retrieved successfully
            var passwordTokenRequest = new PasswordTokenRequest
            {
                Address = discoveryEndPoint.TokenEndpoint,
                ClientId = _clientSettings.MultiShopManagerClient.ClientId,
                ClientSecret = _clientSettings.MultiShopManagerClient.ClientSecret,
                UserName = signInDto.Username,
                Password = signInDto.Password
            };
            // Check if the token request was successful
            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            // Check if the token was retrieved successfully
            var userInfoRequest = new UserInfoRequest
            {
                Address = discoveryEndPoint.UserInfoEndpoint,
                Token = token.AccessToken
            };

            var userValues = await _httpClient.GetUserInfoAsync(userInfoRequest);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userValues.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();

            // Set the authentication properties
            authenticationProperties.StoreTokens(new List<AuthenticationToken>(){

                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.AccessToken,
                        Value=token.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.RefreshToken,
                        Value=token.RefreshToken
                    },
                    new AuthenticationToken
                    {
                        Name=OpenIdConnectParameterNames.ExpiresIn,
                        Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString()
                    }
                });

            authenticationProperties.IsPersistent = false;

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);
            return true;
        }
    }
}
