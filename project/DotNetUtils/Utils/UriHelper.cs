using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DotNetUtils
{
    public static class UriHelper
    {
        public static Uri BuildWithQuery(string baseUri, params KeyValuePair<string, string>[] queryParameters)
        {
            Uri root;
            try
            {
                root = new Uri(baseUri);
            }
            catch (ArgumentNullException argNullEx)
            {
                throw new ArgumentNullException("baseUri is null", argNullEx);
            }
            catch (UriFormatException uriFormatEx)
            {
                throw new ArgumentOutOfRangeException("baseUri is invalid", uriFormatEx);
            }
            if (queryParameters.IsNullOrEmpty())
            {
                return root;
            }
            var builder = new UriBuilder(root);
            var queryBuilder = new StringBuilder();
            for (var i = 0; i < queryParameters.Length; i++)
            {
                queryBuilder.AppendFormat
                (
                    "{0}={1}",
                    HttpUtility.UrlEncode(queryParameters[i].Key),
                    HttpUtility.UrlEncode(queryParameters[i].Value)
                );
                if (i < queryParameters.Length - 1)
                {
                    queryBuilder.Append("&");
                }
            }
            builder.Query = queryBuilder.ToString();
            return builder.Uri;
        }

        public static string ReadableCharacterSafeUrlEncode(byte[] data)
        {
            var raw = Base32.ToBase32String(data);
            return raw;
        }

        public static byte[] ReadableCharacterSafeUrlDecode(string encoded)
        {
            try
            {
                var decodedData = Base32.FromBase32String(encoded);
                return decodedData;
            }
            catch
            {
                return null;
            }
        }
    }
}
