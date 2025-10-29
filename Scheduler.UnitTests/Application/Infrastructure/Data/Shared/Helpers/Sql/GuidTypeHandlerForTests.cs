using Dapper;
using System;
using System.Data;

namespace Scheduler.UnitTests.Application.Infrastructure.Data.Shared.Helpers.Sql
{
    internal sealed class GuidTypeHandlerForTests : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            // Armazenamos como texto CANÔNICO; poderia usar value.ToByteArray() se preferir BLOB.
            parameter.Value = value.ToString();
            parameter.DbType = DbType.String;
        }

        public override Guid Parse(object value)
        {
            if (value is Guid g) return g;
            if (value is byte[] b) return new Guid(b);
            if (value is string s) return Guid.Parse(s);
            return Guid.Parse(Convert.ToString(value));
        }
    }
}