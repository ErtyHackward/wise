using System;
using System.Collections.Generic;

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

        public static readonly List<ProviderType> BuiltInProviders = new List<ProviderType> { 
            new ProviderType { 
                CLRType = "Elasticsearch.Ado.ElasticSearchConnection, ElasticSearch.Ado", 
                TypeTitle = "ElasticSearch", 
                IconPath = "/images/elasticsearch.svg",
                SampleConnectionString = "Server=localhost;Port=9200;User=guest;Password=123456;"
            },
            new ProviderType { 
                CLRType = "ClickHouse.Ado.ClickHouseConnection, ClickHouse.Ado", 
                TypeTitle = "Clickhouse", 
                IconPath = "/images/clickhouse.svg",
                SampleConnectionString = "Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;Database=write_your_database;User=default;"
            },
            new ProviderType { 
                CLRType = "MySql.Data.MySqlClient.MySqlConnection, MySqlConnector", 
                TypeTitle = "MySQL", 
                IconPath = "/images/mysql.svg",
                SampleConnectionString = "Server=localhost;Database=write_you_database;Uid=write_login;Pwd=write_password;"
            },
        };
    }

    public struct ProviderType
    {
        public string CLRType { get; set; }

        public string TypeTitle { get; set; }

        public string IconPath { get; set; }

        public string SampleConnectionString { get; set; }
    }
    
}