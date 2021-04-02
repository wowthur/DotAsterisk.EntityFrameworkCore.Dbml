using System.Text.RegularExpressions;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    /// <summary>
    /// Representation of a DBML table column
    /// </summary>
    public class DbmlColumn
    {
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column data type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///  Column options
        /// </summary>
        public string[] Options { get; set; }

        /// <summary>
        /// Parse DBML column source line and return DbmlColumn object
        /// </summary>
        /// <param name="line">Line to be parsed into DbmlColumn</param>
        /// <returns>DbmlColumn object</returns>
        public static DbmlColumn FromLine(string line)
        {
            var match = Regex.Match(line, @"^\s+([^ ]+) ([^ ]+) ?(\[(.+),?\])?");

            if (!match.Success)
            {
                return null;
            }

            return new DbmlColumn
            {
                Name = match.Groups[1].Value,
                Type = match.Groups[2].Value,
                Options = match.Groups.Count > 3 ? match.Groups[4].Value.Split(',') : null
            };
        }
    }
}