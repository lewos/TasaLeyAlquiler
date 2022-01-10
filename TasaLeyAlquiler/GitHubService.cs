using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TasaLeyAlquiler
{
    public class GitHubService
    {
        private readonly GitHubClient _client;
        public GitHubService()
        {
            _client = new GitHubClient(new ProductHeaderValue("my-cool-app"));
        }


        public async Task<User> GetCurrentUser(string authToken) 
        {
            var tokenAuth = new Credentials(authToken);
            _client.Credentials = tokenAuth;

            var user = await _client.User.Current();

            return user;
        }

        public async Task<int?> CheckRate() 
        {
            // Prior to first API call, this will be null, because it only deals with the last call.
            var apiInfo = _client.GetLastApiInfo();

            // If the ApiInfo isn't null, there will be a property called RateLimit
            var rateLimit = apiInfo?.RateLimit;

            var howManyRequestsCanIMakePerHour = rateLimit?.Limit;
            var howManyRequestsDoIHaveLeft = rateLimit?.Remaining;
            var whenDoesTheLimitReset = rateLimit?.Reset; // UTC time

            return howManyRequestsDoIHaveLeft;
        }

        public async Task<Repository> GetRepo(long repoId)
        {
            return await _client.Repository.Get(repoId);
        }

        public async Task<Tuple<string,string>> GetFileContent(long repoId, string filePath)
        {
            var currentFileText = "";
            var contents = await _client.Repository.Content.GetAllContents(repoId, filePath);
            var targetFile = contents[0];
            if (targetFile.EncodedContent != null)
            {
                currentFileText = Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent));
            }
            else
            {
                currentFileText = targetFile.Content;
            }

            if (string.IsNullOrEmpty(currentFileText.Trim()))
                currentFileText = string.Format("{0}", $"#  Nueva Ley de Alquileres" + "\n" + 
                                                        $"## Ley Nº 27.551" + "\n" +
                                                        $"### Tasas" + "\n" + 
                                                        $"| Fecha | Tasa |" + "\n" + 
                                                        $"| ------------| -----:|");

            return new Tuple<string,string>(currentFileText, targetFile.Sha);
        }

        public async Task UpdateAndCommit(long repoId, string currentFileText, string fileName, Dictionary<string, string> dict, string branchToUpdate, string targetFileSha)
        {
            try
            {
                var newFileText = "";
                //TODO update si la tasa de algun dia cambio
                foreach (var k in dict)
                {
                    if (currentFileText.Contains($"|{k.Key}"))
                    {
                        Console.WriteLine($"{k.Key}");
                        //ver el valor de la tasa y ver si es diferente, hacer el update
                        var index = currentFileText.IndexOf($"|{k.Key} | ") + $"|{k.Key} | ".Length;
                        var lastIndex = currentFileText.Substring(index, 10).IndexOf('|');
                        var tasa = currentFileText.Substring(index, lastIndex);

                        Console.WriteLine($"{index};{lastIndex}{tasa}");

                        if (!tasa.Trim().Equals(k.Value))
                        {
                            currentFileText.Replace($"|{k.Key} | {tasa}|", $"|{k.Key} | {k.Value}|");
                        }

                    }


                    if (!currentFileText.Contains($"|{k.Key} | {k.Value}|"))
                    {
                        string newLine = $"|{k.Key} | {k.Value}|";
                        currentFileText = string.Format("{0}\n{1}", currentFileText, $"{newLine}");
                    }
                }


                var updateRequest = new UpdateFileRequest($"File update {DateTime.UtcNow.ToShortDateString()}", currentFileText, targetFileSha, branchToUpdate);

                var updatefile = await _client.Repository.Content.UpdateFile(repoId, fileName, updateRequest);
            }
            catch (Exception ex)
            {
                var algo = ex;
                throw;
            }
            

        }
    }
}
