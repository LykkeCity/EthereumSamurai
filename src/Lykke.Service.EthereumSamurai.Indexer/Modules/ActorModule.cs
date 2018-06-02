﻿using Akka.Actor;
using Autofac;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using Lykke.Job.EthereumSamurai.Roles.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Modules
{
    public sealed class ActorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IActorRole>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IChildActorFactory>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<Erc20BalanceIndexingActorFactory>()
                .As<IErc20BalanceIndexingActorFactory>()
                .SingleInstance();

            builder
                .RegisterType<Erc20ContractIndexingActorFactory>()
                .As<IErc20ContractIndexingActorFactory>()
                .SingleInstance();

            builder
                .RegisterType<BlockIndexingActorFactory>()
                .As<IBlockIndexingActorFactory>()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ActorBase)));
        }
    }
}
