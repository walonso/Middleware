using System;

public class ClientConfiguration : IClientConfiguration
{
    public string ClientName { get; set; }

    public DateTime InvokedDateTime { get; set; }
}