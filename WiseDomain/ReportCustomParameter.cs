namespace WiseDomain
{
    public class ReportCustomParameter
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string QueryId { get; set; }

        public string QueryValue { get; set; }

        public ReportCustomParameterType Type { get; set; }

        public bool Required { get; set; }
    }
}