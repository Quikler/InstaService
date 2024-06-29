using InstagramApiSharp.Classes.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InstagramService.Classes.Helpers
{
    internal static class InstaParseHelper
    {
        public static string ParseUsernameFromStory(string uri)
            => new Uri(uri).Segments[2].Trim('/');

        public static IReadOnlyList<InstaMediaType> ParseMediaTypes(InstaMedia media)
        {
            var mediaTypes = new List<InstaMediaType>();

            if (media.MediaType != InstaMediaType.Carousel)
            {
                mediaTypes.Add(media.MediaType);
                return mediaTypes;
            }

            ArrayList carouselMedias = InstaCarouselHelper.GetCarouselMedias(media.Carousel);
            foreach (object carouselMedia in carouselMedias)
            {
                if (carouselMedia is InstaVideo)
                {
                    mediaTypes.Add(InstaMediaType.Video);
                }
                else
                {
                    mediaTypes.Add(InstaMediaType.Image);
                }
            }

            return mediaTypes;
        }

        public static IReadOnlyList<string> ParseUris(InstaMedia media)
        {
            var mediaUris = new List<string>();

            switch (media.MediaType)
            {
                case InstaMediaType.Image:
                    mediaUris.Add(media.Images.MaxBy(im => im.Width * im.Height).Uri);
                    break;
                case InstaMediaType.Video:
                    mediaUris.Add(media.Videos.MaxBy(iv => iv.Width * iv.Height).Uri);
                    break;
                case InstaMediaType.Carousel:
                    ArrayList carouselMedias = InstaCarouselHelper.GetCarouselMedias(media.Carousel);
                    foreach (object carouselMedia in carouselMedias)
                    {
                        if (carouselMedia is InstaVideo video)
                        {
                            mediaUris.Add(video.Uri);
                        }
                        else
                        {
                            mediaUris.Add(((InstaImage)carouselMedia).Uri);
                        }
                    }
                    break;
            }

            return mediaUris;
        }
    }
}