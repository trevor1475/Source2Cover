using System.Text.RegularExpressions;

namespace Source2Cover.JobSites
{
    internal class JobBank : JobSite
    {
        internal override string UrlPattern => @"(https{0,1})(:\/\/\w{0,3}\.{0,1}jobbank\.gc\.ca\/)(.*)";
        internal override string JobDescriptionElementId => "responsibilities";
        internal override string JobTitleElementId => "noc-title";
        internal override string JobTitleElementType { get; } = ElementTypes.Span;
        internal override string CompanyElementId => "hiringOrganization";
        internal override string CompanyElementType { get; } = ElementTypes.Span;
        internal override string CityElementId => "addressLocality";
        internal override string CityElementType { get; } = ElementTypes.Span;
        internal string ProvinceElementId => "addressRegion";
        internal string ProvinceElementType { get; } = ElementTypes.Span;
        internal override string HtmlSignifier => "EDSC_JobBank";

        public JobBank() { }

        internal override string ParseElement(string elementId, string elementType, string source)
        {
            if (elementId == CityElementId)
            {
                var city = base.ParseElement(elementId, elementType, source);
                var province = base.ParseElement(ProvinceElementId, ProvinceElementType, source);
                return $"{city}, {province}";
            }

            return base.ParseElement(elementId, elementType, source);
        }
    }
}
