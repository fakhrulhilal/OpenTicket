using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.Configuration;
using FluentValidation;
using FluentValidation.Mvc;
using MediatR;
using MediatR.Pipeline;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;
using OpenTicket.Domain.MailClient;
using OpenTicket.Domain.Mapper;
using OpenTicket.Web.Models;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace OpenTicket.Web
{
    internal static class DependencyConfig
    {
        internal static void Bootstrap(Container container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            Register(container);
            // This is an extension method from the integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void Register(Container container)
        {
            var mainConnectionSetting = ConfigurationManager.ConnectionStrings["Main"];
            if (mainConnectionSetting == null)
                throw new ConfigurationErrorsException("No connection string named 'Main' defined in Web.config");
            container.RegisterDisposableTransient(() => new OpenTicketDbContext(mainConnectionSetting.ConnectionString), "Used in both web & hangfire");
            container.Register<ImportEmailJob>(Lifestyle.Transient);
            container.Collection.Register<IMailClientFactory>(new[] { typeof(MailAdapter.MailMessageAdapter).Assembly }, Lifestyle.Singleton);
            RegisterAutoMapper(container);
            RegisterMediator(container);
            RegisterFluentValidation(container);
        }

        private static void RegisterFluentValidation(Container container)
        {
            container.Register(typeof(IValidator<>), typeof(PageableQuery).Assembly, typeof(PagedRequest).Assembly);
            container.RegisterSingleton<IValidatorFactory>(() => new FluentValidationSimpleInjectorAdapter(container));
            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new FluentValidationSimpleInjectorAdapter(container);
                provider.AddImplicitRequiredValidator = false;
            });
        }

        private static void RegisterAutoMapper(Container container)
        {
            var configExpression = new MapperConfigurationExpression();
            configExpression.ConstructServicesUsing(container.GetInstance);
            configExpression.AddMaps(typeof(DomainToEntityMapping).Assembly);
            var config = new MapperConfiguration(configExpression);
            config.AssertConfigurationIsValid();
            var mapper = new Mapper(config, container.GetInstance);
            container.RegisterSingleton<IMapper>(() => mapper);
        }

        private static void RegisterMediator(Container container)
        {
            var assemblies = new[]
            {
                typeof(IMediator).Assembly,
                typeof(PageableQuery).Assembly
            };
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            void RegisterHandlers(Type collectionType)
            {
                var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
                {
                    IncludeGenericTypeDefinitions = true,
                    IncludeComposites = false
                });

                container.Collection.Register(collectionType, handlerTypes);
            }

            RegisterHandlers(typeof(INotificationHandler<>));
            RegisterHandlers(typeof(IRequestExceptionAction<,>));
            RegisterHandlers(typeof(IRequestExceptionHandler<,,>));

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(RequestExceptionProcessorBehavior<,>),
                typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>)
                //typeof(GenericPipelineBehavior<,>)
            });
            container.Collection.Register(typeof(IRequestPreProcessor<>), assemblies);
            container.Collection.Register(typeof(IRequestPostProcessor<,>), assemblies);
            container.RegisterSingleton(() => new ServiceFactory(container.GetInstance));
        }
    }
}