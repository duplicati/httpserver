using System;
using System.Collections.Specialized;

namespace HttpServer
{
    public static class HttpUtilityHelper
    {
        /// <summary>
        /// The regular expression that matches %20 type values in a querystring
        /// </summary>
        private static System.Text.RegularExpressions.Regex RE_ESCAPECHAR = new System.Text.RegularExpressions.Regex(@"[^0-9a-zA-Z]", System.Text.RegularExpressions.RegexOptions.Compiled);
        
        /// <summary>
        /// Encodes a URL, like System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <returns>The encoded URL</returns>
        /// <param name="value">The URL fragment to encode</param>
        /// <param name="encoding">The encoding to use</param>
        public static string UrlPathEncode(string value, System.Text.Encoding encoding = null)
        {
            return UrlEncode(value, encoding, "%20");
        }

        /// <summary>
        /// Converts a sequence of bytes to a hex string
        /// </summary>
        /// <returns>The array as hex string.</returns>
        /// <param name="data">The data to convert</param>
        private static string ByteArrayAsHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        /// <returns>The string as byte array.</returns>
        /// <param name="hex">The hex string</param>
        /// <param name="data">The parsed data</param>
        public static byte[] HexStringAsByteArray(string hex, byte[] data)
        {
            for (var i = 0; i < hex.Length; i += 2)
                data[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return data;
        }

        /// <summary>
        /// Encodes a URL, like System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <returns>The encoded URL</returns>
        /// <param name="value">The URL fragment to encode</param>
        /// <param name="encoding">The encoding to use</param>
        /// <param name="spacevalue">The value to encode a space as</param>
        public static string UrlEncode(string value, System.Text.Encoding encoding = null, string spacevalue = "+") 
        {
            if (value == null)
                throw new ArgumentNullException("value");
                
            encoding = encoding ?? System.Text.Encoding.UTF8;

            var encoder = encoding.GetEncoder();
            var inbuf = new char[1];               
            var outbuf = new byte[2];
            var shortout = new byte[1];
            
            return RE_ESCAPECHAR.Replace(value, (m) => {
                if (m.Value == " ")
                    return spacevalue;
                    
                inbuf[0] = m.Value[0];
                
                try 
                {
                    var len = encoder.GetBytes(inbuf, 0, 1, outbuf, 0, true);
                    if (len == 1)
                    {
                        shortout[0] = outbuf[0];
                        return "%" + ByteArrayAsHexString(shortout).ToLower();
                    }
                    else if (len == 2)
                        return "%u" + ByteArrayAsHexString(outbuf).ToLower();
                }
                catch
                {
                } 
                
                //Fallback
                return m.Value;
            });
            
        }

        /// <summary>
        /// The regular expression that matches %20 type values in a querystring
        /// </summary>
        private static System.Text.RegularExpressions.Regex RE_NUMBER = new System.Text.RegularExpressions.Regex(@"(\%(?<number>([0-9]|[a-z]|[A-Z]){2}))|(\+)|(\%u(?<number>([0-9]|[a-z]|[A-Z]){4}))", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// A helper for converting byte arrays to hex, vice versa
        /// </summary>
        private const string HEX_DIGITS_UPPER = "0123456789ABCDEF";

        /// <summary>
        /// Encodes a URL, like System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <returns>The decoded URL</returns>
        /// <param name="url">The URL fragment to decode</param>
        /// <param name="encoding">The encoding to use</param>
        public static string UrlDecode(string value, System.Text.Encoding encoding = null)
        {
            if (value == null)
                throw new ArgumentNullException("value");
                
            encoding = encoding ?? System.Text.Encoding.UTF8;
                
            var decoder = encoding.GetDecoder();
            var inbuf = new byte[2];
            var outbuf = new char[1];
            
            return RE_NUMBER.Replace(value, (m) => { 
                if (m.Value == "+")
                    return " ";

                try
                {
                    var hex = m.Groups["number"].Value;
                    HexStringAsByteArray(hex, inbuf);
                                 
                    decoder.GetChars(inbuf, 0, hex.Length / 2, outbuf, 0); 
                    return outbuf[0].ToString();
                }
                catch
                {
                }
                
                //Fallback
                return m.Value;
            });
            
        }
        
        /// <summary>
        /// The regular expression that matches a=b type values in a querystring
        /// </summary>
        private static System.Text.RegularExpressions.Regex RE_URLPARAM = new System.Text.RegularExpressions.Regex(@"(?<key>[^\=\&]+)(\=(?<value>[^\&]*))?", System.Text.RegularExpressions.RegexOptions.Compiled);
        
        /// <summary>
        /// Parses the query string.
        /// This is a duplicate of the System.Web.HttpUtility.ParseQueryString that does not work well on Mono
        /// </summary>
        /// <returns>The parsed query string</returns>
        /// <param name="query">The query to parse</param>
        public static NameValueCollection ParseQueryString(string query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (query.StartsWith("?"))
                query = query.Substring(1);
            if (string.IsNullOrEmpty(query))
                return new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
                
            var result = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            foreach(System.Text.RegularExpressions.Match m in RE_URLPARAM.Matches(query))
                result.Add(UrlDecode(m.Groups["key"].Value), UrlDecode(m.Groups["value"].Success ? m.Groups["value"].Value : ""));
            
            return result;            
        }        
    }
}

