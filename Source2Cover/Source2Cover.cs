using Source2Cover.JobSites;
using System.Text.RegularExpressions;

namespace Source2Cover
{
    class Source2Cover
    {
        private static void Main(string[] args)
        {
            string url;
            JobSite jobSite;

            IEnumerable<JobSite> jobSites = typeof(JobSite)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(JobSite)) && !t.IsAbstract)
            .Select(t => (JobSite)Activator.CreateInstance(t));

            if (args.Length == 0 || (jobSite = MatchesJobSite(args[0], jobSites)) is null)
            {
                do
                {
                    Console.WriteLine("No valid url found, please enter an indeed url:");
                    url = Console.ReadLine();
                } while (url is null || (jobSite = MatchesJobSite(url, jobSites)) is null);
            }

            string jobDescription;
            using (HttpClient client = new())
            {
                using (HttpResponseMessage response = client.GetAsync(jobSite.Url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string source = content.ReadAsStringAsync().Result;
                        jobDescription = jobSite.GetJobDescription(source);
                    }
                }
            }

            Console.WriteLine(jobDescription);
        }

        private static JobSite? MatchesJobSite(string url, IEnumerable<JobSites.JobSite> jobSites)
        {
            foreach (JobSite jobSite in jobSites)
            {
                if (jobSite.IsValidUrl(url))
                {
                    return jobSite;
                }
            }

            return null;
        }
    }
}