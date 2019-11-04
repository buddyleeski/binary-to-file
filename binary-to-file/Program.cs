using CommandLine;
using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace binary_to_file
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = Parser.Default.ParseArguments<SqlBinaryToFileOptions>(args)
                                   .MapResult(
                                        (SqlBinaryToFileOptions opts) => SqlBinaryToFileOptions.Process(opts),
                                        errs => 1
                                    );
        }

        [Verb("sql", HelpText ="Executes a sql query and saves the specified binary column to a file")]
        public class SqlBinaryToFileOptions
        {
            [Option('q', "query", Required = true,
                HelpText = "The mssql query or the file name of the sql query to execute")]
            public string Query { get; set; }
            [Option('i', "is_file", Required = false, Default = false, 
                HelpText = "True if the query parameter is a file name of the query")]
            public bool FromFile { get; set; }

            [Option('n', "file_template", Required = true,
                HelpText = "Template for the generated file names. Use {FieldName} to insert values from the query into the name")]
            public string FileNameTemplate { get; set; }

            [Option('b', "binary_field", Required = true,
                HelpText ="The name of the binary field to fill with the file")]
            public string BinaryFieldName { get; set; }

            [Option('c', "conn_string", Required = true,
                HelpText = "Connection string to the appropriate sql server.")]
            public string ConnectionString { get; set; }
            
            public static int Process(SqlBinaryToFileOptions opts)
            {
                string query = "";
                if (opts.FromFile)
                {
                    query = File.ReadAllText(opts.Query);
                }
                else
                    query = opts.Query;

                using (SqlConnection conn = new SqlConnection(opts.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string fileName = opts.FileNameTemplate;
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                if (fieldName.ToLower() != opts.BinaryFieldName.ToLower())
                                    fileName = fileName.Replace($"{{{fieldName}}}", reader.GetString(i));
                            }

                            var binary = reader.GetSqlBinary(reader.GetOrdinal(opts.BinaryFieldName));
                            byte[] binaryVal = new byte[0];
                            if (!binary.IsNull)
                            {
                                binaryVal = binary.Value;
                            }

                            File.WriteAllBytes(fileName, binaryVal);
                        }

                        
                    }
                }

                return 0;
            }
        }

    }
}
