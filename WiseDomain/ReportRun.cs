﻿using System;
using System.Collections.Generic;

namespace WiseDomain
{
    public class ReportRun
    {
        public int Id { get; set; }

        public ReportConfiguration Report { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public DateTime? QueryTimeFrom { get; set; }

        public DateTime? QueryTimeTo { get; set; }

        public List<ParameterValue> CustomParameterValues { get; set; }
    }

    public struct ParameterValue
    {
        public int Id { get; set; }

        public object Value { get; set; }
    }
}