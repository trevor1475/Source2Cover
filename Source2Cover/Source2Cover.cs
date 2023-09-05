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

            var postings = Directory.GetFiles("Postings");

            foreach (var posting in postings)
            {
                var jobHtml = File.ReadAllText(posting);
                jobSite = MatchesJobSite(jobHtml, jobSites);

                if (jobSite != null)
                {
                    jobSite.Parse(jobHtml);
                    CoverLetterGenerator.Generate(jobSite.JobTitle, jobSite.Company, jobSite.City, jobSite.JobDescription, UrlId: jobSite.UrlId);
                }
            }

            //if (args.Length == 0 || (jobSite = MatchesJobSite(args[0], jobSites)) is null)
            //{
            //    do
            //    {
            //        Console.WriteLine("No valid url found, please enter an indeed url:");
            //        url = Console.ReadLine();
            //    } while (url is null || (jobSite = MatchesJobSite(url, jobSites)) is null);
            //}


            // TODO keep getting 403, not sure
            //using (var handler = new HttpClientHandler())
            //{
            //    handler.UseDefaultCredentials = true;

            //    using (HttpClient client = new())
            //    {
            //        client.DefaultRequestHeaders.Clear();
            //        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

            //        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            //        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
            //        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");



            //        client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "max-age=0");
            //        client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            //        //client.DefaultRequestHeaders.ConnectionClose = false;

            //        using (HttpResponseMessage response = client.GetAsync(jobSite.Url).Result)
            //        {
            //            // HTTP, not HTTPS
            //            if (!response.IsSuccessStatusCode)
            //            {
            //                Console.WriteLine($"Failed to pull source code from: {jobSite.Url}");
            //                Console.WriteLine($"{response.ToString()}");
            //                return;
            //            }

            //            using (HttpContent content = response.Content)
            //            {
            //                string source = content.ReadAsStringAsync().Result;
            //                jobSite.Parse(source);
            //            }
            //        }
            //    }
            //}
        }

        private static JobSite? MatchesJobSite(string source, IEnumerable<JobSites.JobSite> jobSites)
        {
            foreach (JobSite jobSite in jobSites)
            {
                if (jobSite.IsValid(source))
                {
                    return jobSite;
                }
            }

            return null;
        }
    }
}