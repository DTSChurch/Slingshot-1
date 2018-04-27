using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Slingshot.TheCity.Utilities
{
    public static class TheCityApi
    {

        private static string secretKey { get; set; } = "6dbb51307527dd14cced2e529e839b23db761d86";
        private static string userToken { get; set; } = "07c4b44282a4a217";
        private static int ApiCounter { get; set; } = 0;
        private static string ApiUrl { get; set; } = "https://api.onthecity.org";

        private static RestClient _client;

        /// <summary>
        /// Exports the users
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        public static void ExportUsers( DateTime modifiedSince )
        {
            const string uri = "users";
            int totalPages = 0;
            int currentPage = 1;
            bool done = false;

            while ( !done || currentPage < 100 )
            {
                // Parsing Parameters
                string modifiedDate = modifiedSince.ToString( "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture );
                string queryParameters = string.Format( "?filter=created_between_{0}_{1}&include_participation=true&page={2}", modifiedDate, DateTime.Now.ToString( "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture ), currentPage);

                // Creating timestamp
                int timestamp = ( int ) ( DateTime.UtcNow.Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalSeconds;

                // Getting HMAC
                string HMAC = GenerateHMAC( "GET", ApiUrl + "/" + uri, queryParameters, timestamp );

                // Creating client and request
                _client = new RestClient( ApiUrl );
                var request = new RestRequest( uri + queryParameters, Method.GET );
                request.AddHeader( "X-City-Sig", HMAC );
                request.AddHeader( "X-City-User-Token", userToken );
                request.AddHeader( "X-City-Time", timestamp.ToString() );
                request.AddHeader( "Accept", "application/vnd.thecity.admin.v1+json" );

                // Making request
                var restResponse = _client.Execute( request );

                // Making sure request was successful
                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK )
                {
                    // Getting Data from request
                    Models.UsersModel users = JsonConvert.DeserializeObject<Models.UsersModel>( restResponse.Content );
                    var rateLimitRemaining = restResponse.Headers.Where( x => x.Name == "X-City-Ratelimit-Remaining-By-Account" ).Single().Value;

                    // Checking if this is the initial iteration
                    if (totalPages == 0 )
                    {
                        totalPages = users.TotalPages;
                        currentPage++;
                    }
                    else
                    {
                        currentPage++;
                    }

                    // Looping through people
                    foreach(var user in users.Users )
                    {
                        // Checking if user is an offline "system" user
                        if( (user.Type != "OfflineUser") && (user.Last.Length != 1) )
                        {
                            // Here is where the transforms need to be added

                            Console.WriteLine( string.Format( "{0},{1} ({2}) - {3} - {4}", user.Last, user.First, user.Type, user.Email, user.CreatedAt ) );
                        }
                    }

                    Console.WriteLine( "##############" + rateLimitRemaining + "##############" );
                }
                else
                {
                    throw new System.Exception( "Request to The City was not successful. HTTP Status code: " + restResponse.StatusCode.ToString() );
                }

                // Incrementing API Counter
                ApiCounter++;
            }
        }

        /// <summary>
        /// Exports the users
        /// </summary>
        /// <param name="httpVerb">The HTTP verb we will be using.</param>
        /// <param name="uri">The full URL we will be querying.</param>
        /// <param name="queryParameters">The query parameters that will be used.</param>
        /// <param name="timestamp">The timestamp of the request.</param>
        private static string GenerateHMAC( string httpVerb, string uri, string queryParameters, int timestamp )
        {
            // Validaing app params are passed
            if ( string.IsNullOrEmpty( httpVerb ) || string.IsNullOrEmpty( uri ) )
            {
                return null;
            }

            // Concatenating time + verb + host + path + query_params + body
            var stringToSign = string.Format( "{0}{1}{2}{3}", timestamp, httpVerb, uri, queryParameters );

            // Constructing HMAC signature
            /*
            var encoding = GetIso88591Encoding();
            byte[] secretKeyBytes = Encoding.Default.GetBytes( secretKey );
            HMACSHA256 HMACSha = new HMACSHA256( secretKeyBytes );
            byte[] inputByteArray = encoding.GetBytes( stringToSign );
            MemoryStream stream = new MemoryStream( inputByteArray );
            byte[] hashValue = HMACSha.ComputeHash( stream );
            string unencodedHMAC = encoding.GetString( hashValue );

            byte[] unencodedHMACBytes = encoding.GetBytes( unencodedHMAC );
            string unescapedHMAC = Convert.ToBase64String( unencodedHMACBytes ).Trim();

            string HMACSignature = Uri.EscapeDataString( unescapedHMAC );
            */

            var encoding = Encoding.UTF8;
            var HMACSha256 = new HMACSHA256( encoding.GetBytes( secretKey ) );
            var stream = new MemoryStream( encoding.GetBytes( stringToSign ) );
            var unencodedHMAC = HMACSha256.ComputeHash( stream );

            // Base 64 encoding the HMAC output
            var unescapedHMAC = Convert.ToBase64String( unencodedHMAC ).TrimEnd();

            // URL Encoding the Base64 encoded HMAC code
            var HMACSignature = Uri.EscapeDataString( unescapedHMAC );

            return HMACSignature;
        }

        private static System.Text.Encoding GetIso88591Encoding()
        {
            var encodingInfo = System.Text.Encoding.GetEncodings().FirstOrDefault( e => e.Name.ToUpperInvariant() == "ISO-8859-1" );

            if ( encodingInfo == null )
            {
                throw new Exception( "Could not retrieve ISO-8859-1 text encoding" );
            }

            return encodingInfo.GetEncoding();
        }
    }
}
