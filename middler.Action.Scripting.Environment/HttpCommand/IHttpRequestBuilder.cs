using System.Net.Http;
using System.Threading.Tasks;

namespace middler.Scripting.HttpCommand
{
    public interface IHttpRequestBuilder
    {
        Task<HttpResponse> SendAsync(HttpMethod httpMethod);
        Task<HttpResponse> SendRequestMessageAsync(HttpRequestMessage httpMethod);

        Task<HttpResponse> GetAsync();
        Task<HttpResponse> PostAsync();
        Task<HttpResponse> PutAsync();
        Task<HttpResponse> DeleteAsync();


        HttpResponse Send(HttpMethod httpMethod);
        HttpResponse SendRequestMessage(HttpRequestMessage httpMethod);
        HttpResponse Get();

    }
}