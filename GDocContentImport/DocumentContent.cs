namespace GDocContentImport
{
    public class DocumentContent
    {
        public int ProjectId { get; set; }
        public int PageId { get; set; }
        public string ElementId { get; set; }
        public string Content { get; set; }

        public DocumentContent(int projectId, int pageId, string elementId, string content)
        {
            ProjectId = projectId;
            PageId = pageId;
            ElementId = elementId;
            Content = content;
        }
    }
}
