using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GDocContentImport
{
    internal class GoogleDocsImporter
    {
        // Define the Google Docs API scopes and application name
        private static readonly string[] Scopes = { DocsService.Scope.DocumentsReadonly };
        private static readonly string ApplicationName = "GoogleDocsContentImporter";

        // Retrieve the content of a Google Doc by its ID
        public async Task<string> GetDocumentContentAsync(string documentId)
        {
            var docsService = await GetDocsServiceAsync();
            var document = await docsService.Documents.Get(documentId).ExecuteAsync();
            return ExtractTextFromDocument(document);
        }

        // Initialize the Google Docs API service
        private async Task<DocsService> GetDocsServiceAsync()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credential = await AuthorizeAsync(stream);
                return CreateDocsService(credential);
            }
        }

        // Authorize the application using the provided client secret file
        private async Task<UserCredential> AuthorizeAsync(Stream clientSecretsStream)
        {
            var credPath = "token.json";
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(clientSecretsStream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }

        // Create a new Google Docs service instance with the authorized user credentials
        private DocsService CreateDocsService(UserCredential credential)
        {
            return new DocsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        // Extract the text content from a Google Docs API document object
        private string ExtractTextFromDocument(Document document)
        {
            var content = new System.Text.StringBuilder();
            foreach (var element in document.Body.Content)
            {
                if (element.Paragraph != null)
                {
                    foreach (var textRun in element.Paragraph.Elements)
                    {
                        content.Append(textRun.TextRun.Content);
                    }
                }
            }
            return content.ToString();
        }
    }
}
