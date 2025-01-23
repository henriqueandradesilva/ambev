namespace Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;

/// <summary>
/// Represents the settings required to configure a RabbitMQ connection.
/// </summary>
public class RabbitMQSettings
{
    /// <summary>
    /// Gets or sets the port number used to connect to RabbitMQ.
    /// </summary>
    public string Port { get; set; }

    /// <summary>
    /// Gets or sets the full URL of the RabbitMQ port.
    /// </summary>
    public string PortUrl { get; set; }

    /// <summary>
    /// Gets or sets the username for authenticating with RabbitMQ.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password for authenticating with RabbitMQ.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the hostname of the RabbitMQ server.
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// Gets or sets the virtual host used within RabbitMQ.
    /// </summary>
    public string VirtualHost { get; set; }

    /// <summary>
    /// Gets or sets the interval for health checks on RabbitMQ, in seconds.
    /// </summary>
    public string CheckInterval { get; set; }

    /// <summary>
    /// Gets or sets the URL of the Web API used in conjunction with RabbitMQ.
    /// </summary>
    public string UrlWebApi { get; set; }
}
