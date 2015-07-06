using System;
using System.ComponentModel;
using System.Configuration;
using System.Data.EntityClient;
using System.Diagnostics;

namespace DotNetUtils
{
    public static class ConfigurationHelper
    {
        private const string providerName = @"System.Data.SqlClient";

        public static EntityConnection CreateEntityConnection(string connectionString, string metaData)
        {
            var entityBuilder = new EntityConnectionStringBuilder
            {
                Provider = providerName,
                ProviderConnectionString = connectionString,
                Metadata = metaData
            };
            var ret = new EntityConnection(entityBuilder.ToString());
            ret.Open();
            return ret;
        }

        public static T GetValue<T>(string key, T defaultValue = default(T))
        {
            try
            {
                var val = ConfigurationManager.AppSettings[key];
                if (val == null)
                {
                    return defaultValue;
                }
                var convertor = TypeDescriptor.GetConverter(typeof(T));
                return (T)convertor.ConvertFromString(val);
            }
            catch(Exception ex)
            {
                Trace.TraceInformation("ConfigurationHelper.GetValue<{0}>() is throwing exception, will use default value. Exception: {1}",
                    typeof(T).FullName,
                    ex.ToString());
                return defaultValue;
            }
        }

        /// <summary>
        /// Get a named connection string from Configuration
        /// </summary>
        /// <param name="name">The connection string name.</param>
        /// <returns>
        ///     The connection string if it exists in Configuration. Null otherwise
        /// </returns>
        public static string GetConnectionString(this string name)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("name");
            }
            var setting = ConfigurationManager.ConnectionStrings[name];
            if (setting == null)
            {
                throw new ArgumentException("Cannot load configuration setting with name '{0}'".FormatInvariantCulture(name));
            }
            return setting.ConnectionString;
        }
    }
}
