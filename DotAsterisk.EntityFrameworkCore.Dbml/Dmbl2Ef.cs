using System.Collections.Generic;
using System.IO;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    public class Dmbl2Ef
    {
        /// <summary>
        /// Generate complete DbContext, DbSet project
        /// </summary>
        /// <param name="dbmlfile">Input dbml file location</param>
        /// <param name="outputdir">Output directory to write files to</param>
        /// <param name="casing">Casing to be used for table/class names</param>
        public static void Generate(string dbmlfile, string outputdir, DbmlCasing casing)
        {
            var i = Path.GetFullPath(dbmlfile);
            var o = Path.GetFullPath(outputdir);

            if (!File.Exists(i))
                throw new FileNotFoundException("Input file not found", i);
            if (!Directory.Exists(o))
                throw new FileNotFoundException("Output folder not found", o);

            var dbml = Dbml.FromFile(i);

            // Write tables
            foreach (var table in dbml.Tables)
            {
                using var sw = new StringWriter();
                table.WriteClass("WAAK.Data", sw, casing);

                using var tableStreamWriter = new StreamWriter(Path.Combine(o, table.Name + ".cs"));
                tableStreamWriter.Write(sw.ToString());
            }

            // Write context
            using var contextStreamWriter = new StreamWriter(Path.Combine(o, "BaseContext.cs"));
            WriteContext("WAAK.Data", "BaseContext", dbml.Tables, dbml.References, contextStreamWriter, casing);
        }

        /// <summary>
        /// Write dbcontext base class from tables and references to TextWriter
        /// </summary>
        /// <param name="ns">Namespace of generated class</param>
        /// <param name="classname">Class name of generated class</param>
        /// <param name="tables">List of tables</param>
        /// <param name="references">List of references</param>
        /// <param name="writer">TextWriter to write to</param>
        /// <param name="casing">Casing to be used for column names</param>
        public static void WriteContext(string ns, string classname, IList<DbmlTable> tables, IList<DbmlReference> references, TextWriter writer, DbmlCasing casing)
        {
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine("namespace " + ns);
            writer.WriteLine("{");
            writer.WriteLine($"    public abstract class {classname} : DbContext");
            writer.WriteLine("    {");

            foreach (var table in tables)
            {
                writer.WriteLine($"        public DbSet<{table.Name}> {table.Name}s {{ get; set; }}");
            }

            if (references.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
                writer.WriteLine("        {");

                foreach (var reference in references)
                {
                    var fromColumn = casing == DbmlCasing.CamelCasing
                        ? char.ToUpperInvariant(reference.SourceColumn[0]) + reference.SourceColumn[1..]
                        : reference.SourceColumn;

                    writer.WriteLine("        // " + reference.Source);
                    writer.WriteLine($"            modelBuilder.Entity<{reference.SourceTable}>()");
                    writer.WriteLine($"                .HasOne<{reference.DestinationTable}>()");
                    writer.WriteLine("                .WithMany()");
                    writer.WriteLine($"                .HasForeignKey(k => k.{fromColumn});");
                    writer.WriteLine();
                }
                writer.WriteLine("        }");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }
    }
}