using System.Text.RegularExpressions;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    /// <summary>
    /// Representation of a DBML reference
    /// </summary>
    public class DbmlReference
    {
        /// <summary>
        /// Source table
        /// </summary>
        public string SourceTable { get; set; }

        /// <summary>
        /// Source column
        /// </summary>
        public string SourceColumn { get; set; }

        /// <summary>
        /// Destination table
        /// </summary>
        public string DestinationTable { get; set; }

        /// <summary>
        /// Destination column
        /// </summary>
        public string DestinationColumn { get; set; }

        /// <summary>
        /// Source code of reference line from DBML file
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Parse DBML source line and return DbmlReference object
        /// </summary>
        /// <param name="line">Line to be parsed into DbmlReference</param>
        /// <returns>DbmlReference object</returns>
        public static DbmlReference FromLine(string line)
        {
            var match = Regex.Match(line, @"^Ref: (.+)\.(.+) > (.+)\.(.+)$");

            if (!match.Success)
            {
                return null;
            }

            return new DbmlReference
            {
                SourceTable = match.Groups[1].Value,
                SourceColumn = match.Groups[2].Value,
                DestinationTable = match.Groups[3].Value,
                DestinationColumn = match.Groups[4].Value,
                Source = line
            };
        }
    }
}