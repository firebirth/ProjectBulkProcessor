using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace ProjectBulkProcessor.Extensions
{
    public static class AutofacExtensions
    {
        public static void AssertConfigurationIsValid(this IContainer container)
        {
            var problems = FindConfigurationProblems(container);
            if (problems.Any())
            {
                throw new AggregateException(problems);
            }
        }

        public static IList<Exception> FindConfigurationProblems(this IContainer container)
        {
            var services = container.ComponentRegistry
                                    .Registrations
                                    .SelectMany(x => x.Services)
                                    .OfType<TypedService>()
                                    .Where(x => !x.ServiceType.Name.StartsWith("Autofac"))
                                    .ToList();
            var exceptions = new List<Exception>();
            foreach (var typedService in services)
            {
                try
                {
                    container.Resolve(typedService.ServiceType);
                }
                catch (DependencyResolutionException ex)
                {
                    exceptions.Add(ex);
                }
            }

            return exceptions;
        }
    }
}