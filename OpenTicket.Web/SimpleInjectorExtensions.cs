using System;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace OpenTicket.Web
{
    internal static class SimpleInjectorExtensions
    {
        internal static void RegisterDisposableTransient<TService>(this Container container, Func<TService> instanceCreator,
            string suppressMessage)
            where TService : class, IDisposable
        {
            var registration = Lifestyle.Transient.CreateRegistration(instanceCreator, container);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, suppressMessage);
            container.AddRegistration(typeof(TService), registration);
        }
    }
}