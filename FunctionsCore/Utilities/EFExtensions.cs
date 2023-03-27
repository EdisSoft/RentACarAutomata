using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FunctionsCore.Commons.Functions
{
    public static class EFExtensions
    {
        public static DbTransaction GetDbTransaction(this IDbContextTransaction source)
        {
            return (source as IInfrastructure<DbTransaction>).Instance;
        }
    }
}
