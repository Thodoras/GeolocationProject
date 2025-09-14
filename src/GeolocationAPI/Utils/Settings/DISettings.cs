using System.Data.Common;

public class DISettings
{
    public int MaxConcurrency { get; set; }
    public int UpdateInterval { get; set; }
    public int minDelayBetweenRequestsInMilliseconds { get; set; }
    public required string FreeGeoIPURL { get; set; }
    public required string FreeGeoIPKey { get; set; }

    public required string MSQLExpressServer { get; set; }
    public required string MSQLExpressDatabase { get; set; }
    public required bool MSQLExpressTrustServer { get; set; }
    public required string MSQLExpressTrustServerCertificate {  get; set; }

    public string GetMSSQLExpressConnectionString()
    {
        var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (runningInContainer)
        {
            return $"Server=sql-server;Database={MSQLExpressDatabase};User Id=sa;Password=YourPassword123;TrustServerCertificate=True;";
        }
        
        var connectionString = $"Server={MSQLExpressServer};Database={MSQLExpressDatabase};Trusted_Connection={MSQLExpressTrustServer};TrustServerCertificate={MSQLExpressTrustServerCertificate}";
        return connectionString;
    }
}