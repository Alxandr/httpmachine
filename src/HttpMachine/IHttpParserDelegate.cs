using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Interface for HTTP parser delegate. </summary>
    ///
    /// <remarks>   The HTTP parser delegate is used to provide callback-based processing of HTTP requests. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public interface IHttpParserDelegate
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that a new message has begun. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnMessageBegin(HttpParser parser);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that the HTTP method has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="method">   The HTTP method. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnMethod(HttpParser parser, string method);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that the request URI has been parsed. </summary>
        ///
        /// <param name="parser">       The HTTP parser. </param>
        /// <param name="requestUri">   URI of the request. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnRequestUri(HttpParser parser, string requestUri);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that the path has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="path">     Full pathname of the file. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnPath(HttpParser parser, string path);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that the fragment has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="fragment"> The fragment. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnFragment(HttpParser parser, string fragment);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that the querystring has been parsed. </summary>
        ///
        /// <param name="parser">       The HTTP parser. </param>
        /// <param name="queryString">  The querystring. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnQueryString(HttpParser parser, string queryString);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that a header-name has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="name">     The header name. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnHeaderName(HttpParser parser, string name);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that a header-value has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="value">    The header value. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnHeaderValue(HttpParser parser, string value);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that all headers have been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnHeadersEnd(HttpParser parser);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that a portion of the body has been parsed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        /// <param name="data">     The partial body-data. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnBody(HttpParser parser, ArraySegment<byte> data);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Signifies that a message is completed. </summary>
        ///
        /// <param name="parser">   The HTTP parser. </param>
        ///-------------------------------------------------------------------------------------------------
        void OnMessageEnd(HttpParser parser);
    }
}
