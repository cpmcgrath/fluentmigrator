using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.Runner.Generators.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentMigrator.Runner.Generators.Sybase
{
    internal class SybaseColumn : ColumnBase
    {
        public SybaseColumn() : base(new SybaseTypeMap(), new GenericQuoter())
        {
        }


        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? "DEFAULT AUTOINCREMENT" : string.Empty;
        }

        protected override string FormatSystemMethods(SystemMethods systemMethod)
        {
            switch (systemMethod)
            {
                case SystemMethods.CurrentDateTime:
                    return "GETDATE()";
                case SystemMethods.CurrentUser:
                    return "CURRENT_USER";
                case SystemMethods.CurrentUTCDateTime:
                    return "CURRENT UTC TIMESTAMP";
                case SystemMethods.NewGuid:
                    return "NEWID()";
                case SystemMethods.NewSequentialId:
                    return "NEWID()";
            }
            return null;
        }

        public string FormattedDefault(string column, object defaultValue) {
            return FormatDefaultValue(new ColumnDefinition { Name = column, DefaultValue = defaultValue});
        }

    }
}
