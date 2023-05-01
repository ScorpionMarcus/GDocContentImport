using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GDocContentImport
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: GDocContentImport <GoogleDocID> <ProjectID>");
                return;
            }

            string googleDocId = args[0];
            if (!int.TryParse(args[1], out int projectId))
            {
                Console.WriteLine("Error: Invalid ProjectID format. ProjectID should be an integer.");
                return;
            }

            IConfiguration configuration = LoadConfiguration();
            string connectionString = configuration.GetConnectionString("ContentDatabase") ?? string.Empty;

            GoogleDocsImporter googleDocsImporter = new GoogleDocsImporter();
            ContentDatabase contentDatabase = new ContentDatabase(connectionString);

            string documentContent = await googleDocsImporter.GetDocumentContentAsync(googleDocId);

            DocumentContentParser parser = new DocumentContentParser();
            var parsedContent = parser.Parse(documentContent, projectId);

            foreach (var content in parsedContent)
            {
                await contentDatabase.InsertOrUpdateContentAsync(content.ProjectId, content.PageId, content.ElementId, content.Content);
                Console.WriteLine($"Processed content for ProjectID: {content.ProjectId}, PageID: {content.PageId}, ElementID: {content.ElementId}");
            }
        }

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
