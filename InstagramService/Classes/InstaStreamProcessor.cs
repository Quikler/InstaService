using InstagramService.Classes.Models;
using InstagramApiSharp.Classes;
using InstagramApiSharp.API;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.IO;

namespace InstagramService.Classes
{
    public class InstaStreamProcessor
    {
        public IInstaApi Api { get; }

        public InstaStreamProcessor(IInstaApi api)
        {
            Api = api;
        }

        public async Task<IResult<IEnumerable<InstaMediaStream>>> GetMediaStreamsAsync(
            string url, InstaMediaProcessor mediaProcessor = null)
        {
            mediaProcessor = mediaProcessor ?? new InstaMediaProcessor(Api);
            var instaMediaInfosResult = await mediaProcessor.GetInfosAsync(url);

            if (!instaMediaInfosResult.Succeeded)
                return Result.Fail<IEnumerable<InstaMediaStream>>(instaMediaInfosResult.Info.Exception);

            IEnumerable<InstaMediaInfo> instaMediaInfos = instaMediaInfosResult.Value;

            return await GetMediaStreams(instaMediaInfos);
        }

        public Task<IResult<IEnumerable<InstaMediaStream>>> GetMediaStreamsAsync(
            IEnumerable<InstaMediaInfo> instaMediaInfos) => GetMediaStreams(instaMediaInfos);

        private static async Task<IResult<IEnumerable<InstaMediaStream>>> GetMediaStreams(IEnumerable<InstaMediaInfo> instaMediaInfos)
        {
            int count = instaMediaInfos.Count();
            InstaMediaStream[] instaMediaStreams = new InstaMediaStream[count];
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        InstaMediaInfo mediaInfo = instaMediaInfos.ElementAt(i);

                        using (HttpResponseMessage response = await hc.GetAsync(mediaInfo.Uri))
                        {
                            if (!response.IsSuccessStatusCode)
                                return Result.Fail(response.ReasonPhrase, instaMediaStreams);

                            Stream source = await response.Content.ReadAsStreamAsync();
                            Stream dest = new MemoryStream();

                            source.CopyTo(dest);
                            dest.Seek(0, SeekOrigin.Begin);

                            instaMediaStreams[i] = new InstaMediaStream(dest, mediaInfo.MediaType,
                                mediaInfo.Uri, mediaInfo.InitialUri, i + 1, mediaInfo.InstaMedia);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    return Result.Fail(ex, instaMediaStreams);
                }
            }

            return Result.Success(instaMediaStreams);
        }
    }
}