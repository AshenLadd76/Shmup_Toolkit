using System.Collections;
using ToolBox.Helpers;

namespace ToolBox.Services.Web
{
    /// <summary>
    /// Interface for handling web requests asynchronously.
    /// </summary>
    public interface IWebService
    {
        public IServerResponseData ServerResponseData { get; }

        /// <summary>
        /// Sends a POST request to the specified URI with the given data.
        /// </summary>
        /// <param name="coroutineRunner"></param>
        /// <param name="postData">The data to include in the POST request.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        IEnumerator Post(ICoroutineRunner coroutineRunner,  IFormData postData);

        /// <summary>
        /// Sends a PUT request to the specified URI with the given data.
        /// </summary>
        /// <param name="coroutineRunner"></param>
        /// <param name="putData">The data to include in the PUT request.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        IEnumerator Put(ICoroutineRunner coroutineRunner, IFormData putData);

        /// <summary>
        /// Sends a GET request to the specified URL.
        /// </summary>
        /// <param name="coroutineRunner"></param>
        /// <param name="url"></param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        IEnumerator Get(ICoroutineRunner coroutineRunner, string url);

        /// <summary>
        /// Gets the server response received from the last web request.
        /// </summary>
        /// <returns>The server response as a string.</returns>
        string GetServerResponse();

        /// <summary>
        /// Gets the server response code from the last web request.
        /// </summary>
        /// <returns>The server response code as an int.</returns>
        int GetServerResponseCode();

        /// <summary>
        /// Gets the server response status from the last web request.
        /// </summary>
        /// <returns>The server response code as a bool.</returns>
        bool GetResponseStatus();

    }
}