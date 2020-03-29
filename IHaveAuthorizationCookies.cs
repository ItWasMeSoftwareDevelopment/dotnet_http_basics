using System.Collections.Generic;
using System.Net;

namespace ItWasMe.HttpBasics
{
    public interface IHaveAuthorizationCookies
    {
        IReadOnlyCollection<Cookie> AllCookies { get; }
    }
}
