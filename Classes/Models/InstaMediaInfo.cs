using InstagramApiSharp.Classes.Models;

namespace InstagramService.Classes.Models
{
    public class InstaMediaInfo
    {
        public InstaMediaType MediaType { get; }
        public string Uri { get; }
        public string InitialUri { get; }
        public int CarouselIndex { get; }
        public InstaMedia InstaMedia { get; }

        public InstaMediaInfo(InstaMediaType mediaType, string uri,
            string initialUri, int carouselIndex, InstaMedia instaMedia)
        {
            MediaType = mediaType;
            Uri = uri;
            InitialUri = initialUri;
            CarouselIndex = carouselIndex;
            InstaMedia = instaMedia;
        }
    }
}
