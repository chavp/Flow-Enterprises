using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Flowenter.Parties.Mappings.Extensions
{
    internal static class ModelBuilderExtensions
    {
        public static ValueConverter<string, string> UpperConverter =>
            new ValueConverter<string, string>(
                v => v.ToUpperInvariant(),
                v => v.ToUpperInvariant());

        public static ValueConverter<string, string> LowerConverter =>
            new ValueConverter<string, string>(
                v => v.ToLowerInvariant(),
                v => v.ToLowerInvariant());

        internal static void ApplyUpperConverter(
            this ModelBuilder modelBuilder,
            ImmutableList<string> upperFields)
        {
            foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                IEnumerable<IMutableProperty> codeProperties = mutableEntityType.GetProperties()
                    .Where(p => upperFields.Any(n => p.Name.Equals(n, StringComparison.InvariantCultureIgnoreCase)));

                foreach (IMutableProperty mutableProperty in codeProperties)
                {
                    mutableProperty.SetValueConverter(UpperConverter);
                }
            }
        }
    }
}
