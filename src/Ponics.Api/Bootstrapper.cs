﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Ponics.Analysis.Levels;
using Ponics.AquaponicSystems;
using Ponics.Commands;
using Ponics.Data.Decorator;
using Ponics.Data.Mongo.CommandHandlers;
using Ponics.Data.Mongo.QueryHandlers;
using Ponics.Data.Seed;
using Ponics.Kernel.Data;
using Ponics.Organisms;
using Ponics.Queries;

namespace Ponics.Api
{
    public static class Bootstrapper
    {
        private static Container _container;
        private static readonly Assembly[] ContractAssemblies = {typeof(Query<>).Assembly};

        public static Container Bootstrap()
        {
            _container = new Container();

            RegisterMongo();
            RegisterLevelsMagicStrings();
            RegisterQueryHandlers();
            RegisterDataQueryHandlers();
            RegisterSeedData();
            RegisterCommandHandlers();
            RegisterDataCommandHandlers();
            RegisterAddTolerance();
            RegisterDecorators();

            _container.Verify();

            return _container;
        }

        private static void RegisterDecorators()
        {
            _container.RegisterDecorator(
                typeof(IDataQueryHandler<GetAllOrganisms, IList<Organism>>),
                typeof(SeedOrganismsDecorator));
        }

        private static void RegisterCommandHandlers()
        {
            _container.Register<
                ICommandHandler<AddOrganism>,
                AddOrganismCommandHandler>();
        }

        private static void RegisterDataCommandHandlers()
        {
            _container.Register<
                IDataCommandHandler<AddOrganism>,
                AddOrganismDataCommandHandler>();

            _container.Register<
                IDataCommandHandler<UpdateOrganism>,
                UpdateOrganismDataCommandHandler>();
        }

        private static void RegisterSeedData()
        {
            _container.Register(typeof(SeedData<>), new[] {typeof(SeedData<>).Assembly});
        }

        private static void RegisterAddTolerance()
        {
            var commandHandlerType = typeof(ICommandHandler<>);
            var addToleranceCommandHandlerType = typeof(AddToleranceCommandHandler<>);

            foreach (var addToleranceType in GetAddToleranceTypes())
            {
                var toleranceType = addToleranceType.BaseType.GenericTypeArguments[0];

                var gernicCommandHandlerType = commandHandlerType.MakeGenericType(addToleranceType);
                var genericAddToleranceCommandHandlerType = addToleranceCommandHandlerType.MakeGenericType(toleranceType);
                _container.Register(gernicCommandHandlerType, genericAddToleranceCommandHandlerType);
            }
        }

        private static void RegisterDataQueryHandlers()
        {
            _container.Register<
                IDataQueryHandler<GetAllOrganisms, IList<Organism>>, 
                GetAllOrganismsDataQueryHandler>();

            _container.Register<
                IDataQueryHandler<GetAllSystems, IList<AquaponicSystem>>, 
                GetAllSystemsHandlerDataQueryHandler>();
        }

        private static void RegisterQueryHandlers()
        {
            _container.Register(typeof(IQueryHandler<,>), new[] {typeof(IQueryHandler<,>).Assembly});
        }

        private static void RegisterMongo()
        {
            var mongodbUri = Environment.GetEnvironmentVariable("MONGODB_URI");

            var mongoUrl = new MongoUrl(mongodbUri);
            var dbname = mongoUrl.DatabaseName;
            var db = new MongoClient(mongoUrl).GetDatabase(dbname);
            _container.Register(() => db, Lifestyle.Singleton);

            BsonClassMap.RegisterClassMap<AquaponicSystem>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Organism>(cm => 
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
            });

            foreach (var toleranceType in GetToleranceTypes())
            {
                var bsonClassMap = new BsonClassMap(toleranceType);
                BsonClassMap.RegisterClassMap(bsonClassMap);
            }
        }

        private static void RegisterLevelsMagicStrings()
        {
            _container.Register<IToleranceMagicStrings, ToleranceMagicStrings>(); 
            var levelMagicStringsAssembly = typeof(ILevelsMagicStrings).Assembly;

            var registrations =
                from type in levelMagicStringsAssembly.GetExportedTypes()
                where typeof(ILevelsMagicStrings).IsAssignableFrom(type)
                where !type.IsAbstract
                select type;

            foreach (var reg in registrations)
            {
                _container.Register(reg.GetInterfaces().Single(i => i != typeof(ILevelsMagicStrings)), reg);
            }
        }

        public static object GetInstance(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        public static IEnumerable<Type> GetCommandTypes() =>
            from assembly in ContractAssemblies
            from type in assembly.GetExportedTypes()
            where typeof(Command).IsAssignableFrom(type) && !type.IsAbstract
            select type;

        public static IEnumerable<QueryInfo> GetQueryTypes() =>
            from assembly in ContractAssemblies
            from type in assembly.GetExportedTypes()
            where QueryInfo.IsQuery(type) && !type.IsAbstract
            select new QueryInfo(type);
        
        public static object GetCommandHandler(Type commandType) =>
            _container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(commandType));
        
        public static object GetQueryHandler(Type queryType) =>
            _container.GetInstance(CreateQueryHandlerType(queryType));

        private static Type CreateQueryHandlerType(Type queryType) =>
            typeof(IQueryHandler<,>).MakeGenericType(queryType, new QueryInfo(queryType).ResultType);

        public static IEnumerable<Type> GetToleranceTypes() =>
            from type in typeof(Tolerance).Assembly.GetExportedTypes()
            where typeof(Tolerance).IsAssignableFrom(type)
            where !type.IsAbstract
            select type;

        public static IEnumerable<Type> GetAddToleranceTypes() =>
            from type in typeof(AddTolerance<>).Assembly.GetExportedTypes()
            let baseType = type.BaseType
            where baseType != null
            where baseType.IsGenericType
            where baseType.GetGenericTypeDefinition() == typeof(AddTolerance<>)
            where !type.IsAbstract
            select type;
    }

    [DebuggerDisplay("{QueryType.Name,nq}")]
    public sealed class QueryInfo
    {
        public readonly Type QueryType;
        public readonly Type ResultType;

        public QueryInfo(Type queryType)
        {
            QueryType = queryType;
            ResultType = DetermineResultTypes(queryType).Single();
        }

        public static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type) =>
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
            select interfaceType.GetGenericArguments()[0];
    }
}