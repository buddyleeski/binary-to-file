# binary-to-file
Command line utility. Runs a query against a database loads a specified binary field into files for each row.

```
PS C:\binary-to-file\bin\Debug\netcoreapp2.2> dotnet .\binary-to-file.dll
binary-to-file 1.0.0
Copyright (C) 2019 binary-to-file

ERROR(S):
  Required option 'q, query' is missing.
  Required option 'n, file_template' is missing.
  Required option 'b, binary_field' is missing.
  Required option 'c, conn_string' is missing.

  -q, --query            Required. The mssql query or the file name of the sql query to execute

  -i, --is_file          (Default: false) True if the query parameter is a file name of the query

  -n, --file_template    Required. Template for the generated file names. Use {FieldName} to insert values from the
                         query into the name

  -b, --binary_field     Required. The name of the binary field to fill with the file

  -c, --conn_string      Required. Connection string to the appropriate sql server.

  --help                 Display this help screen.

  --version              Display version information.
```
