using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GDocContentImport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: GDocContentImport <documentId>");
                return;
            }

            string documentId = args[0];

            // Load the configuration from the appsettings.json file.
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get the connection string from the configuration, or use an empty string if it's null.
            string connectionString = configuration.GetConnectionString("ContentDatabase") ?? string.Empty;

            GoogleDocsImporter googleDocsImporter = new GoogleDocsImporter();
            ContentDatabase contentDatabase = new ContentDatabase(connectionString);

            string documentContent = await googleDocsImporter.GetDocumentContentAsync(documentId);
            DocumentContentParser parser = new DocumentContentParser();
            (int projectId, Dictionary<int, (string ElementId, string Content)> pageContentMap) = parser.Parse(documentContent);

            foreach (var entry in pageContentMap)
            {
                int pageId = entry.Key;
                string elementId = entry.Value.ElementId;
                string content = entry.Value.Content;

                await contentDatabase.InsertOrUpdateContentAsync(projectId, pageId, elementId, content);
                Console.WriteLine($"Processed content for PageID: {pageId}, ElementID: {elementId}");
            }
        }
    }
}
