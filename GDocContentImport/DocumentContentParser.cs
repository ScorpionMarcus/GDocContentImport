using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GDocContentImport
{
    public class DocumentContentParser
    {
        public List<DocumentContent> Parse(string documentContent, int projectId)
        {
            var parsedContent = new List<DocumentContent>();

            var pageSections = Regex.Split(documentContent, @"\n(?=PAGEID:\s*\d+)", RegexOptions.Multiline);

            foreach (var pageSection in pageSections)
            {
                if (string.IsNullOrWhiteSpace(pageSection))
                {
                    continue;
                }

                var pageIdMatch = Regex.Match(pageSection, @"PAGEID:\s*(\d+)");
                if (!pageIdMatch.Success)
                {
                    Console.WriteLine($"Warning: Ignoring line: {pageSection}");
                    continue;
                }

                int pageId = int.Parse(pageIdMatch.Groups[1].Value);
                var elementIdMatch = Regex.Match(pageSection, @"ELEMENTID:\s*(\w+)");
                var contentMatch = Regex.Match(pageSection, @"(?<=ELEMENTID:\s*\w+\s)(.*)");

                if (elementIdMatch.Success && contentMatch.Success)
                {
                    string elementId = elementIdMatch.Groups[1].Value;
                    string content = contentMatch.Value;
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

