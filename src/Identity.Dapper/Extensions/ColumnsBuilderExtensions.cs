using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Identity.Dapper.Models;

namespace Identity.Dapper
{
    public static class ColumnsBuilderExtensions
    {
        public static string GetCommaSeparatedColumns(this IEnumerable<string> properties)
        {
            var columnsBuilder = new StringBuilder();

            columnsBuilder.Append(string.Join(", ", properties));

            return columnsBuilder.ToString();
        }

        public static IEnumerable<string> GetColumns<TEntity>(
            this TEntity entity,
            SqlConfiguration sqlConfiguration,
            bool ignoreIdProperty = false,
            IEnumerable<string>? ignoreProperties = null,
            bool forInsert = true)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            ignoreProperties = ignoreProperties ?? Enumerable.Empty<string>();

            IEnumerable<string> roleProperties;
            var idProperty = entity.GetType().GetProperty("Id");

            if (idProperty != null && !ignoreIdProperty)
            {
                var defaultIdTypeValue = idProperty.PropertyType == typeof(string)
                    ? string.Empty
                    : Activator.CreateInstance(idProperty.PropertyType);
                var idPropertyValue = idProperty.GetValue(entity, null);

                if (!idPropertyValue.Equals(defaultIdTypeValue))
                {
                    roleProperties = entity.GetType()
                        .GetPublicPropertiesNames(
                            x => !ignoreProperties.Any(y => x.Name.Equals(y, StringComparison.OrdinalIgnoreCase)));
                }
                else if (forInsert)
                {
                    roleProperties = entity.GetType()
                        .GetPublicPropertiesNames(
                            y => !y.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                                 && !ignoreProperties.Any(
                                     x => x.Equals(y.Name, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    roleProperties = entity.GetType()
                        .GetPublicPropertiesNames(
                            x => !ignoreProperties.Any(y => x.Name.Equals(y, StringComparison.OrdinalIgnoreCase)));
                }
            }
            else
            {
                roleProperties = entity.GetType()
                    .GetPublicPropertiesNames(
                        y => !y.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                             && !ignoreProperties.Any(x => x.Equals(y.Name, StringComparison.OrdinalIgnoreCase)));
            }

            roleProperties = roleProperties.Select(
                y => string.Concat(
                    sqlConfiguration.TableColumnStartNotation,
                    y,
                    sqlConfiguration.TableColumnEndNotation));

            return roleProperties;
        }
    }
}
