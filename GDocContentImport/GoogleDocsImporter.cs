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
        private static readonly string[] Scopes = { DocsService.Scope.DocumentsReadonly };
        private static readonly string ApplicationName = "GoogleDocsContentImporter";

        public async Task<string> GetDocumentContentAsync(string documentId)
        {
            var docsService = await GetDocsServiceAsync();
            var document = await docsService.Documents.Get(documentId).ExecuteAsync();
            return ExtractTextFromDocument(document);
        }

        private async Task<DocsService> GetDocsServiceAsync()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credential = await AuthorizeAsync(stream);
                return CreateDocsService(credential);
            }
        }

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

        private DocsService CreateDocsService(UserCredential credential)
        {
            return new DocsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

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
