using System.Collections;
using ToolBox.Helpers;
using ToolBox.Services.Auth;
using UnityEngine.Networking;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Services.Web
{
    public class WebService : IWebService
    {
        private const string ContentType = "Content-Type";
        private const string Authorization = "Authorization";
        private const string Bearer = "Bearer";
        private const string AppJson = "application/json";

        private ISerializer _serializer;
        private ICoroutineRunner _coroutineRunner;
        private ITokenService _tokenService;

        private string _token = "";
        
        public WebService(ISerializer serializer, ITokenService tokenService)
        {
            _serializer = serializer;
            _tokenService = tokenService;
        }
        
        public IEnumerator Post(ICoroutineRunner coroutineRunner, IFormData formData) => Send(coroutineRunner, formData, UnityWebRequest.kHttpVerbPOST);
        
        public IEnumerator Put(ICoroutineRunner coroutineRunner, IFormData formData) => Send(coroutineRunner, formData, UnityWebRequest.kHttpVerbPUT);
        
        public IEnumerator Get(ICoroutineRunner coroutineRunner, string url)
        {
            if (coroutineRunner == null)
            {
                Logger.LogError("Coroutine runner not set");
                yield break;
            }
    
            if (string.IsNullOrWhiteSpace(url))
            {
                Logger.LogError("URL is null or empty");
                yield break;
            }

            using UnityWebRequest request = UnityWebRequest.Get(url);

            yield return coroutineRunner.StartCoroutine(SendRequest(request));
        }
        
        private IEnumerator Send(ICoroutineRunner coroutineRunner, IFormData data, string requestType )
        {
            if (coroutineRunner == null)
            {
                Logger.LogError( "Coroutine runner not set" );
                yield break;
            }

            if (string.IsNullOrWhiteSpace(requestType))
            {
                Logger.LogError( $"Request type is null or empty" );
                yield break;
            }
            
            UnityWebRequest request = new UnityWebRequest( data.Url,  requestType );
            
            SetRequestBody(request, data.JsonData);
            
            try
            {
                yield return coroutineRunner.StartCoroutine(SendRequest(request));
            }
            finally
            {
                request.Dispose(); // ensures cleanup even if SendRequest fails
            }
        }
        
        private IEnumerator SendRequest(UnityWebRequest request)
        {
            yield return null;
            
            SetRequestHeader(request);
            
            yield return request.SendWebRequest();
            
            HandleServerResponse();
        }

        private void SetRequestHeader(UnityWebRequest request)
        {
            var serverToken = GetToken();
           
            request.SetRequestHeader(Authorization, $"{Bearer} {serverToken}");
        }

        private void SetRequestBody(UnityWebRequest request, string jsonData )
        {
            if (request == null)
            {
                Logger.LogError("UnityWebRequest is null in SetData.");
                return;
            }

            if (string.IsNullOrEmpty(jsonData))
            {
                Logger.LogWarning("Empty or null JSON data passed to SetData.");
                jsonData = "{}"; // fallback to empty JSON object if desired
            }
            
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader(ContentType, AppJson);
        }
        
        private void HandleServerResponse()
        {
            Logger.Log( $"Handling server response....." );
        }
        
        public IServerResponseData ServerResponseData { get; }
        
        public string GetServerResponse()
        {
            throw new System.NotImplementedException();
        }

        public int GetServerResponseCode()
        {
            throw new System.NotImplementedException();
        }

        public bool GetResponseStatus()
        {
            throw new System.NotImplementedException();
        }
        
        private void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
                _tokenService?.SetToken( token );
        }

        private string GetToken() => _tokenService?.GetToken() ?? string.Empty;
    }
}
