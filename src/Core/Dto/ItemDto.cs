namespace Dto
{
    public abstract class ItemDto
    {
        public string Url { get; set; }

        public abstract string HtmlMessage();
    }
}