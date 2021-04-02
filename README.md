# DotAsterisk.EntityFrameworkCore.Dbml

Library and CLI application to generate EF classes from [DBML](https://dbml.org/) file.

## CLI Usage
```
Usage: DotAsterisk.EntityFrameworkCore.Dbml.Cli -i INPUTFILE -o OUTPUTDIR [-c camel]

Convert .dbml file to EF classes

options:
  -i INPUTFILE  Path to DBML input file
  -o OUTPUTDIR  Path to output directory
  -c camel      Casing to use for tables and columns
                'camel' is the only option supported at this moment.
```

### Example:
```
DotAsterisk.EntityFrameworkCore.Dbml.Cli -i database.dbml -o MyProject.Data -c camel
```