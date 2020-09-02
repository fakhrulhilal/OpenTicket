using SimpleInjector;
using System;

namespace OpenTicket.Web
{
    public class HangfireSimpleInjectorActivator : Hangfire.JobActivator
    {
        private readonly Container _container;

        public HangfireSimpleInjectorActivator(Container container) =>
            _container = container ?? throw new ArgumentNullException(nameof(container));

        public override object ActivateJob(Type jobType) => _container.GetInstance(jobType);
    }
}