namespace FluentMigrator.Runner.Processors.Sybase {
    public class SybaseDbFactory : ReflectionBasedDbFactory {
        public SybaseDbFactory()
            : base("System.Data.Odbc", "System.Data.OdbcFactory") {
        }
    }
}