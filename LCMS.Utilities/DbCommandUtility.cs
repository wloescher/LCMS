using System.Data;
using System.Data.Common;
using System.Globalization;

namespace LCMS.Utilities
{
    public static class DbCommandUtility
    {
        /// <summary>
        /// Get query from DbCommand.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="includeFormatting"></param>
        /// <returns>Query as string.</returns>
        public static string GetQuery(DbCommand cmd, bool includeFormatting = true)
        {
            if (cmd == null) return string.Empty;

            // Add command text
            var query = string.Format(CultureInfo.CurrentCulture, "EXEC {0}", cmd.CommandText);

            // Add line break
            if (includeFormatting && cmd.Parameters.Count > 0)
            {
                query += "\n";
            }

            // Loop through parameters
            foreach (DbParameter parameter in cmd.Parameters)
            {
                // Add tab
                if (includeFormatting)
                {
                    query += "\t";
                }

                // Add comma
                if (parameter != cmd.Parameters[0])
                {
                    query += ", ";
                }

                // Get parameter value
                var parameterValue = string.Empty;
                switch (parameter.DbType)
                {
                    case DbType.Binary:
                        parameterValue = parameter.Value == null ? "NULL" : (bool)parameter.Value ? "1" : "0";
                        break;

                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.Int64:
                    case DbType.Single:
                    case DbType.Double:
                    case DbType.Currency:
                        parameterValue = parameter.Value == null ? "NULL" : parameter.Value.ToString();
                        break;

                    default:
                        parameterValue = parameter.Value == null ? "NULL" : string.Format(CultureInfo.CurrentCulture, "'{0}'", parameter.Value);
                        break;
                }

                // Add parameter name and value
                query += string.Format(CultureInfo.CurrentCulture, "{0} = {1}", parameter.ParameterName, parameterValue);

                // Add line break
                if (includeFormatting && parameter != cmd.Parameters[cmd.Parameters.Count - 1])
                {
                    query += "\n";
                }
            }

            // Add semicolon
            query += ";";

            return query;
        }
    }
}
