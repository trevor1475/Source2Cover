using System.Text.RegularExpressions;

namespace Source2Cover.JobSites
{
    internal abstract class JobSite
    {
        internal abstract string HtmlSignifier { get; }
        internal abstract string UrlPattern { get; }
        internal abstract string JobTitleElementId { get; }
        internal virtual string JobTitleElementType { get; } = ElementTypes.H1;
        public string JobTitle { get; private set; }
        internal abstract string CompanyElementId { get; }
        internal virtual string CompanyElementType { get; } = ElementTypes.A;
        public string Company { get; private set; }
        internal abstract string JobDescriptionElementId { get; }
        internal virtual string JobDescriptionElementType { get; } = ElementTypes.DIV;
        public string JobDescription { get; private set; }
        internal abstract string CityElementId { get; }
        internal virtual string CityElementType { get; } = ElementTypes.DIV;
        public string City { get; private set; }

        public string UrlId { get; set; }
        public string Url { get; set; }

        internal JobSite() {}

        internal virtual bool IsValid(string url)
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

        internal virtual void Parse(string source)
        {
            JobTitle = ParseElement(JobTitleElementId, JobTitleElementType, source);
            Company = ParseElement(CompanyElementId, CompanyElementType, source);
            JobDescription = ParseElement(JobDescriptionElementId, JobDescriptionElementType, source);
            City = ParseElement(CityElementId, CityElementType, source);
        }

        internal virtual string ParseElement(string elementId, string elementType, string source)
        {
            // Find element by Identifier, cut off end of element, cut off inner elements
            string text = source.Substring(source.IndexOf(elementId));
            int startOfInnerText;
            int endOfInnerText;

            do
            {
                startOfInnerText = text.IndexOf(">");
                text = text.Substring(startOfInnerText + 1);
            } while (text[0] == '<');

            endOfInnerText = text.IndexOf($"</{elementType}");
            text = text.Substring(0, endOfInnerText);
            while (text[text.Length - 1] == '>')
            {
                endOfInnerText = text.LastIndexOf("</");
                text = text.Substring(0, endOfInnerText);
            } ;

            //int endOfElement = text.IndexOf($"</{elementType}");
            //if (endOfElement != -1)
            //{
            //    text = text.Substring(0, endOfElement);
            //}

            //int endOfInnerText = text.IndexOf($"</");
            //if (endOfInnerText != -1)
            //{
            //    text = text.Substring(0, endOfInnerText);
            //}

            //int startOfInnerText = text.LastIndexOf(">");
            //if (startOfInnerText != -1)
            //{
            //    text = text.Substring(startOfInnerText);
            //}

            //if (text.Contains(">"))
            //{
            //    return text.Substring(text.IndexOf(">") + 1);
            //}

            return text;
        }
    }
}
