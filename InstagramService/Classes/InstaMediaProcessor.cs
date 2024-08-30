using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramService.Classes.Helpers;
using InstagramService.Classes.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace InstagramService.Classes
{
    public class InstaMediaProcessor
    {
        public IInstaApi Api { get; }

        public InstaMediaProcessor(IInstaApi api)
        {
            Api = api;
        }

        public async Task<IResult<IEnumerable<InstaMediaInfo>>> GetInfosAsync(string uri)
        {
            IResult<InstaMedia> mediaResult = await GetInstaMediaAsync(uri);

            if (!mediaResult.Succeeded)
                return Result.Fail<IEnumerable<InstaMediaInfo>>(mediaResult.Info.ToString());

            InstaMedia media = mediaResult.Value;

            IReadOnlyList<string> mediaUris = InstaParseHelper.ParseUris(media);
            IReadOnlyList<InstaMediaType> mediaTypes = InstaParseHelper.ParseMediaTypes(media);
            IReadOnlyList<string> initialUris = media.Carousel?.Count > 0 ?
                InstaCarouselHelper.GetCarouselInitialUris(uri, media.Carousel) : new[] { uri };

            InstaMediaInfo[] instaMediaInfos = new InstaMediaInfo[mediaUris.Count];

            for (int i = 0; i < mediaUris.Count; i++)
            {
                instaMediaInfos[i] = new InstaMediaInfo(mediaTypes[i], mediaUris[i], initialUris[i], i + 1, media);
            }

            return Result.Success(instaMediaInfos);
        }

        public async Task<IResult<InstaMedia>> GetInstaMediaAsync(string url)
        {
            if (!System.Uri.TryCreate(url, System.UriKind.Absolute, out System.Uri resultUrl))
                return Result.Fail<InstaMedia>("Invalid url");

            /*if (!System.Uri.TryCreate(url, System.UriKind.RelativeOrAbsolute, out System.Uri resultUrl))
                return Result.Fail<InstaMedia>("Invalid url");*/

            /*var idRes = await Api.MediaProcessor.GetMediaIdFromUrlAsync(new Uri("https://www.instagram.com/p/C9nOtyisgm2NPYagc1PPXUhUEVWAhdcSSluLTE0/"));
            var media = await Api.MediaProcessor.GetMediaByIdAsync(idRes.Value);*/

            IResult<string> mediaIdFromUrlResult = await Api.MediaProcessor.GetMediaIdFromUrlAsync(resultUrl);
            if (!mediaIdFromUrlResult.Succeeded)
                return Result.Fail<InstaMedia>(mediaIdFromUrlResult.Info.Message);

            IResult<InstaMedia> mediaByIdResult = await Api.MediaProcessor.GetMediaByIdAsync(mediaIdFromUrlResult.Value);
            if (!mediaByIdResult.Succeeded)
                return Result.Fail<InstaMedia>(mediaByIdResult.Info.Message);

            return mediaByIdResult;
        }

        public async Task<FileInfo> DownloadMediaAsync(InstaMediaStream instaMediaStream, 
            string destinationFolderPath, string fileName, string imageFormat = "jpeg", string videoFormat = "mp4")
        {
            string extension = instaMediaStream.MediaType == InstaMediaType.Image ? imageFormat : videoFormat;
            string finalExtension = extension.Contains('.') ? extension : $".{extension}";

            string filePath = Path.Combine(destinationFolderPath, $"{fileName}{finalExtension}");
            FileInfo fileInfo = new FileInfo(filePath);

            using (Stream stream = fileInfo.OpenWrite())
                await instaMediaStream.Stream.CopyToAsync(stream);

            return fileInfo;
        }

        public Task<IResult<FileInfo[]>> DownloadMediasAsync(IEnumerable<InstaMediaStream> mediaStreams,
            string destinationFolderPath, string[] fileNames, string imageFormat = "jpeg", string videoFormat = "mp4")
            => DownloadMedias(mediaStreams, destinationFolderPath, fileNames, imageFormat, videoFormat);
        
        public async Task<IResult<FileInfo[]>> DownloadMediasAsync(IEnumerable<InstaMediaStream> mediaStreams,
            string destinationFolderPath, string imageFormat = "jpeg", string videoFormat = "mp4")
        {
            string[] fileNames = new string[mediaStreams.Count()];
            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = $"{mediaStreams.ElementAt(i).InstaMedia.Code}-{i + 1}";
            }
            return await DownloadMedias(mediaStreams,
                destinationFolderPath, fileNames, imageFormat, videoFormat);
        }

        private async Task<IResult<FileInfo[]>> DownloadMedias(IEnumerable<InstaMediaStream> mediaStreams,
            string destinationFolderPath, string[] fileNames, string imageFormat = "jpeg", string videoFormat = "mp4")
        {
            int mediaStreamsCount = mediaStreams.Count();
            FileInfo[] fileInfos = new FileInfo[mediaStreamsCount];

            try
            {
                Directory.CreateDirectory(destinationFolderPath);
                for (int i = 0; i < mediaStreamsCount; i++)
                {
                    fileInfos[i] = await DownloadMediaAsync(mediaStreams.ElementAt(i), destinationFolderPath,
                        fileNames[i], imageFormat, videoFormat);
                }
            }
            catch (IOException ex)
            {
                return Result.Fail<FileInfo[]>(ex);
            }

            return Result.Success(fileInfos);
        }
    }
}