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

        public User Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public AccessMode AccessMode { get; set; }

        public List<ReportCustomParameter> CustomParameters { get; set; }

        public List<ReportGroupJoin> ReportGroups { get; set; }
    }

    public class ReportGroup
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public AccessMode AccessMode { get; set; }

        public List<ReportGroupJoin> ReportGroups { get; set; }

        public List<ReportGroupUserGroupJoin> AllowedUserGroups { get; set; }
    }

    public class ReportGroupUserGroupJoin
    {
        public int ReportGroupId { get; set; }
        public ReportGroup ReportGroup { get; set; }

        public int UserGroupId { get; set; }
        public UserGroup Group { get; set; }
    }

    public class ReportGroupJoin
    {
        public int ReportId { get; set; }
        public ReportConfiguration ReportConfiguration { get; set; }

        public int GroupId { get; set; }
        public ReportGroup ReportGroup { get; set; }
    }


}