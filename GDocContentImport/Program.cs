using GDocContentImport;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Replace the following with your Google Docs document ID and database connection string.
        string documentId = "your-google-docs-document-id";
        string connectionString = "your-database-connection-string";

        // Replace these with the appropriate values for your content database.
        int projectId = 55657;
        int pageId = 15869557;
        string elementId = "MainContent";

        var googleDocsImporter = new GoogleDocsImporter();
        var contentDatabase = new ContentDatabase(connectionString);

        try
        {
            Console.WriteLine("Retrieving content from Google Docs...");
            string content = await googleDocsImporter.GetDocumentContentAsync(documentId);
            Console.WriteLine("Content retrieved. Inserting into the database...");

            await contentDatabase.InsertContentAsync(projectId, pageId, elementId, content);
            Console.WriteLine("Content inserted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
