namespace AsyncApi.Worker
{
    public class ClientConfiguration
    {
        public Clients Client { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Version { get; set; }
        public string Endpoint { get; set; }
    }
}
