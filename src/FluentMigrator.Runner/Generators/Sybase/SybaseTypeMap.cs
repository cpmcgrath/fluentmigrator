using FluentMigrator.Runner.Generators.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FluentMigrator.Runner.Generators.Sybase
{
    internal class SybaseTypeMap : TypeMapBase
    {
        public const int AnsiStringCapacity = 32767;
        public const int AnsiTextCapacity = 2147483647;
        public const int DecimalCapacity = 30;

        protected override void SetupTypeMaps()
        {
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR(255)");
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "VARCHAR(255)");
            SetTypeMap(DbType.AnsiString, "VARCHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "LONG VARCHAR", AnsiTextCapacity);
            SetTypeMap(DbType.Binary, "VARBINARY(8000)");
            SetTypeMap(DbType.Binary, "VARBINARY($size)", AnsiStringCapacity);
            SetTypeMap(DbType.Binary, "LONG BINARY", int.MaxValue);            
            SetTypeMap(DbType.Boolean, "BIT");
            SetTypeMap(DbType.Byte, "UNSIGNED TINYINT");            
            SetTypeMap(DbType.Currency, "MONEY");
            SetTypeMap(DbType.Date, "DATE");
            SetTypeMap(DbType.DateTime, "DATETIME");
            SetTypeMap(DbType.Decimal, "NUMERIC(19,5)");
            SetTypeMap(DbType.Decimal, "NUMERIC($size,$precision)", DecimalCapacity);
            SetTypeMap(DbType.Double, "DOUBLE");
            SetTypeMap(DbType.Guid, "UNIQUEIDENTIFIERSTR");
            SetTypeMap(DbType.Int16, "SMALLINT");
            SetTypeMap(DbType.Int32, "INT");
            SetTypeMap(DbType.Int64, "BIGINT");
            SetTypeMap(DbType.Single, "REAL");
            SetTypeMap(DbType.StringFixedLength, "CHAR(255)");
            SetTypeMap(DbType.StringFixedLength, "CHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.String, "VARCHAR(255)");
            SetTypeMap(DbType.String, "VARCHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.String, "LONG VARCHAR", int.MaxValue);
            SetTypeMap(DbType.String, "LONG VARCHAR", AnsiStringCapacity);
            SetTypeMap(DbType.Time, "TIME");
            SetTypeMap(DbType.Xml, "LONG VARCHAR");
        }
    }
}
