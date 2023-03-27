using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace FunctionsCore.Utilities
{
    public static class LinqExtensions
    {
        public static List<T> ToListWithNoLock<T>(this IQueryable<T> query)
        {
            throw new NotImplementedException("Nincs tesztelve!"); 
            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }))
            {
                List<T> toReturn = query.ToList();
                scope.Complete();
                return toReturn;
            }
        }

        public static T SingleOrDefaultWithNoLock<T>(this IQueryable<T> query)
        {
            throw new NotImplementedException("Nincs tesztelve!");
            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }))
            {
                var toReturn = query.SingleOrDefault();
                scope.Complete();
                return toReturn;
            }
        }

        public static T FirstOrDefaultWithNoLock<T>(this IQueryable<T> query)
        {
            throw new NotImplementedException("Nincs tesztelve!");
            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }))
            {
                var toReturn = query.FirstOrDefault();
                scope.Complete();
                return toReturn;
            }
        }
    }
}
