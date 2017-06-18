using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Services
{
    public static class RegisterDependenciesExt
    {
        public static IServiceCollection RegisterServices(this IServiceCollection collection)
        {

            return collection;
        }
    }
}
