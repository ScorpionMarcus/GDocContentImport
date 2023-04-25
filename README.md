# GDocContentImport

GDocContentImport is a .NET console application that imports content from Google Docs into a proprietary .NET-based CMS. The content is inserted into an SQL Server content database table called Cobalt.dbo.Content. The program can handle multiple projects, and the Google Docs document must have a specific format to define project IDs and element IDs.

## Getting Started

To set up and run the project, follow the steps below:

### Prerequisites

- .NET SDK (version 6.0 or later)
- A Google API key with access to Google Docs API
- A client_secret.json file generated from your Google Cloud Console
- An SQL Server database with a table called Cobalt.dbo.Content

### Installation

1. Clone the repository:

git clone https://github.com/ScorpionMarcus/GDocContentImport.git

2. Navigate to the project folder:

cd GDocContentImport

3. Install the required NuGet packages:

dotnet restore

4. Add your `client_secret.json` file to the project folder.

5. Update the `appsettings.json` file with your database connection string.

### Running the Application

To run the application, execute the following command in the project folder:

dotnet run <documentId>

Replace `<documentId>` with the Google Docs document ID containing your content.

The application will fetch the content from the specified Google Docs document and insert or update it into the Cobalt.dbo.Content table in the specified SQL Server database. If an element already exists with content, the application will prompt the user to decide whether to overwrite the content.

## Built With

- C# (.NET 6.0)
- Google.Apis.Docs.v1
- Google.Apis.Auth
- Microsoft.Extensions.Configuration
- System.Data.SqlClient
