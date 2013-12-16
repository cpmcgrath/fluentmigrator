using System;
using System.Data;
using System.Data.Common;
using FluentMigrator.Builders.Execute;

namespace FluentMigrator.Runner.Processors.Sybase {

    public class SybaseProcessor : GenericProcessorBase {
        public override string DatabaseType {
            get { return "Sybase"; }
        }

        public SybaseProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options) {
        }

        public override bool SchemaExists(string schemaName) {
            return Exists("select count(*) from SYSUSERPERM where user_group = 'N' and user_name = '{0}'", Escape(schemaName));
        }

        public override bool TableExists(string schemaName, string tableName) {
            return Exists("select count(*) from systable t inner join sysuserperm u on u.user_id = t.creator where table_name = '{0}' and table_type = 'base' and u.user_name = '{1}'",
                Escape(tableName), Escape(schemaName));
        }

        private string Escape(string identifier) {
            return identifier.Replace("'", "''");
        }

        public override bool ColumnExists(string schemaName, string tableName, string columnName) {
            return Exists(@"select count(*) from systable t 
                            inner join sysuserperm u on u.user_id = t.creator 
                            inner join syscolumn c on c.table_id = t.table_id
                            where table_name = '{0}' and table_type = 'base' and u.user_name = '{1}' and c.column_name = '{2}'",
                Escape(tableName), Escape(schemaName), Escape(columnName));
        }

        public override bool ConstraintExists(string schemaName, string tableName, string constraintName) {
            return Exists(@"select * from sysconstraint c
                            inner join systable t on t.table_id = c.table_id
                            inner join sysuserperm u on u.user_id = t.creator
                            where t.table_name = '{0}' and constraint_name = '{1}' and u.user_name = '{2}'",
                Escape(tableName), Escape(constraintName), Escape(schemaName));
        }

        public override bool IndexExists(string schemaName, string tableName, string indexName) {
            return Exists(@"select * from sysindex i
                            inner join systable t on t.table_id = i.table_id
                            inner join sysuserperm u on u.user_id = t.creator
                            where t.table_name = '{0}' and index_name = '{1}' and u.user_name = '{2}'",
                Escape(tableName), Escape(indexName), Escape(schemaName));                                                                                                      
        }

        public override bool SequenceExists(string schemaName, string sequenceName) {
            return false;
        }

        public override void Execute(string template, params object[] args) {
            Process(String.Format(template, args));
        }

        public override bool Exists(string template, params object[] args) {
            EnsureConnectionIsOpen();

            using (var command = Factory.CreateCommand(String.Format(template, args), Connection))
            using (var reader = command.ExecuteReader()) {
                try {
                    if (!reader.Read()) return false;
                    if (int.Parse(reader[0].ToString()) <= 0) return false;
                    return true;
                } catch {
                    return false;
                }
            }
        }

        public override DataSet ReadTableData(string schemaName, string tableName) {
            return Read("select * from [{0}]", tableName);
        }

        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue) {
            return Exists(@"select * from syscolumn c
                            inner join systable t on t.table_id = c.table_id
                            inner join sysuserperm u on u.user_id = t.creator
                            where c.[default] is not null and t.table_name = '{0}'
                                and u.user_name = '{1}'
                                and c.Column_name = '{2}'
                                and c.[default] like '%{3}%'",
                Escape(tableName), Escape(schemaName), Escape(columnName), Escape(defaultValue.ToString()));
        }

        public override void Process(PerformDBOperationExpression expression) {
            Announcer.Say("Performing DB Operation");

            if (Options.PreviewOnly)
                return;

            EnsureConnectionIsOpen();

            if (expression.Operation != null)
                expression.Operation(Connection, null);
        }

        protected override void Process(string sql) {
            Announcer.Sql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                return;

            EnsureConnectionIsOpen();

            if (sql.IndexOf("GO", StringComparison.OrdinalIgnoreCase) >= 0) {
                ExecuteBatchNonQuery(sql);

            } else {
                ExecuteNonQuery(sql);
            }


        }

        private void ExecuteNonQuery(string sql) {
            using (var command = Factory.CreateCommand(sql, Connection)) {
                try {
                    command.ExecuteNonQuery();
                } catch (DbException ex) {
                    throw new Exception(ex.Message + "\r\nWhile Processing:\r\n\"" + command.CommandText + "\"", ex);
                }
            }
        }

        private void ExecuteBatchNonQuery(string sql) {
            sql += "\nGO";   // make sure last batch is executed.
            string sqlBatch = string.Empty;

            using (var command = Factory.CreateCommand(sql, Connection)) {
                try {
                    foreach (string line in sql.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)) {
                        if (line.ToUpperInvariant().Trim() == "GO") {
                            if (!string.IsNullOrEmpty(sqlBatch)) {
                                command.CommandText = sqlBatch;
                                command.ExecuteNonQuery();
                                sqlBatch = string.Empty;
                            }
                        } else {
                            sqlBatch += line + "\n";
                        }
                    }
                } catch (DbException ex) {
                    throw new Exception(ex.Message + "\r\nWhile Processing:\r\n\"" + command.CommandText + "\"", ex);
                }
            }
        }

        public override DataSet Read(string template, params object[] args) {
            EnsureConnectionIsOpen();

            var ds = new DataSet();
            using (var command = Factory.CreateCommand(String.Format(template, args), Connection)) {
                var adapter = Factory.CreateDataAdapter(command);
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}