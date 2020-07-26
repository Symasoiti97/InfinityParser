namespace Dto.ThreeNineMd
{
    public class ShortThreeNineMdItem : ItemDto
    {
        public string Title { get; set; }
        public string Price { get; set; }
        public string ShortDescription { get; set; }

        public override string HtmlMessage()
        {
            return $"<b>{Title}</b>\nЦена: {Price}\nКраткое описание: {ShortDescription}\n<a href=\"{Url}\">{Url}</a>";
        }
    }
}