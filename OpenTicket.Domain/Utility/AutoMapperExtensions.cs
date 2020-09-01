using System;
using AutoMapper;

namespace OpenTicket.Domain.Utility
{
    internal static class AutoMapperExtensions
    {
        private static void IgnoreUnmappedProperties(TypeMap map, IMappingExpression expr)
        {
            foreach (string propName in map.GetUnmappedPropertyNames())
            {
                if (map.SourceType.GetProperty(propName) != null)
                    expr.ForSourceMember(propName, opt => opt.DoNotValidate());
                if (map.DestinationType.GetProperty(propName) != null)
                    expr.ForMember(propName, opt => opt.Ignore());
            }
        }

        /// <summary>
        /// Ignore unmapped property for all mapping defined in the profile
        /// </summary>
        /// <param name="profile"></param>
        internal static void IgnoreUnmapped(this IProfileExpression profile) =>
            profile.ForAllMaps(IgnoreUnmappedProperties);

        /// <summary>
        /// Ignore property mapping with certain criteria
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="filter"></param>
        internal static void IgnoreUnmapped(this IProfileExpression profile, Func<TypeMap, bool> filter) =>
            profile.ForAllMaps((map, expr) =>
            {
                if (filter(map))
                {
                    IgnoreUnmappedProperties(map, expr);
                }
            });

        /// <summary>
        /// Ignore all properties from source member that's not available on destination
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        internal static void IgnoreUnmapped(this IProfileExpression profile, Type src, Type dest) =>
            profile.IgnoreUnmapped(map => map.SourceType == src && map.DestinationType == dest);

        /// <summary>
        /// Ignore all properties from source member that's not available on destination
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="profile"></param>
        internal static void IgnoreUnmapped<TSrc, TDst>(this IProfileExpression profile) =>
            profile.IgnoreUnmapped(typeof(TSrc), typeof(TDst));
    }
}
