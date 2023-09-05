namespace Source2Cover
{
    internal class Config
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string CityAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Github { get; set; }

        public Dictionary<string, string> GoalRegexes { get; set; }
        public Dictionary<string, string> SkillRegexes { get; set; }
        public Dictionary<string, string> ExperienceRegexes { get; set; }
        public string[] SkillIntros { get; set; }
        public string[] ExperienceIntros { get; set; }
        
        public string OutputDirectoryPath { get; set; }
    }
}
