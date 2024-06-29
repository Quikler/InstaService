using InstagramApiSharp.Classes.Models;
using System.IO;
using System;

namespace InstagramService.Classes.Models
{
    public class InstaMediaStream : IDisposable
    {
        public Stream Stream { get; }
        public InstaMediaType MediaType { get; }
        public string Uri { get; }
        public string InitialUri { get; }
        public int CarouselIndex { get; }
        public InstaMedia InstaMedia { get; }

        public InstaMediaStream(Stream stream, InstaMediaType mediaType, 
            string uri, string initialUri, int carouselIndex, InstaMedia instaMedia)
        {
            Stream = stream;
            MediaType = mediaType;
            Uri = uri;
            InitialUri = initialUri;
            CarouselIndex = carouselIndex;
            InstaMedia = instaMedia;
        }

        ~InstaMediaStream() => Stream?.Dispose();
        public void Dispose()
        {
            Stream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}