using System.Text.RegularExpressions;

namespace Source2Cover.JobSites
{
    internal abstract class JobSite
    {
        internal abstract string UrlPattern { get; }
        internal abstract string JobDescriptionElementId { get; }
        internal virtual string JobDescriptionElementType { get; } = ElementTypes.DIV;

        internal string Url { get; set; }

        internal JobSite() {}

        internal virtual bool IsValidUrl(string url)
        {
            Url = url;
            return Regex.IsMatch(url, UrlPattern);
        }

        internal virtual string RemoveHtmlClutter(string innerText)
        {
            string cleanedUpText = Regex.Replace(innerText, @"&amp;", "&");
            cleanedUpText = Regex.Replace(cleanedUpText, @"<li>", " - ");
            cleanedUpText = Regex.Replace(cleanedUpText, @"<\/{0,1}(?:b|i|p|ul|li)>", "");
            cleanedUpText = Regex.Replace(cleanedUpText, @"\n+", "\n");
            return cleanedUpText;
        }

        internal virtual string GetJobDescription(string source)
        {
            string startOfId = source.Substring(source.IndexOf(JobDescriptionElementId));
            int startOfInnerText = startOfId.IndexOf(">") + 1;
            string innerText = startOfId.Substring(startOfInnerText, startOfId.IndexOf($"</{JobDescriptionElementType}>") - startOfInnerText);

            return RemoveHtmlClutter(innerText);
        }
    }
}
