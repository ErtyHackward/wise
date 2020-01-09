using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiseDomain
{
    public class ReportResponse
    {
        public int ReportId { get; set; }

        public int RowsCount { get; set; }

        public int ColumnsCount { get; set; }

        public string FinalQuery { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AuthorLogin { get; set; }

        public string UserLogin { get; set; }

        public string ReportTitle { get; set; }

        public string Description { get; set; }

        public List<List<object>> PreviewValues { get; set; }

        public List<string> Columns { get; set; }

        public int GenerationTimeMs { get; set; }

        public string ErrorText { get; set; }
    }
}
