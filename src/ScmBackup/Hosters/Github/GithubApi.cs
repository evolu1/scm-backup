﻿using Newtonsoft.Json;
using ScmBackup.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Security.Authentication;

namespace ScmBackup.Hosters.Github
{
    /// <summary>
    /// Calls the GitHub API
    /// </summary>
    internal class GithubApi : IGithubApi
    {
        private readonly IHttpRequest request;
        private readonly ILogger logger;

        public HttpResult LastResult { get; private set; }

        public GithubApi(IHttpRequest request, ILogger logger)
        {
            this.request = request;
            this.logger = logger;
        }

        public List<HosterRepository> GetRepositoryList(ConfigSource config)
        {
            var list = new List<HosterRepository>();
            string className = this.GetType().Name;

            // https://developer.github.com/v3/#schema
            request.SetBaseUrl("https://api.github.com");

            // https://developer.github.com/v3/#current-version
            request.AddHeader("Accept", "application/vnd.github.v3+json");

            // https://developer.github.com/v3/#user-agent-required
            request.AddHeader("User-Agent", Resource.GetString("AppTitle"));
            
            bool isAuthenticated = !String.IsNullOrWhiteSpace(config.AuthName) && !String.IsNullOrWhiteSpace(config.Password);
            if (isAuthenticated)
            {
                // https://developer.github.com/v3/auth/#basic-authentication
                request.AddBasicAuthHeader(config.AuthName, config.Password);
            }

            string url = string.Empty;
            switch (config.Type.ToLower())
            {
                case "user":

                    if (isAuthenticated)
                    {
                        // https://developer.github.com/v3/repos/#list-your-repositories
                        url = "/user/repos";
                    }
                    else
                    {
                        // https://developer.github.com/v3/repos/#list-user-repositories
                        url = string.Format("/users/{0}/repos", config.Name);
                    }
                    break;

                case "org":

                    throw new NotImplementedException();

            }

            this.logger.Log(ErrorLevel.Info, Resource.GetString("ApiGettingUrl"), className, request.HttpClient.BaseAddress.ToString() + url);
            this.LastResult = request.Execute(url).Result;

            if (this.LastResult.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<List<GithubApiResponse>>(this.LastResult.Content);
                foreach (var apiRepo in apiResponse)
                {
                    var repo = new HosterRepository(apiRepo.full_name, apiRepo.clone_url, ScmType.Git);
                    list.Add(repo);
                }
            }
            else
            {
                switch (this.LastResult.Status)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new AuthenticationException(string.Format(Resource.GetString("GithubApiAuthenticationFailed"), config.AuthName));
                    case HttpStatusCode.Forbidden:
                        throw new SecurityException(Resource.GetString("GithubApiMissingPermissions"));
                    case HttpStatusCode.NotFound:
                        throw new InvalidOperationException(string.Format(Resource.GetString("GithubApiInvalidUsername"), config.Name));
                }
            }

            return list;
        }
    }
}