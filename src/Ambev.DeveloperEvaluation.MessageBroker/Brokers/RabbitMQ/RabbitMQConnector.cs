using RabbitMQ.Client;

namespace Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;

/// <summary>
/// Manages the connection to RabbitMQ.
/// Ensures a single connection is reused throughout the application.
/// </summary>
public class RabbitMQConnector
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;

    public RabbitMQConnector(
        string hostName,
        string userName,
        string password)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    /// <summary>
    /// Gets or creates a RabbitMQ connection.
    /// </summary>
    /// <returns>An active RabbitMQ connection.</returns>
    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
        }

        return _connection;
    }

    /// <summary>
    /// Disposes of the RabbitMQ connection when no longer needed.
    /// </summary>
    public void DisposeConnection()
    {
        if (_connection != null && _connection.IsOpen)
        {
            _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}