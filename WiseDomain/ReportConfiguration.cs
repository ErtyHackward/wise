using System;
using System.Collections.Generic;

namespace WiseDomain
{
    public class ReportConfiguration
    {
        public int Id { get; set; }

        public DataProviderConfiguration DataProvider { get; set; }

        public string Query { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<ReportCustomParameter> CustomParameters { get; set; }
    }
}