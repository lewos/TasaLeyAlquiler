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
                var config = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .Build();

                var gitAuthToken = config.GetSection("GH_TOKEN");
                if (gitAuthToken == null)
                    throw new Exception("sigue null");
                var repoId = config.GetSection("GH_REPO_ID");
                var branchToUpdate = config.GetSection("GH_BRANCH");
                var fileName = config.GetSection("GH_FILE_NAME");




                // Fecha de inicio
                var desde = new DateTime(2020, 07, 01);
                // Fecha de corrida del job
                var hasta = DateTime.UtcNow;

                var content = await ServiceBCRA.GetFechaYTasasAsync(desde, hasta);

                var dict = Parser.GetDictFromContent(content);

                var gitHubService = new GitHubService();

                var user = await gitHubService.GetCurrentUser(gitAuthToken.Value);

                ////Console.WriteLine($"Request disponibles: {gitHubService.CheckRate()}");

                var repo = await gitHubService.GetRepo(Convert.ToInt64(repoId.Value));


                var tuple = await gitHubService.GetFileContent(Convert.ToInt64(repoId.Value), fileName.Value);
                var currentFileText = tuple.Item1;
                var targetFileSha = tuple.Item2;

                await gitHubService.UpdateAndCommit(Convert.ToInt64(repoId.Value), currentFileText, fileName.Value, dict, branchToUpdate.Value, targetFileSha);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }

    }
}
