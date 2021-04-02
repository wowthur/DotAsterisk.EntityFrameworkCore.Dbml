using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DotAsterisk.EntityFrameworkCore.Dbml.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-o", "output" },
                { "-i", "input" },
                { "-c", "case" }
            };
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();
            var casing = config["case"] == "camel" ? DbmlCasing.CamelCasing : DbmlCasing.Default;

            Dmbl2Ef.Generate(config["input"], config["output"], casing);
        }
    }
}
