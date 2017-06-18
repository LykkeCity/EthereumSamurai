using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb
{
    public static class RegisterDependenciesExt
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection collection)
        {

            return collection;
        }
    }
}
