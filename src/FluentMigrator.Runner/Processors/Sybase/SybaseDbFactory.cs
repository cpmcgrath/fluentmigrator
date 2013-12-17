using System.Data;
using System.Data.Odbc;


namespace FluentMigrator.Runner.Processors.Sybase {
    public class SybaseDbFactory : IDbFactory {

        public IDbConnection CreateConnection(string connectionString) {
            return new OdbcConnection(connectionString);
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction) {
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.Transaction = transaction;
            return cmd;
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand command) {
            OdbcCommand cmd = (OdbcCommand)command;
            return new OdbcDataAdapter(cmd);            
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection) {
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }
    }
}