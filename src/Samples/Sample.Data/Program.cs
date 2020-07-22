using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Twino.Extensions.Data;
using Twino.Ioc;

namespace Sample.Data
{
    public class MyContext : DbContext
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceContainer container = new ServiceContainer();

            //container.AddDataContextTransient
            //container.AddDataContextTransientPool
            container.AddDataContextScoped<MyContext>(o => o.UseNpgsql("connection-string"));
            container.AddDataContextScopedPool<MyContext>(o => o.UseNpgsql("connection-string"),
                                                          p =>
                                                          {
                                                              p.PoolMinSize = 20;
                                                              p.PoolMaxSize = 120;
                                                              p.MaximumLockDuration = TimeSpan.FromSeconds(60);
                                                              p.IdleTimeout = TimeSpan.FromSeconds(90);
                                                              p.WaitAvailableDuration = TimeSpan.FromMilliseconds(1500);
                                                              p.ExceedLimitWhenWaitTimeout = false;
                                                          });

            //create a scope for using Scoped registrations
            IContainerScope scope = container.CreateScope();
            
            //get db context from pool
            MyContext context = await container.Get<MyContext>(scope);
        }
    }
}