using System;
using System.Collections.Generic;

namespace GDocContentImport
{
    public class DocumentContentParser
    {
        public Dictionary<int, Dictionary<int, (string ElementId, string Content)>> Parse(string documentContent)
        {
            const string projectIdDelimiter = "PROJECTID:";
            const string pageIdDelimiter = "PAGEID:";
            const string elementIdDelimiter = "ELEMENTID:";

            var projectSections = documentContent.Split(new[] { projectIdDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            var projectContentMap = new Dictionary<int, Dictionary<int, (string ElementId, string Content)>>();

            foreach (var projectSection in projectSections)
            {
                var lines = projectSection.Split('\n');
                if (lines.Length < 2) continue;

                string projectIdStr = lines[0].Trim();
                if (!int.TryParse(projectIdStr, out int projectId)) continue;

                var pageContentMap = new Dictionary<int, (string ElementId, string Content)>();
                projectContentMap[projectId] = pageContentMap;

                string pageSections = string.Join('\n', lines[1..]);
                var sections = pageSections.Split(new[] { pageIdDelimiter }, StringSplitOptions.RemoveEmptyEntries);

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
            }

            return projectContentMap;
        }
    }
}
