# GDocContentImport

GDocContentImport is a .NET console application that imports content from Google Docs into the main content zone of various pages on a website using a proprietary .NET-based CMS. The content is inserted into a content database called Cobalt.dbo.Content.

## Getting Started

To set up and run the project, follow the steps below:

### Prerequisites

- .NET SDK (version 5.0 or later)
- A Google API key with access to Google Docs API
- A client_secret.json file generated from your Google Cloud Console
- An SQL Server database with a table called Cobalt.dbo.Content

### Installation

1. Clone the repository:

git clone https://github.com/yourusername/GDocContentImport.git


2. Navigate to the project folder:

cd GDocContentImport

3. Install the required NuGet packages:

dotnet restore

4. Add your `client_secret.json` file to the project folder.

5. Update the `Program.cs` file with your database connection string and other necessary information like the Google Docs document ID, projectId, pageId, and elementId.

### Running the Application

To run the application, execute the following command in the project folder:

dotnet run

The application will fetch the content from the specified Google Docs document and insert it into the Cobalt.dbo.Content table in the specified SQL Server database.

## Built With

- C# (.NET 5.0)
- Google.Apis.Docs.v1
- Google.Apis.Auth
- System.Data.SqlClient
