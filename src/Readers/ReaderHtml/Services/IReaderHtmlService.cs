using System.Threading.Tasks;

namespace ReaderHtml.Services
{
    public interface IReaderHtmlService
    {
        Task<string> GetAsync(string address);
    }
}