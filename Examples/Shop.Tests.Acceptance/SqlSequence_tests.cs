using System;
using System.Data.SqlClient;
using Shop.Infrastructure;
using Shop.Tests.Unit;
using Shop.Tests.Unit.XUnit;

namespace Shop.Tests.Acceptance
{
    public class SqlSequence_tests : Sequence_provider_tests,
                                     IDisposable
    {
        public SqlSequence_tests()
        {
            var prov = new SqlSequenceProvider(ConnectionString);
            prov.Connect();
            Provider = prov;
        }
        protected override ISequenceProvider Provider { get; }

        public void Dispose()
        {
            DeleteCreatedSequences("global");
            DeleteCreatedSequences(CreatedSequences.ToArray());
            CreatedSequences.Clear();
        }


        private const string ConnectionString =
            "Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True";

        private void DeleteCreatedSequences(params string[] sequences)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (var sequence in sequences)
                    try
                    {
                        var cmd = new SqlCommand("DROP SEQUENCE " + sequence);
                        cmd.Connection = connection;
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        Console.WriteLine("error was occured while deleting sequence " + sequence);
                    }
            }
        }
    }
}