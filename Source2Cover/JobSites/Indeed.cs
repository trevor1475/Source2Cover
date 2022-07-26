using System.Text.RegularExpressions;

namespace Source2Cover.JobSites
{
    internal class Indeed : JobSite
    {
        internal override string UrlPattern => @"(https:\/\/{0,1}\w{0,3}\.{0,1}indeed\.\w{2,3}\/)(.*)";
        internal override string JobDescriptionElementId => "jobDescriptionText";

        public Indeed() { }

        internal override bool IsValidUrl(string url)
        {
            Match match;

            if (!(match = Regex.Match(url, UrlPattern)).Success && match.Groups.Count == 3)
            {
                return false;
            }

            string urlArgs = match.Groups[2].Value;
            string jobArg = urlArgs.Substring(urlArgs.IndexOf("jk="));
            
            int notOnlyArgIndex = jobArg.IndexOf('&');
            if (notOnlyArgIndex != -1)
            {
                jobArg = jobArg.Substring(0, notOnlyArgIndex);
            }

            Url = $"{match.Groups[1]}viewjob?{jobArg}";

            return Regex.IsMatch(url, UrlPattern);
        }
    }
}
