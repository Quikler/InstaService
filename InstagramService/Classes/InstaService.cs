using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using System.Net.Http;

namespace InstagramService.Classes
{
    public class InstaService
    {
        public IInstaApi Api { get; }
        public InstaMediaProcessor MediaProcessor { get; }
        public InstaStreamProcessor StreamProcessor { get; }

        public InstaService(IInstaApi api)
        {
            Api = api;
            MediaProcessor = new InstaMediaProcessor(Api);
            StreamProcessor = new InstaStreamProcessor(Api);
        }

        public static InstaService BuildAndLoad(string sessionFilePath,
            DebugLogger useLogger = null, HttpClient useHttpClient = null)
        {
            IInstaApi api = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = sessionFilePath })
                .UseLogger(useLogger)
                .UseHttpClient(useHttpClient)
                .Build();

            api.SessionHandler.Load(false);

            return new InstaService(api);
        }
    }
}