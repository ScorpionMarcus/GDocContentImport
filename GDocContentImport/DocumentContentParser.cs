using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GDocContentImport
{
    public class DocumentContentParser
    {
        // Parse the document content and return a list of DocumentContent objects
        public List<DocumentContent> Parse(string documentContent, int projectId)
        {
            var parsedContent = new List<DocumentContent>();

            // Split the content into sections based on the PAGEID pattern
            var pageSections = Regex.Split(documentContent, @"\n(?=PAGEID:\s*\d+)", RegexOptions.Multiline);

            // Iterate through the sections and extract the required information
            foreach (var pageSection in pageSections)
            {
                if (string.IsNullOrWhiteSpace(pageSection))
                {
                    continue;
                }

                // Extract the page ID from the section
                var pageIdMatch = Regex.Match(pageSection, @"PAGEID:\s*(\d+)");
                if (!pageIdMatch.Success)
                {
                    Console.WriteLine($"Warning: Ignoring line: {pageSection}");
                    continue;
                }

                int pageId = int.Parse(pageIdMatch.Groups[1].Value);

                // Extract the element ID and content from the section
                var elementIdMatch = Regex.Match(pageSection, @"ELEMENTID:\s*(\w+)");
                var contentMatch = Regex.Match(pageSection, @"(?<=ELEMENTID:\s*\w+\s)(.*)");

                if (elementIdMatch.Success && contentMatch.Success)
                {
                    string elementId = elementIdMatch.Groups[1].Value;
                    string content = contentMatch.Value;

                    // Create a new DocumentContent object and add it to the parsed content list
                    parsedContent.Add(new DocumentContent(projectId, pageId, elementId, content));
                }
                else
                {
                    Console.WriteLine($"Warning: Ignoring line: {pageSection}");
                }
            }

            return parsedContent;
        }
    }
}
