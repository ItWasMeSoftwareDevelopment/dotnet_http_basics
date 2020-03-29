namespace ItWasMe.HttpBasics
{
    public class ProxyModel : IProxyModel
    {
        public string ProxyIp { get; set; }
        public int ProxyPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
