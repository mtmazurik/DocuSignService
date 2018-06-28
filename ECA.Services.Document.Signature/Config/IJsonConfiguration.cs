namespace ECA.Services.Document.Signature.Config
{
    public interface IJsonConfiguration
    {
        string ApiUrl { get; }
        string IntegrationKey { get; }
        string ConnectionString { get; }
        double TaskManagerIntervalSeconds { get; }
        string JwtSecretKey { get; }
        string JwtIssuer { get; }
    }
}