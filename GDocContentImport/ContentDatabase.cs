using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GDocContentImport
{
    internal class ContentDatabase
    {
        private readonly string _connectionString;

        public ContentDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

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
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@pageId", pageId);
                    command.Parameters.AddWithValue("@elementId", elementId);

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

        public async Task InsertOrUpdateContentAsync(int projectId, int pageId, string elementId, string content)
        {
            var (elementExists, elementHasContent) = await CheckElementContentAsync(projectId, pageId, elementId);
            string query;

            if (!elementExists)
            {
                query = @"
                INSERT INTO Cobalt.dbo.Content
                (ProjectID, PageID, ElementID, Content, LastWrite, ChangedBy, ChangedOn, DateCreated)
                VALUES
                (@projectId, @pageId, @elementId, @content, GETDATE(), 'GoogleDocsImporter', GETDATE(), GETDATE())";
            }
            else if (!elementHasContent)
            {
                query = @"
                UPDATE Cobalt.dbo.Content
                SET Content = @content, LastWrite = GETDATE(), ChangedBy = 'GoogleDocsImporter', ChangedOn = GETDATE()
                WHERE ProjectID = @projectId AND PageID = @pageId AND ElementID = @elementId";
            }
            else
            {
                Console.WriteLine($"ElementID {elementId} already exists and has content. Do you want to overwrite the content? (Y/N)");
                var userInput = Console.ReadLine();
                if (userInput?.ToUpper() != "Y")
                {
                    return;
                }

                query = @"
                UPDATE Cobalt.dbo.Content
                SET Content = @content, LastWrite = GETDATE(), ChangedBy = 'GoogleDocsImporter', ChangedOn = GETDATE()
                WHERE ProjectID = @projectId AND PageID = @pageId AND ElementID = @elementId";
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@pageId", pageId);
                    command.Parameters.AddWithValue("@elementId", elementId);
                    command.Parameters.AddWithValue("@content", content);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

