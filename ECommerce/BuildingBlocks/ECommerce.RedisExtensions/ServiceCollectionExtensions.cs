using ECommerce.RedisExtensions.Abstract;
using ECommerce.RedisExtensions.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ECommerce.RedisExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ECommerce_";
            });

            services.AddSingleton<ICacheService, RedisCacheManager>();
            return services;
        }
    }
}
