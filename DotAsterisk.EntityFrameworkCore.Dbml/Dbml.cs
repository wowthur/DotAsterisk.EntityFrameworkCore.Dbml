using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    /// <summary>
    /// Casing style to be used for generated classes and/or columns
    /// </summary>
    public enum DbmlCasing
    {
        Default,
        CamelCasing
    }

    /// <summary>
    /// Representation of a DBML file
    /// </summary>
    public class Dbml
    {
        /// <summary>
        /// List of tables
        /// </summary>
        public IList<DbmlTable> Tables { get; set; } = new List<DbmlTable>();

        /// <summary>
        /// List of references
        /// </summary>
        public IList<DbmlReference> References { get; set; } = new List<DbmlReference>();

        /// <summary>
        /// Read dbml file and return Dbml object
        /// </summary>
        /// <param name="inputfile"></param>
        /// <returns>Dbml object</returns>
        public static Dbml FromFile(string inputfile)
        {
            var retval = new Dbml();
            var intable = false;
            var block = new StringBuilder();

            using var sr = new StreamReader(inputfile);

            var line = sr.ReadLine();

            while (line != null)
            {
                if (line.StartsWith("//"))
                {
                    // Skip comments
                }
                else if (line.StartsWith("Table "))
                {
                    block.Clear();
                    block.AppendLine(line);
                    intable = true;
                }
                else if (line.StartsWith("Ref: "))
                {
                    retval.References.Add(DbmlReference.FromLine(line));
                }
                else if (intable && line.StartsWith("}"))
                {
                    intable = false;
                    block.AppendLine(line);
                    retval.Tables.Add(new DbmlTable(block.ToString()));
                    block.Clear();
                }
                else
                {
                    block.AppendLine(line);
                }

                line = sr.ReadLine();
            };

            return retval;
        }
    }
}
