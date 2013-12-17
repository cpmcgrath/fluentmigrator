using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FluentMigrator.Runner.Generators.Sybase
{
    public class SybaseGenerator : GenericGenerator
    {
        public SybaseGenerator() : base(new SybaseColumn(), new SybaseQuoter(), new EmptyDescriptionGenerator())
        {
        }



        public override string Generate(CreateSchemaExpression expression)
        {
            return string.Format("CREATE SCHEMA AUTHORIZATION {0}", Quoter.QuoteSchemaName(expression.SchemaName));
        }

        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return String.Format("ALTER TABLE {0}.{1} MODIFY {2} {3}",
                Quoter.QuoteSchemaName(expression.SchemaName),
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName),
                ((SybaseColumn)Column).FormattedDefault(expression.ColumnName, expression.DefaultValue));
                
        }

        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return String.Format("ALTER TABLE {0}.{1} MODIFY {2} DEFAULT NULL",
                Quoter.QuoteSchemaName(expression.SchemaName),
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName));
        }

        public override string AddColumn { get { return "ALTER TABLE {0} ADD {1}"; } }
        public override string DropColumn { get { return "ALTER TABLE {0} DROP {1}"; } }
        public override string AlterColumn { get { return "ALTER TABLE {0} MODIFY {1}"; } }
        public override string RenameColumn { get { return "ALTER TABLE {0} RENAME {1} TO {2}"; } }

        public override string RenameTable { get { return "ALTER TABLE {0} RENAME {1}"; } }

        public override string CreateForeignKeyConstraint { get { return "ALTER TABLE {0} ADD FOREIGN KEY {1} ({2}) REFERENCES {3} ({4}){5}{6}"; } }


        public override string Generate(CreateSequenceExpression expression)
        {
            return compatabilityMode.HandleCompatabilty("Sequences are not supported in Sybase");
        }

        public override string Generate(DeleteSequenceExpression expression)
        {
            return compatabilityMode.HandleCompatabilty("Sequences are not supported in Sybase");
        }


        protected new string FormatCascade(string onWhat, Rule rule)
        {
            string action = "RESTRICT";
            switch (rule)
            {
                case Rule.None:
                    return "";
                case Rule.Cascade:
                    action = "CASCADE";
                    break;
                case Rule.SetNull:
                    action = "SET NULL";
                    break;
                case Rule.SetDefault:
                    action = "SET DEFAULT";
                    break;
            }

            return string.Format(" ON {0} {1}", onWhat, action);
        }


        public override string Generate(CreateForeignKeyExpression expression)
        {
            if (expression.ForeignKey.PrimaryColumns.Count != expression.ForeignKey.ForeignColumns.Count)
            {
                throw new ArgumentException("Number of primary columns and secondary columns must be equal");
            }

            string keyName = string.IsNullOrEmpty(expression.ForeignKey.Name)
                ? GenerateForeignKeyName(expression)
                : expression.ForeignKey.Name;

            List<string> primaryColumns = new List<string>();
            List<string> foreignColumns = new List<string>();
            foreach (var column in expression.ForeignKey.PrimaryColumns)
            {
                primaryColumns.Add(Quoter.QuoteColumnName(column));
            }

            foreach (var column in expression.ForeignKey.ForeignColumns)
            {
                foreignColumns.Add(Quoter.QuoteColumnName(column));
            }
            return string.Format(
                CreateForeignKeyConstraint,
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable),
                Quoter.QuoteConstraintName(keyName),
                String.Join(", ", foreignColumns.ToArray()),
                Quoter.QuoteTableName(expression.ForeignKey.PrimaryTable),
                String.Join(", ", primaryColumns.ToArray()),
                FormatCascade("DELETE", expression.ForeignKey.OnDelete),
                FormatCascade("UPDATE", expression.ForeignKey.OnUpdate) 
                );
        }

        
    }
}
