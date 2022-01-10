using Microsoft.Extensions.Configuration;
using System;

namespace TasaLeyAlquiler
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {

                var gitAuthToken = Environment.GetEnvironmentVariable("GH_TOKEN");
             
                var repoId = Environment.GetEnvironmentVariable("GH_REPO_ID");
                var branchToUpdate = Environment.GetEnvironmentVariable("GH_BRANCH");
                var fileName = Environment.GetEnvironmentVariable("GH_FILE_NAME");

                // Fecha de inicio
                var desde = new DateTime(2020, 07, 01);
                // Fecha de corrida del job
                var hasta = DateTime.UtcNow;

                var content = await ServiceBCRA.GetFechaYTasasAsync(desde, hasta);

                var dict = Parser.GetDictFromContent(content);

                var gitHubService = new GitHubService();

                var user = await gitHubService.GetCurrentUser(gitAuthToken);

                ////Console.WriteLine($"Request disponibles: {gitHubService.CheckRate()}");

                var repo = await gitHubService.GetRepo(Convert.ToInt64(repoId));

                var tuple = await gitHubService.GetFileContent(Convert.ToInt64(repoId), fileName);
                var currentFileText = tuple.Item1;
                var targetFileSha = tuple.Item2;

                await gitHubService.UpdateAndCommit(Convert.ToInt64(repoId), currentFileText, fileName, dict, branchToUpdate, targetFileSha);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }

    }
}
