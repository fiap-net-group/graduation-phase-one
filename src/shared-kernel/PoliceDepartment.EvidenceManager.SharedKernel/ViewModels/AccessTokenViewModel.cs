namespace PoliceDepartment.EvidenceManager.SharedKernel.ViewModels
{
    public class AccessTokenViewModel
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string UserId { get; set; }

        public AccessTokenViewModel()
        {
            TokenType = string.Empty;
            AccessToken = string.Empty;
            Expires = DateTime.MinValue;
            UserId = string.Empty;
        }

        public AccessTokenViewModel(string tokenType, string accessToken, DateTime expires, string userId)
        {
            TokenType = tokenType;
            AccessToken = accessToken;
            Expires = expires;
            UserId = userId;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(TokenType) &&
                   !string.IsNullOrWhiteSpace(AccessToken) &&
                   Expires != DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(UserId);
        }
    }
}
