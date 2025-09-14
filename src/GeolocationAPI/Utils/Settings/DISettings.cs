public class DISettings
{
    public int MaxConcurrency { get; set; }
    public int UpdateInterval { get; set; }
    public int minDelayBetweenRequestsInMilliseconds { get; set; }

    public required string MSQLExpressServer { get; set; }
    public required string MSQLExpressDatabase { get; set; }
    public required bool MSQLExpressTrustServer { get; set; }
    public required string MSQLExpressTrustServerCertificate {  get; set; }

    public string GetMSSQLExpressConnectionString()
    {
        // TODO: add other string
        return $"Server={MSQLExpressServer};Database={MSQLExpressDatabase};Trusted_Connection={MSQLExpressTrustServer};TrustServerCertificate={MSQLExpressTrustServerCertificate}";
    }
}