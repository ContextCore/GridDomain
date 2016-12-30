using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shop.Infrastructure;
using Shop.Tests.Unit;

namespace Shop.Tests.Acceptance
{
    [TestFixture]
    public class SqlSequence_tests: Sequence_provider_tests
    {
        protected override Func<ISequenceProvider> SequenceProviderFactory { get; }

        private const string ConnectionString = "Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True";

        public SqlSequence_tests()
        {
            SequenceProviderFactory = () => {
              var  sequenceProvider = new SqlSequenceProvider(ConnectionString);
              sequenceProvider.Connect();
              return sequenceProvider;
            };
        }

        [TearDown]
        public void ClearSequences()
        {
            DeleteCreatedSequences(CreatedSequences.ToArray());
            CreatedSequences.Clear();
        }

        [OneTimeSetUp]
        public void ClearGlobalSequences()
        {
           DeleteCreatedSequences("global");
        }


        private void DeleteCreatedSequences(params string[] sequences)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (var sequence in sequences)
                {
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

        [OneTimeSetUp]
        public void Init()
        {
          
        }

    }
}
