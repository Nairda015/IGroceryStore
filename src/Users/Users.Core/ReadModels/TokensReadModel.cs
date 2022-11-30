namespace IGroceryStore.Users.ReadModels
{
    public class TokensReadModel
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
    }
}
