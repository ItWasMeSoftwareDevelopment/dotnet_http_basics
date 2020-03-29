namespace ItWasMe.HttpBasics
{
    public interface IProxyModel
    {
        string ProxyIp { get; set; }
        int ProxyPort { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
