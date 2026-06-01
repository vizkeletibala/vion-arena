namespace Vitrial.Networking
{
    /// <summary>
    /// Runtime connection values consumed by the client bootstrap once networking is enabled.
    /// Kept transport-agnostic so the Milestone 1 local prototype can expose server prep without
    /// choosing Netcode, Mirror, Steam, or a custom socket layer yet.
    /// </summary>
    public readonly struct ClientConnectionConfig
    {
        public ClientConnectionConfig(string host, int port, bool secureTransport, string playerAlias)
        {
            Host = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host;
            Port = port <= 0 ? 7777 : port;
            SecureTransport = secureTransport;
            PlayerAlias = string.IsNullOrWhiteSpace(playerAlias) ? "Player" : playerAlias;
        }

        public string Host { get; }
        public int Port { get; }
        public bool SecureTransport { get; }
        public string PlayerAlias { get; }

        public static ClientConnectionConfig Localhost => new ClientConnectionConfig("127.0.0.1", 7777, false, "Player");
    }
}
