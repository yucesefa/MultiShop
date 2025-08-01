﻿using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Services.Interfaces;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI.Services.Concrete
{
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly HttpClient _httpClient;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly ClientSettings _clientSettings;

        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, HttpClient httpClient, IClientAccessTokenCache clientAccessTokenCache, IOptions<ClientSettings> clientSettings)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _httpClient = httpClient;
            _clientAccessTokenCache = clientAccessTokenCache;
            _clientSettings = clientSettings.Value;
        }

        public async Task<string> GetToken()
        {
            var currentToken = await _clientAccessTokenCache.GetAsync("multishoptoken");
            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }
            var discoveryEndPoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityServerUrl,
                Policy =
                {
                    RequireHttps = false
                }
            });

            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discoveryEndPoint.TokenEndpoint,
                ClientId = _clientSettings.MultiShopVisitorClient.ClientId,
                ClientSecret = _clientSettings.MultiShopVisitorClient.ClientSecret,
            };

            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
            await _clientAccessTokenCache.SetAsync("multishoptoken", newToken.AccessToken,newToken.ExpiresIn);
            return newToken.AccessToken;
        }
    }
}
