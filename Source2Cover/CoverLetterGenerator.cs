using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Source2Cover
{
    internal static class CoverLetterGenerator
    {
        private static Config _config { get; set; }
        private static List<KeyValuePair<Regex, string>> _goalRegexes;
        private static List<KeyValuePair<Regex, string>> _skillRegexes;
        private static List<KeyValuePair<Regex, string>> _experienceRegexes;

        static CoverLetterGenerator()
        {
            using (StreamReader configFile = new StreamReader("Config.json"))
            {
                var serializer = new JsonSerializer();
                _config = (Config)serializer.Deserialize(configFile, typeof(Config));
            }

            _goalRegexes = ParseRegexesConfig(_config.GoalRegexes);
            _skillRegexes = ParseRegexesConfig(_config.SkillRegexes);
            _experienceRegexes = ParseRegexesConfig(_config.ExperienceRegexes);
        }

        internal static void Generate(string jobTitle, string company, string city, string jobDescription, string? UrlId = null)
        {
            Console.WriteLine($"Generating: {company} - {jobTitle}");

            TextInfo textInfo = new CultureInfo("en-CA", false).TextInfo;

            Console.WriteLine($"Goals:");
            var matchedGoals = MatchRegexes(jobDescription, _goalRegexes);

            Console.WriteLine($"\nSkills:");
            var matchedSkills = MatchRegexes(jobDescription, _skillRegexes);

            Console.WriteLine($"\nExperience:");
            var matchedExperiences = MatchRegexes(jobDescription, _experienceRegexes);

            string coverLetter =
                $"{_config.Name.ToUpper()}\n" +
                $"{_config.StreetAddress}\n" +
                $"{_config.CityAddress}\n" +
                $"{_config.PhoneNumber}\n" +
                $"{_config.Email}\n" +
                $"{_config.Github}\n" +
                 "\n" +
                $"{DateTime.Now.Date.ToLongDateString()}\n" +
                 "\n" +
                $"{company}\n" +
                $"{city}\n" +
                 "\n" +
                 "Dear Hiring Team,\n\n" +
                $"\tI am extremely interested in the position of {jobTitle}. My previous work experience and current skill set make me an ideal candidate for you and {company}. " +
                $"{matchedGoals.FirstOrDefault("**** No Goal Found! ****")} " +
                $"{GetCombinedIntrosAndMatches(_config.SkillIntros, matchedSkills)}\n" +
                 "\n" +
                $"\t{GetCombinedIntrosAndMatches(_config.ExperienceIntros, matchedExperiences)}\n" +
                 "\n" +
                $"\tI am extremely interested in working with {company}. I believe I can be a strong asset to you and your company. " +
                $"I look forward to demonstrating my skills further and meeting you and the {textInfo.ToTitleCase(company)} development team. " +
                 "Thank you for your consideration and I look forward to hearing from you." +
                 "\n\n" +
                 "Sincerely,\n" +
                $"{_config.Name}";

            var outputPath = _config.OutputDirectoryPath;
            if (_config.OutputDirectoryPath == string.Empty)
            {
                outputPath = ".\\CoverLetters";
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string outputFullPath = $"{outputPath}\\{company} - {jobTitle}";   
            if (UrlId != null)
            {
                outputFullPath += $" - {UrlId}";
            }
            outputFullPath += ".txt";

            outputFullPath = Regex.Replace(outputFullPath, @"[\/:*?<>|]", " ");
            File.WriteAllTextAsync(outputFullPath, coverLetter);

            Console.WriteLine("\nFinished\n");
        }

        private static string GetCombinedIntrosAndMatches(string[] intros, List<string> matches)
        {
            string returnVal = "";

            for (int i = 0; i < intros.Length; i++)
            {
                if (matches.Count > i)
                {
                    string match = matches.ElementAt(i);
                    if (intros[i] == "")
                    {
                        returnVal += char.ToUpper(match[0]) + match.Substring(1);
                    }
                    else
                    {
                        returnVal += intros[i];

                        if (!returnVal.EndsWith(' '))
                        {
                            returnVal  += ' ';
                        }

                        returnVal += match;
                    }
                }
                else
                {
                    returnVal += "**** Insuffient matches Found! ****";
                    return returnVal;
                }

                if (returnVal.Last() != '.')
                {
                    returnVal += ".";
                }

                if (i != intros.Length - 1)
                {
                    returnVal += " ";
                }
            }

            return returnVal;
        }

        private static List<string> MatchRegexes(string jobDescription, List<KeyValuePair<Regex, string>> regexes)
        {
            var matches = new List<string>();
            foreach (var regex in regexes)
            {
                if (regex.Key.Match(jobDescription).Success)
                {
                    Console.WriteLine($"Matched: {regex.Key}");
                    matches.Add(regex.Value);
                }
            }

            return matches;
        }

        private static List<KeyValuePair<Regex, string>> ParseRegexesConfig(Dictionary<string, string> regexes)
        {
            var list = new List<KeyValuePair<Regex, string>>();

            if (regexes is null)
            {
                return list;
            }
            
            foreach (var regexStringPair in regexes)
            {
                try
                {
                    list.Add(new KeyValuePair<Regex, string>(new Regex(regexStringPair.Key, RegexOptions.IgnoreCase), regexStringPair.Value));
                } catch (Exception e)//ArgumentException)
                {
                    Console.WriteLine($"Invalid Regex: {regexStringPair.Key}");
                }
            }

            return list;
        }
    }
}
