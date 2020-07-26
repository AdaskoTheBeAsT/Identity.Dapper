using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Identity.Dapper
{
    public static class StringReplaceExtensions
    {
#pragma warning disable MA0009 // Add timeout parameter
        private static readonly Regex _regex = new Regex(@"[^\w\d]");
#pragma warning restore MA0009 // Add timeout parameter

        public static string ReplaceInsertQueryParameters(this string query, string schemaName, string tableName, string columns, string values)
        {
            if (string.IsNullOrEmpty(query))
            {
                return query;
            }

            return query.Replace("%SCHEMA%", schemaName)
                        .Replace("%TABLENAME%", tableName)
                        .Replace("%COLUMNS%", $"({columns})")
                        .Replace("%VALUES%", values);
        }

        public static string ReplaceDeleteQueryParameters(this string query, string schemaName, string tableName, string idParameter)
        {
            if (string.IsNullOrEmpty(query))
            {
                return query;
            }

            return query.Replace("%SCHEMA%", schemaName)
                        .Replace("%TABLENAME%", tableName)
                        .Replace("%ID%", idParameter);
        }

        public static string ReplaceUpdateQueryParameters(this string query, string schemaName, string tableName, string setValues, string idParameter)
        {
            if (string.IsNullOrEmpty(query))
            {
                return query;
            }

            return query.Replace("%SCHEMA%", schemaName)
                        .Replace("%TABLENAME%", tableName)
                        .Replace("%SETVALUES%", setValues)
                        .Replace("%ID%", idParameter);
        }

        public static string ReplaceQueryParameters(
            this string query,
            string schemaName,
            string tableName,
            string parameterNotation,
            string[] parameterPlaceholders,
            string[] sqlParameterValues)
        {
            if (parameterPlaceholders is null)
            {
                throw new ArgumentNullException(nameof(parameterPlaceholders));
            }

            if (sqlParameterValues is null)
            {
                throw new ArgumentNullException(nameof(sqlParameterValues));
            }

            var queryBuilder = new StringBuilder(query);
            for (int i = 0; i < parameterPlaceholders.Length; i++)
            {
                queryBuilder.Replace($"{parameterPlaceholders[i]}", $"{parameterNotation}{sqlParameterValues[i]}");
            }

            queryBuilder.Replace("%SCHEMA%", schemaName)
                .Replace("%TABLENAME%", tableName);

            return queryBuilder.ToString();
        }

        public static string ReplaceQueryParameters(
            this string query,
            string schemaName,
            string tableName,
            string parameterNotation,
            string[] parameterPlaceholders,
            string[] sqlParameterValues,
            string[] othersPlaceholders,
            string[] othersPlaceholdersValues)
        {
            if (othersPlaceholders is null)
            {
                throw new ArgumentNullException(nameof(othersPlaceholders));
            }

            if (othersPlaceholdersValues is null)
            {
                throw new ArgumentNullException(nameof(othersPlaceholdersValues));
            }

            var queryBuilder = new StringBuilder(
                query.ReplaceQueryParameters(
                    schemaName,
                    tableName,
                    parameterNotation,
                    parameterPlaceholders,
                    sqlParameterValues));
            for (int i = 0; i < othersPlaceholders.Length; i++)
            {
                queryBuilder.Replace(othersPlaceholders[i], othersPlaceholdersValues[i]);
            }

            return queryBuilder.ToString();
        }

        public static string RemoveSpecialCharacters(this string value) => _regex.Replace(value, string.Empty);
    }
}
