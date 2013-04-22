using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMachine
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Helper class for parsing HTTP messages. </summary>
    ///
    /// <remarks>   Aleksander, 22.04.2013. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public class HttpMessageParser : IHttpParserDelegate
    {
        HttpParser _parser;
        HttpMessage _message;
        Action<IHttpMessage> _receiver;
        string _header;
        readonly Queue<IHttpMessage> _messages;

        public HttpMessageParser()
        {
            _parser = new HttpParser(this);
            _message = new HttpMessage();
            _messages = new Queue<IHttpMessage>();
        }

        void IHttpParserDelegate.OnMessageBegin(HttpParser parser)
        {
            // Ignore
        }

        void IHttpParserDelegate.OnMethod(HttpParser parser, string method)
        {
            _message.Method = method;
        }

        void IHttpParserDelegate.OnRequestUri(HttpParser parser, string requestUri)
        {
            _message.RequestUri = requestUri;
        }

        void IHttpParserDelegate.OnPath(HttpParser parser, string path)
        {
            _message.RequestPath = path;
        }

        void IHttpParserDelegate.OnFragment(HttpParser parser, string fragment)
        {
            _message.Fragment = fragment;
        }

        void IHttpParserDelegate.OnQueryString(HttpParser parser, string queryString)
        {
            _message.QueryString = queryString;
        }

        void IHttpParserDelegate.OnHeaderName(HttpParser parser, string name)
        {
            _header = name;
        }

        void IHttpParserDelegate.OnHeaderValue(HttpParser parser, string value)
        {
            _message.Headers.Add(new KeyValuePair<string, string>(_header, value));
        }

        void IHttpParserDelegate.OnHeadersEnd(HttpParser parser)
        {
            // Ignore
        }

        void IHttpParserDelegate.OnBody(HttpParser parser, ArraySegment<byte> data)
        {
            _message.BodyBytes.Add(data);
        }

        void IHttpParserDelegate.OnMessageEnd(HttpParser parser)
        {
            // Create stream
            var length = _message.BodyBytes.Aggregate(0, (s, b) => s + b.Count);
            byte[] bs = new byte[length];
            if (length > 0)
            {
                int where = 0;
                foreach (var buf in _message.BodyBytes)
                {
                    Buffer.BlockCopy(buf.Array, buf.Offset, bs, where, buf.Count);
                    where += buf.Count;
                }
            }
            _message.Body = new MemoryStream(bs, 0, bs.Length, false, false);
            _message.Body.Position = 0;
            _message.BodyBytes = null;


            _message.HttpVersion = new Version(parser.MajorVersion, parser.MinorVersion);
            _message.ShouldKeepAlive = parser.ShouldKeepAlive;
            _messages.Enqueue(_message);
            _message = new HttpMessage();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the input-data and returns a list of 0 or more requrests contained in the data. Not
        /// thread-safe.
        /// </summary>
        ///
        /// <param name="data"> The byte-data to be parsed. </param>
        ///
        /// <returns>   A list of 0 or more requests found in the passed data. </returns>
        ///-------------------------------------------------------------------------------------------------
        public IEnumerable<IHttpMessage> Execute(ArraySegment<byte> data)
        {
            long totalLength = 0;
            List<IHttpMessage> result = new List<IHttpMessage>();
            while (true)
            {
                totalLength += _parser.Execute(data);
                while (_messages.Count > 0)
                    result.Add(_messages.Dequeue());

                if (totalLength >= data.Count)
                    return result;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the input-data and returns a list of 0 or more requrests contained in the data. Not
        /// thread-safe.
        /// </summary>
        ///
        /// <param name="data"> The byte-data to be parsed. </param>
        ///
        /// <returns>   A list of 0 or more requests found in the passed data. </returns>
        ///-------------------------------------------------------------------------------------------------
        public IEnumerable<IHttpMessage> Execute(byte[] data)
        {
            return Execute(new ArraySegment<byte>(data));
        }
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Represents a parsed HTTP request. </summary>
    ///
    /// <remarks>   
    /// The IHttpMessage interface provides all known data about a parsed HTTP request.
    /// </remarks>
    ///-------------------------------------------------------------------------------------------------
    public interface IHttpMessage
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the HTTP data transfer method (such as GET, POST, or HEAD). </summary>
        ///
        /// <value> The HTTP data transfer method. </value>
        ///-------------------------------------------------------------------------------------------------
        string Method { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the raw URL of the current request. </summary>
        ///
        /// <value> The request uri. </value>
        ///-------------------------------------------------------------------------------------------------
        string RequestUri { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the full pathname of the request file. </summary>
        ///
        /// <value> The full pathname of the request file. </value>
        ///-------------------------------------------------------------------------------------------------
        string RequestPath { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the path fragment. </summary>
        ///
        /// <value> The path fragment. </value>
        ///-------------------------------------------------------------------------------------------------
        string Fragment { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the query string. </summary>
        ///
        /// <value> The query string. </value>
        ///-------------------------------------------------------------------------------------------------
        string QueryString { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the headers. </summary>
        ///
        /// <value> The headers. </value>
        ///-------------------------------------------------------------------------------------------------
        IList<KeyValuePair<string, string>> Headers { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the body. </summary>
        ///
        /// <value> The body. </value>
        ///-------------------------------------------------------------------------------------------------
        Stream Body { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the http version. </summary>
        ///
        /// <value> The http version. </value>
        ///-------------------------------------------------------------------------------------------------
        Version HttpVersion { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a value indicating whether we should keep alive the connection. </summary>
        ///
        /// <value> true if we should keep alive the connection, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        bool ShouldKeepAlive { get; }
    }

    class HttpMessage : IHttpMessage
    {
        public HttpMessage()
        {
            Headers = new List<KeyValuePair<string, string>>();
            BodyBytes = new List<ArraySegment<byte>>();
        }

        public List<ArraySegment<byte>> BodyBytes { get; internal set; }

        public string Method { get; internal set; }
        public string RequestUri { get; internal set; }
        public string RequestPath { get; internal set; }
        public string Fragment { get; internal set; }
        public string QueryString { get; internal set; }
        public IList<KeyValuePair<string, string>> Headers { get; private set; }
        public Stream Body { get; internal set; }

        public Version HttpVersion { get; internal set; }
        public bool ShouldKeepAlive { get; internal set; }
    }
}
