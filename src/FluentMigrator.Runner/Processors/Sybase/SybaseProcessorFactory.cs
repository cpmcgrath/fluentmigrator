

namespace FluentMigrator.Runner.Processors.Sybase {
    using System.Data.Odbc;
    using Generators.Sybase;

    public class SybaseProcessorFactory : MigrationProcessorFactory {
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options) {
            var factory = new SybaseDbFactory();
            var connection = factory.CreateConnection(connectionString);            
            return new SybaseProcessor(connection, new SybaseGenerator(), announcer, options, factory);
        }
    }
}