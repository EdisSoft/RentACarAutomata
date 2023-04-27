using FunctionsCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FunctionsCore.Services
{
    public class HTTPRequestService : IHTTPRequestService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private CRMRequestOptions options;

        public HTTPRequestService(HttpClient httpClient, IConfiguration configuration)
        {
            options = configuration.GetSection(nameof(CRMRequestOptions)).Get<CRMRequestOptions>();

            this.httpClient = httpClient;

            SetAuthorization();            
        }

        private void SetAuthorization()
        {
            var authenticationString = $"{options.UserName}:{options.Password}";
            var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        }

        public async Task<List<FoglalasokModel>> GetFoglalasok(string nev)
        {
            var responseString = await httpClient.GetStringAsync(options.RequestBase + "?action=pickup&mainparam=" + nev);
            return JsonConvert.DeserializeObject<List<FoglalasokModel>>(responseString);
        }
    }
}
