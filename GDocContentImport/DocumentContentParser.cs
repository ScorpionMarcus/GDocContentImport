using System;
using System.Collections.Generic;

namespace GDocContentImport
{
    public class DocumentContentParser
    {
        public Dictionary<int, (string ElementId, string Content)> Parse(string documentContent)
        {
            const string pageIdDelimiter = "PAGEID:";
            const string elementIdDelimiter = "ELEMENTID:";
            var sections = documentContent.Split(new[] { pageIdDelimiter }, StringSplitOptions.RemoveEmptyEntries);
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

            return pageContentMap;
        }
    }
}
