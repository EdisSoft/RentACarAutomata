using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FunctionsCore.Utilities.SqlHelper
{
    public class SqlProcExecuter
    {
        /*TODO: 
        1. AddStructuredPar -al átadott parnál számít a property-k sorrendje
        2. tranzakció indításakor meg kell előbb vizsgálni, hogy van-e már tranzakció
        3. timeout/deadlock exception esetére lehessen megadni, hogy hányszor próbálkozzon újra
        */
        private string schemaName;
        private string procName;
        private List<SqlParameter> procParams;
        private DbContext dbContext;
        private List<string> procParamNames;
        ParamHelper paramHelper => new ParamHelper();
        ExecuterHelper executerHelper => new ExecuterHelper();

        public SqlProcExecuter(string schemaName, string procName, DbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(schemaName)) throw new Exception($"{nameof(schemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(procName)) throw new Exception($"{nameof(procName)} megadása kötelező!");
            if (dbContext == null) throw new Exception($"{nameof(dbContext)} nem lehet null!");
            this.schemaName = paramHelper.Trim(schemaName);
            this.procName = paramHelper.Trim(procName);
            this.dbContext = dbContext;
            procParamNames = new List<string>();
            this.procParams = new List<SqlParameter>();
        }

        public SqlProcExecuter AddPar(string paramName, object value)
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            procParamNames.Add(paramName);
            procParams.Add(new SqlParameter(paramName, value ?? DBNull.Value));
            return this;
        }

        public SqlProcExecuter AddSympleListPar<T>(string paramName, string typeSchemaName, string typeName, List<T> list) where T : notnull
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(typeSchemaName)) throw new Exception($"{nameof(typeSchemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(typeName)) throw new Exception($"{nameof(typeName)} megadása kötelező!");
            if (list == null) throw new Exception($"{nameof(list)} értéke nem lehet null!");

            procParamNames.Add(paramName);
            procParams.Add(paramHelper.CreateSympleListParameter(paramName, typeSchemaName, typeName, list));

            return this;
        }

        /// <summary>
        /// Tárolt eljárás végrehajtásához létrehoz egy tetszőleges tábla típusú paramétert.
        /// </summary>
        /// <typeparam name="T">Tábla paraméter rekordjának megfelelő típus.</typeparam>
        /// <param name="paramValue">A megadott generikus T típusból álló lista.</param>
        /// <param name="paramName">A tárolt eljárásban használt típus név, "@" kiírása nem kötelező.</param>
        /// <param name="paramTypeName">Az adatbázisban megadott típusnév.</param>
        /// <returns></returns>
        public SqlProcExecuter AddStructuredPar<T>(string paramName, string paramTypeSchemaName, string paramTypeName, List<T> paramValue) where T : class
        {
            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeSchemaName)) throw new Exception($"{nameof(paramTypeSchemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeName)) throw new Exception($"{nameof(paramTypeName)} megadása kötelező!");
            if (paramValue == null)
            {
                throw new Exception($"{nameof(paramValue)} értéke nem lehet null!");
            }
            procParamNames.Add(paramName);
            SqlParameter parameter = paramHelper.GetStructuredPar<T>(paramName, paramTypeSchemaName, paramTypeName, paramValue);
            procParams.Add(parameter);
            return this;
        }

        public void ExecuteNonQuery(bool inTran = false, string optionalErrorLog = "Hiba a tárolt futtatása során: ")
        {
            if (inTran) executerHelper.ExecuteNonQueryWithTran(dbContext, procParams, GetFullProcName(), optionalErrorLog);
            else executerHelper.ExecuteNonQueryWithoutTran(dbContext, procParams, GetFullProcName(), optionalErrorLog);
        }
            
        public List<T> ExecuteReader<T>(bool inTran = false, string optionalErrorLog = "Hiba a tárolt futtatása során: ")
        {
            if (inTran) return executerHelper.ExecuteReaderWithTran<T>(dbContext, procParams, GetFullProcName(), optionalErrorLog);
            else return executerHelper.ExecuteReaderWithoutTran<T>(dbContext, procParams, GetFullProcName(), optionalErrorLog);
        }

        public T ExecuteScalar<T>(bool inTran = false, string optionalErrorLog = "Hiba a tárolt futtatása során: ")
        {
            if (inTran) return executerHelper.ExecuteScalarWithTran<T>(dbContext, procParams, GetFullProcName(), optionalErrorLog);
            else return executerHelper.ExecuteScalarWithoutTran<T>(dbContext, procParams, GetFullProcName(), optionalErrorLog);
        }

        string GetFullProcName() => $"{GetDatabaseStandardName(schemaName, procName)} {GetConcatenatedParamNames()}";

        string GetDatabaseStandardName(string dbSchemaName, string dbObjectName) => $"[{paramHelper.Trim(dbSchemaName)}].[{paramHelper.Trim(dbObjectName)}]";

        string GetConcatenatedParamNames() => string.Join(", ", procParamNames.Select(x => $"@{x.Trim().TrimStart('@')}"));
    }
}
