namespace InstagramService.Classes.Helpers
{
    internal static class InstaUriHelper
    {
        public static string GetPartWithoutQuery(string url)
            => new System.Uri(url).GetLeftPart(System.UriPartial.Path);

        public static string GetPartWithoutQuery(System.Uri uri)
            => uri.GetLeftPart(System.UriPartial.Path);
    }
}