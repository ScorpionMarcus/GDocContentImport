using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GDocContentImport
{
    internal class ContentDatabase
    {
        private readonly string _connectionString;

        // Constructor for initializing the ContentDatabase class with a connection string
        public ContentDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Insert or update content in the database
        public async Task InsertOrUpdateContentAsync(int projectId, int pageId, string elementId, string content)
        {
            var (elementExists, elementHasContent) = await CheckElementContentAsync(projectId, pageId, elementId);
            string query = GetQuery(elementExists, elementHasContent, elementId);

            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            // Execute the query using SqlConnection and SqlCommand
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    AddParametersToCommand(command, projectId, pageId, elementId, content);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Check if the content element exists and has content in the database
        private async Task<(bool Exists, bool HasContent)> CheckElementContentAsync(int projectId, int pageId, string elementId)
        {
            var query = @"
            SELECT Content
            FROM Cobalt.dbo.Content
            WHERE ProjectID = @projectId AND PageID = @pageId AND ElementID = @elementId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    AddParametersToCommand(command, projectId, pageId, elementId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string content = reader["Content"] as string ?? string.Empty;
                            return (true, !string.IsNullOrEmpty(content));
                        }
                    }
                }
            }
            return (false, false);
        }

        // Get the appropriate SQL query based on the element's existence and content
        private string GetQuery(bool elementExists, bool elementHasContent, string elementId)
        {
            if (!elementExists)
            {
                return @"
            INSERT INTO Cobalt.dbo.Content
            (ProjectID, PageID, ElementID, Content, LastWrite, ChangedBy, ChangedOn, DateCreated)
            VALUES
            (@projectId, @pageId, @elementId, @content, GETDATE(), 'GoogleDocsImporter', GETDATE(), GETDATE())";
            }
            else if (!elementHasContent)
            {
                return @"
            UPDATE Cobalt.dbo.Content
            SET Content = @content, LastWrite = GETDATE(), ChangedBy = 'GoogleDocsImporter', ChangedOn = GETDATE()
            WHERE ProjectID = @projectId AND PageID = @pageId AND ElementID = @elementId";
            }
            else
            {
                Console.WriteLine($"ElementID {elementId} already exists and has content. Do you want to overwrite the content? (Y/N)");
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Y)
                {
                    Console.WriteLine();
                    return string.Empty;
                }

                return @"
            UPDATE Cobalt.dbo.Content
            SET Content = @content, LastWrite = GETDATE(), ChangedBy = 'GoogleDocsImporter', ChangedOn = GETDATE()
            WHERE ProjectID = @projectId AND PageID = @pageId AND ElementID = @elementId";
            }

            // Return the appropriate query based on the element's existence and content
        }

        // Add parameters to the SqlCommand
        private void AddParametersToCommand(SqlCommand command, int projectId, int pageId, string elementId, string? content = null)
        {
            command.Parameters.AddWithValue("@projectId", projectId);
            command.Parameters.AddWithValue("@pageId", pageId);
            command.Parameters.AddWithValue("@elementId", elementId);
            if (content != null)
            {
                command.Parameters.AddWithValue("@content", content);
            }
        }
    }
}
