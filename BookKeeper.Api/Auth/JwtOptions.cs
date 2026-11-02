namespace BookKeeper.Api.Auth;

// The <Section> section in the configuration. Program.cs checks it when the API starts.
public class JwtOptions
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
}
