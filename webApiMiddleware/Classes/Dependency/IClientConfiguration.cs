using System;

public interface IClientConfiguration
{
    string ClientName { get; set; }
    DateTime InvokedDateTime { get; set; }
}