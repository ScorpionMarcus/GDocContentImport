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
            // Check if the required number of arguments are provided
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: GDocContentImport <GoogleDocID> <ProjectID>");
                return;
            }

            // Parse the Google Doc ID and Project ID from the arguments
            string googleDocId = args[0];
            if (!int.TryParse(args[1], out int projectId))
            {
                Console.WriteLine("Error: Invalid ProjectID format. ProjectID should be an integer.");
                return;
            }

            // Load the application configuration
            IConfiguration configuration = LoadConfiguration();
            // Get the connection string for the content database
            string connectionString = configuration.GetConnectionString("ContentDatabase") ?? string.Empty;

            // Initialize the GoogleDocsImporter and ContentDatabase instances
            GoogleDocsImporter googleDocsImporter = new GoogleDocsImporter();
            ContentDatabase contentDatabase = new ContentDatabase(connectionString);

            // Retrieve the content of the Google Doc
            string documentContent = await googleDocsImporter.GetDocumentContentAsync(googleDocId);

            // Parse the document content into a structured format
            DocumentContentParser parser = new DocumentContentParser();
            var parsedContent = parser.Parse(documentContent, projectId);

            // Insert or update the parsed content in the content database
            foreach (var content in parsedContent)
            {
                await contentDatabase.InsertOrUpdateContentAsync(content.ProjectId, content.PageId, content.ElementId, content.Content);
                Console.WriteLine($"Processed content for ProjectID: {content.ProjectId}, PageID: {content.PageId}, ElementID: {content.ElementId}");
            }
        }

        // Load the application configuration from the appsettings.json file
        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
