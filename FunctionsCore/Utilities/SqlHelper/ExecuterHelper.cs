using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FunctionsCore.Utilities.SqlHelper
{
    internal class ExecuterHelper
    {
        internal List<T> ExecuteReaderWithTran<T>(DbContext dbContext, List<SqlParameter> pars, string sqlStr, string optionalErrorLog)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<T> result = dbContext.ExecuteReader<T>(sqlStr, pars?.ToArray());
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error(optionalErrorLog, ex);
                    throw ex;
                }
            }
        }

        internal List<T> ExecuteReaderWithoutTran<T>(DbContext dbContext, List<SqlParameter> pars, string fullProcName, string optionalErrorLog)
        {
            try
            {
                List<T> result = dbContext.ExecuteReader<T>(fullProcName, pars?.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(optionalErrorLog, ex);
                throw ex;
            }
        }

        internal T ExecuteScalarWithTran<T>(DbContext dbContext, List<SqlParameter> pars, string fullProcName, string optionalErrorLog)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    T result = dbContext.ExecuteScalar<T>(fullProcName, pars?.ToArray());
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error(optionalErrorLog, ex);
                    throw ex;
                }
            }
        }

        internal T ExecuteScalarWithoutTran<T>(DbContext dbContext, List<SqlParameter> pars, string fullProcName, string optionalErrorLog)
        {
            try
            {
                T result = dbContext.ExecuteScalar<T>(fullProcName, pars?.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(optionalErrorLog, ex);
                throw ex;
            }
        }

        internal void ExecuteNonQueryWithTran(DbContext dbContext, List<SqlParameter> pars, string fullProcName, string errorLog)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.ExecuteNonQuery(fullProcName, pars?.ToArray());
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Log.Error("Commit Exception Type: {0}", ex.GetType().ToString());
                    Log.Error("  Message: {0}", ex.Message);
                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Log.Error("Rollback Exception Type: {0}", ex2.GetType().ToString());
                        Log.Error("  Message: {0}", ex2.Message);
                    }
                    throw ex;
                }
            }
        }

        /* TODO beilleszteni
         catch (Exception ex)
        {
            Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
            Console.WriteLine("  Message: {0}", ex.Message);
            // Attempt to roll back the transaction.
            try
            {
                transaction.Rollback();
            }
            catch (Exception ex2)
            {
                // This catch block will handle any errors that may have occurred
                // on the server that would cause the rollback to fail, such as
                // a closed connection.
                Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                Console.WriteLine("  Message: {0}", ex2.Message);
            }
        }
         * */

        internal void ExecuteNonQueryWithoutTran(DbContext dbContext, List<SqlParameter> pars, string fullProcName, string optionalErrorLog)
        {
            try
            {
                dbContext.ExecuteNonQuery(fullProcName, pars?.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(optionalErrorLog, ex);
                throw ex;
            }
        }
    }
}