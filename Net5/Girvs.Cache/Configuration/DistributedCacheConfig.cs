﻿using System.Text.Json.Serialization;
using Girvs.Configuration;

namespace Girvs.Cache.Configuration
{
    /// <summary>
    /// Represents distributed cache configuration parameters
    /// </summary>
    public class DistributedCacheConfig : IAppModuleConfig
    {
        /// <summary>
        /// Gets or sets a distributed cache type
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DistributedCacheType DistributedCacheType { get; set; } = DistributedCacheType.Redis;

        /// <summary>
        /// Gets or sets a value indicating whether we should use distributed cache
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets connection string. Used when distributed cache is enabled
        /// </summary>
        public string ConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";

        /// <summary>
        /// Gets or sets schema name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
        /// </summary>
        public string SchemaName { get; set; } = "dbo";

        /// <summary>
        /// Gets or sets table name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
        /// </summary>
        public string TableName { get; set; } = "DistributedCache";
    }
}