using System;

namespace WiseDomain
{
    public class DataProviderConfiguration
    {
        public int DataProviderConfigurationId { get; set; }

        public string DataProviderType { get; set; }

        public string ConnectionString { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}