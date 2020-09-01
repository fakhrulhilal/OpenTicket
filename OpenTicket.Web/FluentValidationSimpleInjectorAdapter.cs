using System;
using FluentValidation;
using SimpleInjector;

namespace OpenTicket.Web
{
    public class FluentValidationSimpleInjectorAdapter : IValidatorFactory
    {
        private readonly Container _container;

        /// <summary>The constructor of the factory.</summary>
        /// <param name="container">The Simple Injector Container</param>
        public FluentValidationSimpleInjectorAdapter(Container container) => _container = container;

        /// <summary>Gets the validator for the specified type.</summary>
        public IValidator<T> GetValidator<T>() => _container.GetInstance<IValidator<T>>();

        /// <summary>Gets the validator for the specified type.</summary>
        public IValidator GetValidator(Type type)
        {
            var validator = typeof(IValidator<>).MakeGenericType(type);
            return (IValidator)_container.GetInstance(validator);
        }
    }
}
