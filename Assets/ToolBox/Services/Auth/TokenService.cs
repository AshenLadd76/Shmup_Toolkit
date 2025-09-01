using CodeBase.Services.Encryption;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Services.Auth
{
    /// <summary>
    /// Service for managing authentication tokens with encryption.
    /// </summary>
    
    public class TokenService : ITokenService
    {
        private const string AuthTokenPrefs = "AuthToken";
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="encryptionService">The encryption service used for encrypting and decrypting tokens.</param>
        public TokenService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }
        
        
        /// <summary>
        /// Sets the authentication token after encrypting it.
        /// </summary>
        /// <param name="token">The authentication token to be stored.</param>
        ///
        public void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Logger.LogError("Supplied token is empty or null");
                return;
            }
            
            Logger.Log( $"Setting Token : { token }" );
            
            var encryptedToken = _encryptionService?.Encrypt(token);
            
            if (encryptedToken != null)
            {
                PlayerPrefs.SetString(AuthTokenPrefs, encryptedToken);
                PlayerPrefs.Save();
            }
            else
            {
                Logger.LogError("Failed to encrypt token");
            }
        }
        
       
        /// <summary>
        /// Gets the decrypted authentication token.
        /// </summary>
        /// <returns>The decrypted authentication token, or an empty string if the token does not exist.</returns>
        public string GetToken()
        {
            if (!PlayerPrefs.HasKey(AuthTokenPrefs)) return string.Empty;

            var encryptedToken = PlayerPrefs.GetString(AuthTokenPrefs);
            
            return _encryptionService?.Decrypt(encryptedToken) ?? string.Empty;
        }
    }
}