namespace Dto
{
    public class ItemDto
    {
        public string Url { get; set; }

        public virtual string HtmlMessage()
        {
            return $"<a href=\"{Url}\">{Url}</a>";
        }
    }
}