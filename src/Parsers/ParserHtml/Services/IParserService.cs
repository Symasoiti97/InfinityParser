using System.Threading.Tasks;

namespace ParserHtml.Services
{
    public interface IParserService<T> where T : class
    {
        Task<T> ParseHtmlContent(string htmlContent);
    }
}