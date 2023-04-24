using System;
using System.Collections.Generic;

namespace GDocContentImport
{
    public class DocumentContentParser
    {
        public (int ProjectId, Dictionary<int, (string ElementId, string Content)> PageContentMap) Parse(string documentContent)
        {
            const string projectIdDelimiter = "PROJECTID:";
            const string pageIdDelimiter = "PAGEID:";
            const string elementIdDelimiter = "ELEMENTID:";

            int projectIdIndex = documentContent.IndexOf(projectIdDelimiter);
            if (projectIdIndex == -1)
            {
                throw new InvalidOperationException("ProjectID not found in the Google Doc.");
            }

            int projectIdEndIndex = documentContent.IndexOf('\n', projectIdIndex);
            if (projectIdEndIndex == -1)
            {
                throw new InvalidOperationException("Invalid Google Doc format.");
            }

            string projectIdStr = documentContent.Substring(projectIdIndex + projectIdDelimiter.Length, projectIdEndIndex - (projectIdIndex + projectIdDelimiter.Length)).Trim();
            if (!int.TryParse(projectIdStr, out int projectId))
            {
                throw new InvalidOperationException("Invalid ProjectID format in the Google Doc.");
            }

            var sections = documentContent.Substring(projectIdEndIndex).Split(new[] { pageIdDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            var pageContentMap = new Dictionary<int, (string ElementId, string Content)>();

            foreach (var section in sections)
            {
                int elementIdIndex = section.IndexOf(elementIdDelimiter);
                if (elementIdIndex == -1) continue;

                int pageIdDelimiterIndex = section.IndexOf('\n');
                if (pageIdDelimiterIndex == -1) continue;

                string pageIdStr = section.Substring(0, pageIdDelimiterIndex).Trim();
                if (!int.TryParse(pageIdStr, out int pageId)) continue;

                int elementIdDelimiterIndex = section.IndexOf('\n', elementIdIndex);
                if (elementIdDelimiterIndex == -1) continue;

                string elementId = section.Substring(elementIdIndex + elementIdDelimiter.Length, elementIdDelimiterIndex - (elementIdIndex + elementIdDelimiter.Length)).Trim();
                string content = section.Substring(elementIdDelimiterIndex + 1).Trim();
                pageContentMap[pageId] = (elementId, content);
            }

            return (projectId, pageContentMap);
        }
    }
}
