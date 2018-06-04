using Akka.Actor;
using Autofac;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;

namespace Lykke.Job.EthereumSamurai.Modules
{
    public sealed class ActorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(IActorRole).Assembly)
                .AssignableTo<IActorRole>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(typeof(IChildActorFactory).Assembly)
                .AssignableTo<IChildActorFactory>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(typeof(IChildActorFactory).Assembly)
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ActorBase)));
        }
    }
}
