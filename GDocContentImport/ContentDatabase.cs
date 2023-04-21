using System.Data.SqlClient;

namespace GDocContentImport
{
    internal class ContentDatabase
    {
        private readonly string _connectionString;

        public ContentDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsertContentAsync(int projectId, int pageId, string elementId, string content)
        {
            var query = @"
            INSERT INTO Cobalt.dbo.Content
            (ProjectID, PageID, ElementID, Content, LastWrite, ChangedBy, ChangedOn, DateCreated)
            VALUES
            (@projectId, @pageId, @elementId, @content, GETDATE(), 'GoogleDocsImporter', GETDATE(), GETDATE())";

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
