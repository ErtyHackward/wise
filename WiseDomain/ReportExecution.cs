using System;

namespace WiseDomain
{
    public class ReportExecution
    {
        public int ReportExecutionId { get; set; }

        public ReportConfiguration ReportConfiguration { get; set; }

        public DateTime ExecutionStartedAt { get; set; }

        public DateTime? ExecutionFinishedAt { get; set; }
    }
}