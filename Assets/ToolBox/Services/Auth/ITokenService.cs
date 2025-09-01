namespace ToolBox.Services.Auth
{
    /// <summary>
    /// Interface for handling authentication tokens.
    /// </summary>
    /// 
    public interface ITokenService
    {
        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The token to be stored.</param>
        void SetToken(string token);

        /// <summary>
        /// Retrieves the current authentication token.
        /// </summary>
        /// <returns>The stored authentication token, or <c>string.empty</c> if no token is set.</returns>
        string GetToken();
    
    }
}
