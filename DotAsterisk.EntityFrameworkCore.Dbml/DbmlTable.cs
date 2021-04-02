using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    /// <summary>
    /// Representation of a DBML table
    /// </summary>
    public class DbmlTable
    {
        /// <summary>
        /// Source code of table fragment from DBML file
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of columns
        /// </summary>
        public IList<DbmlColumn> Columns { get; set; } = new List<DbmlColumn>();

        /// <summary>
        /// Instantiate a new DbmlTable object
        /// </summary>
        /// <param name="source">source code of table fragment from DBML file</param>
        public DbmlTable(string source)
        {
            Source = source.Trim();

            Parse();
        }

        /// <summary>
        /// Write generated C# class of DBML table
        /// </summary>
        /// <param name="ns">Namespace of generated class</param>
        /// <param name="writer">TextWriter to write to</param>
        /// <param name="casing">Casing to be used for table/class names</param>
        public void WriteClass(string ns, TextWriter writer, DbmlCasing casing)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.ComponentModel.DataAnnotations;");
            writer.WriteLine();

            foreach (var line in Source.Split("\n"))
            {
                writer.WriteLine("// " + line.Trim('\r', '\n'));
            }

            writer.WriteLine();
            writer.WriteLine("namespace " + ns);
            writer.WriteLine("{");
            writer.WriteLine("    public class " + Name);
            writer.WriteLine("    {");

            foreach (var column in Columns)
            {
                var columnName = casing == DbmlCasing.CamelCasing
                    ? char.ToUpperInvariant(column.Name[0]) + column.Name[1..]
                    : column.Name;

                foreach (var option in column.Options)
                {
                    switch (option)
                    {
                        case "pk":
                            writer.WriteLine($"        [Key]");
                            break;
                        case "not null":
                            writer.WriteLine($"        [Required]");
                            break;
                    }
                }

                writer.WriteLine($"        public {column.Type} {columnName} {{ get; set; }}");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        /// <summary>
        /// Parse the DBML table fragment sourcecode and set the Name and
        /// Columns properties
        /// </summary>
        public void Parse()
        {
            using var sr = new StringReader(Source);

            var line = sr.ReadLine();

            while (line != null)
            {
                if (line.StartsWith("Table "))
                {
                    Name = GetNameFromLine(line);
                }
                if (line.StartsWith(" "))
                {
                    Columns.Add(DbmlColumn.FromLine(line));
                }

                line = sr.ReadLine();
            }
        }

        /// <summary>
        /// Get table name from source line
        /// </summary>
        /// <param name="line">source line from DBML</param>
        /// <returns>Name of table</returns>
        private static string GetNameFromLine(string line)
        {
            var match = Regex.Match(line, "^Table (.+) {$");

            if (!match.Success)
            {
                return null;
            }

            return match.Groups[1].Value;
        }
    }
}