using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GDocContentImport
{
    internal class GoogleDocsImporter
    {
        private static readonly string[] Scopes = { DocsService.Scope.DocumentsReadonly };
        private static readonly string ApplicationName = "GoogleDocsContentImporter";

        private async Task<DocsService> GetDocsServiceAsync()
        {
            // Replace 'client_secret.json' with your own JSON file containing your Google API credentials.
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));

                return new DocsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
        }

        public async Task<string> GetDocumentContentAsync(string documentId)
        {
            var docsService = await GetDocsServiceAsync();
            var document = await docsService.Documents.Get(documentId).ExecuteAsync();
            return ExtractTextFromDocument(document);
        }

        private string ExtractTextFromDocument(Document document)
        {
            var content = new StringBuilder();
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
