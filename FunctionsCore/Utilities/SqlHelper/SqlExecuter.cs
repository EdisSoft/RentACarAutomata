using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FunctionsCore.Utilities.SqlHelper
{
    public class SqlExecuter
    {
        /*TODO: 
         1. a json mezőknél a json objektumok feltöltését automatizálni
         2. tranzakció indításakor meg kell előbb vizsgálni, hogy van-e már tranzakció
         3. timeout/deadlock exception esetére lehessen megadni, hogy hányszor próbálkozzon újra
        */
        DbContext dbContext;
        string queryString;
        List<SqlParameter> pars;
        ParamHelper paramHelper => new ParamHelper();
        ExecuterHelper executerHelper => new ExecuterHelper();

        public SqlExecuter(string commandOrQuery, DbContext dbContext)
        {
            if(string.IsNullOrWhiteSpace(commandOrQuery)) throw new Exception($"{nameof(commandOrQuery)} megadása kötelező!");
            if (dbContext == null) throw new Exception($"{nameof(dbContext)} nem lehet null!");
            this.dbContext = dbContext;
            this.queryString = commandOrQuery;
            this.pars = new List<SqlParameter>();
        }
        public SqlExecuter AddPar(string paramName, object value)
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            pars.Add(new SqlParameter(GetParNameWithoutKukac(paramName), value?? DBNull.Value));
            return this;
        }

        public SqlExecuter AddSympleListPar<T>(string paramName, string typeSchemaName, string typeName, List<T> list) where T : notnull
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(typeSchemaName)) throw new Exception($"{nameof(typeSchemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(typeName)) throw new Exception($"{nameof(typeName)} megadása kötelező!");
            if (list == null) throw new Exception($"{nameof(list)} értéke nem lehet null!");

            pars.Add(paramHelper.CreateSympleListParameter(paramName, typeSchemaName, typeName, list));

            return this;
        }

        public SqlExecuter AddStructuredPar<T>(string paramName, string paramTypeSchemaName, string paramTypeName, List<T> paramValue) where T : class
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeSchemaName)) throw new Exception($"{nameof(paramTypeSchemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeName)) throw new Exception($"{nameof(paramTypeName)} megadása kötelező!");
            if (paramValue == null) throw new Exception($"{nameof(paramValue)} értéke nem lehet null!");

            pars.Add(paramHelper.GetStructuredPar<T>(paramName, paramTypeSchemaName, paramTypeName, paramValue));
            
            return this;
        }

        public List<T> ExecuteReader<T>(bool inTran = false, string optionalErrorLog = "Hiba sql kifejezés futtatása során: ")
        {
            if (inTran) return executerHelper.ExecuteReaderWithTran<T>(dbContext, pars, queryString, optionalErrorLog);
            else return executerHelper.ExecuteReaderWithoutTran<T>(dbContext, pars, queryString, optionalErrorLog);
        }

        public T ExecuteScalar<T>(bool inTran = false, string optionalErrorLog = "Hiba a tárolt futtatása során: ")
        {
            if (inTran) return executerHelper.ExecuteScalarWithTran<T>(dbContext, pars, queryString, optionalErrorLog);
            else return executerHelper.ExecuteScalarWithoutTran<T>(dbContext, pars, queryString, optionalErrorLog);
        }

        public void ExecuteNonQuery(bool inTran = false, string optionalErrorLog = "Hiba a tárolt futtatása során: ")
        {
            if (inTran) executerHelper.ExecuteNonQueryWithTran(dbContext, pars, queryString, optionalErrorLog);
            else executerHelper.ExecuteNonQueryWithoutTran(dbContext, pars, queryString, optionalErrorLog);
        }

        string GetParNameWithoutKukac(string parName) => parName.Trim().Replace("@",string.Empty);
    }
}