using Microsoft.Extensions.DependencyInjection;

namespace Incident.ApiTests
{
    public class TestOfBase
    {
        protected ServiceProvider ServiceProvider { get; }
        protected TestOfBase(DIFixture dIFixture)
        {
            ServiceProvider = dIFixture.ServiceProvider;
        }
    }
}
