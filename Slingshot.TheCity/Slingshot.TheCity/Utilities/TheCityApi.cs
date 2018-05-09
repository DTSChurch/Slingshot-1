using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
//using RestSharp.Extensions.MonoHttp;
using Slingshot.Core;
using Slingshot.Core.Model;
using Slingshot.Core.Utilities;
using Slingshot.TheCity.Utilities.Translators;

namespace Slingshot.TheCity.Utilities
{
    /// <summary>
    /// API F1 Status
    /// </summary>
    public static class TheCityApi
    {
        private static RestClient _client;
        private static RestRequest _request;
        private static int loopThreshold = 100;

        /// <summary>
        ///  Set F1Api.DumpResponseToXmlFile to true to save all API Responses 
        ///   to XML files and include them in the slingshot package
        /// </summary>
        /// <value>
        /// <c>true</c> if the response should get dumped to XML; otherwise, <c>false</c>.
        /// </value>
        public static bool DumpResponseToXmlFile { get; set; }

        /// <summary>
        /// Gets or sets the last run date.
        /// </summary>
        /// <value>
        /// The last run date.
        /// </value>
        public static DateTime LastRunDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the api counter.
        /// </summary>
        /// <value>
        /// The api counter.
        /// </value>
        public static int ApiCounter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the API URL.
        /// </summary>
        /// <value>
        /// The API URL.
        /// </value>
        public static string ApiUrl
        {
            get
            {
                return "https://api.onthecity.org";
            }
        }

        /// <summary>
        /// Gets or sets the API consumer key.
        /// </summary>
        /// <value>
        /// The API consumer key.
        /// </value>
        public static string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the API consumer secret.
        /// </summary>
        /// <value>
        /// The API consumer secret.
        /// </value>
        public static string ApiToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConnected { get; private set; } = false;

        #region API Call Paths 

        private const string API_USERS = "/users";

        #endregion

        /// <summary>
        /// Initializes the export.
        /// </summary>
        public static void InitializeExport()
        {
            ImportPackage.InitalizePackageFolder();
        }

        /// <summary>
        /// Connects the specified host name.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="apiUsername">The API username.</param>
        /// <param name="apiPassword">The API password.</param>
        public static void Connect( string apiKey, string apiToken )
        {
            ApiKey = apiKey;
            ApiToken = apiToken;

            int timestamp = GetTimeStamp();
            string HMAC = GenerateHMAC( "GET", ApiUrl + API_USERS, "/count", timestamp );

            _client = new RestClient( ApiUrl );
            _request = new RestRequest( API_USERS + "/count", Method.GET );
            _request.AddHeader( "X-City-Sig", HMAC );
            _request.AddHeader( "X-City-User-Token", ApiToken );
            _request.AddHeader( "X-City-Time", timestamp.ToString() );
            _request.AddHeader( "Accept", "application/vnd.thecity.admin.v1+json" );

            // getting the api status sets the IsConnect flag
            UpdateApiStatus();
        }

        /// <summary>
        /// Updates the API status.
        /// </summary>
        public static void UpdateApiStatus()
        {
            // execute the request to get a oauth token and secret
            var response = _client.Execute( _request );

            if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                IsConnected = true;
            }
            else
            {
                ErrorMessage = response.Content;
            }
        }

        /// <summary>
        /// Exports the individuals.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        public static void ExportIndividuals( DateTime modifiedSince )
        {
            int currentPage = 1;
            int loopCounter = 0;
            bool moreIndividualsExist = true;

            try
            {
                while ( moreIndividualsExist )
                {
                    string queryParameters = string.Format( "?filter=updated_between_{0}_{1}&page={2}",
                        modifiedSince.ToString( "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture ) , DateTime.Now.ToString( "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture ), currentPage );

                    int timestamp = GetTimeStamp();
                    string HMAC = GenerateHMAC( "GET", ApiUrl + API_USERS, queryParameters, timestamp );

                    _client = new RestClient( ApiUrl );
                    _request = new RestRequest( API_USERS + queryParameters, Method.GET );
                    _request.AddHeader( "X-City-Sig", HMAC );
                    _request.AddHeader( "X-City-User-Token", ApiToken );
                    _request.AddHeader( "X-City-Time", timestamp.ToString() );
                    _request.AddHeader( "Accept", "application/vnd.thecity.admin.v1+json" );

                    var response = _client.Execute( _request );
                    ApiCounter++;

                    XDocument xdoc = XDocument.Load( JsonReaderWriterFactory.CreateJsonReader(
                        Encoding.ASCII.GetBytes( response.Content ), new XmlDictionaryReaderQuotas() ) );

                    if ( TheCityApi.DumpResponseToXmlFile )
                    {
                        xdoc.Save( Path.Combine( ImportPackage.PackageDirectory, $"API_INDIVIDUALS_ResponseLog_{loopCounter}.xml" ) );
                    }

                    var records = xdoc.Element( "root" ).Element( "users" );

                    if ( records != null )
                    {
                        var returnCount = xdoc.Element( "root" ).Element( "per_page" )?.Value.AsIntegerOrNull();
                        var additionalPages = xdoc.Element( "root" ).Element( "total_pages" ).Value.AsInteger();

                        if ( returnCount.HasValue )
                        {
                            foreach ( var personNode in records.Elements() )
                            {
                                if ( personNode.Element( "id" ) != null && personNode.Element( "id" ).Value.AsIntegerOrNull() > 0 && 
                                     personNode.Element( "type" ).Value != "OfflineUser" && personNode.Element( "last" ).Value.Length > 1 )
                                {
                                    // get family
                                    int personId = personNode.Element( "id" ).Value.AsInteger();
                                    int fmtimestamp = GetTimeStamp();
                                    string fmHMAC = GenerateHMAC( "GET", ApiUrl + API_USERS + "/" + personId.ToString() + "/family", "", fmtimestamp );

                                    _client = new RestClient( ApiUrl );
                                    _request = new RestRequest( API_USERS + "/" + personId.ToString() + "/family", Method.GET );
                                    _request.AddHeader( "X-City-Sig", fmHMAC );
                                    _request.AddHeader( "X-City-User-Token", ApiToken );
                                    _request.AddHeader( "X-City-Time", fmtimestamp.ToString() );
                                    _request.AddHeader( "Accept", "application/vnd.thecity.admin.v1+json" );

                                    var fmresponse = _client.Execute( _request );
                                    ApiCounter++;

                                    XDocument fmxdoc = XDocument.Load( JsonReaderWriterFactory.CreateJsonReader(
                                        Encoding.ASCII.GetBytes( fmresponse.Content ), new XmlDictionaryReaderQuotas() ) );


                                    var importPerson = TheCityPerson.Translate( personNode, fmxdoc.Element( "root" ) );

                                    if ( importPerson != null )
                                    {
                                        ImportPackage.WriteToPackage( importPerson );
                                    }
                                }
                            }

                            if ( additionalPages <= 0 && returnCount <= 0 )
                            {
                                moreIndividualsExist = false;
                            }
                            else
                            {
                                currentPage++;
                            }
                        }
                    }
                    else
                    {
                        moreIndividualsExist = false;
                    }

                    // developer safety blanket (prevents eating all the api calls for the day) 
                    if ( loopCounter > loopThreshold )
                    {
                        break;
                    }
                    loopCounter++;
                }

            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }
        }

        private static int GetTimeStamp()
        {
            return ( int ) ( DateTime.UtcNow.Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalSeconds;
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

            var encoding = Encoding.UTF8;
            var HMACSha256 = new HMACSHA256( encoding.GetBytes( ApiKey ) );
            var stream = new MemoryStream( encoding.GetBytes( stringToSign ) );
            var unencodedHMAC = HMACSha256.ComputeHash( stream );

            // Base 64 encoding the HMAC output
            var unescapedHMAC = Convert.ToBase64String( unencodedHMAC ).TrimEnd();

            // URL Encoding the Base64 encoded HMAC code
            var HMACSignature = Uri.EscapeDataString( unescapedHMAC );

            return HMACSignature;
        }
    }
}
