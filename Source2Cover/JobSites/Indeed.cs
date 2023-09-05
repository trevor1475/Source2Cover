using System.Text.RegularExpressions;

namespace Source2Cover.JobSites
{
    internal class Indeed : JobSite
    {
        internal override string UrlPattern => @"(https{0,1})(:\/\/\w{0,3}\.{0,1}indeed\.\w{2,3}\/)(.*)";
        internal override string JobDescriptionElementId => "JobPosting";
        internal override string JobTitleElementId => "jobsearch-JobInfoHeader-title ";
        internal override string CompanyElementId => "indeed.com/cmp/";
        internal override string HtmlSignifier => "_INDEED";

        public Indeed() { }

        internal override bool IsValid(string source)
        {
            Match match;

            if (!(match = Regex.Match(source, UrlPattern)).Success || match.Groups.Count != 4)
            {
                string urlArgs = match.Groups[match.Groups.Count-1].Value;
                string jobArg = urlArgs.Substring(urlArgs.IndexOf("jk="));

                int notOnlyArgIndex = jobArg.IndexOf('&');
                if (notOnlyArgIndex != -1)
                {
                    jobArg = jobArg.Substring(0, notOnlyArgIndex);
                }

                UrlId = jobArg;
                Url = $"https{match.Groups[2]}viewjob?{jobArg}";

                return Regex.IsMatch(source, UrlPattern);
            }

            if (source.Contains(HtmlSignifier))
            {
                string urlFromSourceId = "<link rel=\"canonical";
                string startOfUrlTag = source.Substring(source.IndexOf(urlFromSourceId));
                string startOfUrl = startOfUrlTag.Substring(startOfUrlTag.IndexOf("jk="));
                UrlId = startOfUrl.Substring(3, startOfUrl.IndexOf("\"") - 3);
                return true;
            }

            return false;
        }

        internal override string ParseElement(string elementId, string elementType, string source)
        {
            if (elementId == JobDescriptionElementId)
            {
                return source.Substring(source.IndexOf(elementId));
            }

            return base.ParseElement(elementId, elementType, source);
        }
    }
}
