using InstagramApiSharp.Classes.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InstagramService.Classes.Helpers
{
    internal static class InstaCarouselHelper
    {
        public static ArrayList GetCarouselMedias(InstaCarousel carouselItems)
        {
            ArrayList arrayList = new ArrayList();

            foreach (var item in carouselItems)
            {
                // if videos exist then it's 100% video
                if (item.Videos.Count > 0)
                {
                    arrayList.Add(item.Videos.MaxBy(iv => iv.Width * iv.Height));
                }
                else
                {
                    arrayList.Add(item.Images.MaxBy(im => im.Width * im.Height));
                }
            }

            return arrayList;
        }

        public static IReadOnlyList<string> GetCarouselInitialUris(string url, InstaCarousel instaCarouselItems)
        {
            const string imageIndex = "img_index=";
            var uri = new System.Uri(url);

            string[] uris = new string[instaCarouselItems.Count];
            string uriWithoutQuery = InstaUriHelper.GetPartWithoutQuery(uri);

            List<string> queries = uri.GetComponents(System.UriComponents.Query,
                System.UriFormat.Unescaped).Split('&').ToList();
            int queriesImgIndexIndex = queries.FindIndex(s => s.Contains(imageIndex));

            if (queriesImgIndexIndex == -1)
            {
                queries.Add(string.Empty);
                queriesImgIndexIndex = queries.Count - 1;
            }

            for (int i = 0; i < instaCarouselItems.Count; i++)
            {
                queries[queriesImgIndexIndex] = imageIndex + (i + 1);
                uris[i] = $"{uriWithoutQuery}?{string.Join("&", queries)}";
            }

            return uris;
        }
    }
}