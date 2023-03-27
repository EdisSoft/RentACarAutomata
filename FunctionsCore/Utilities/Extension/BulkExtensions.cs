using EFCore.BulkExtensions;
using FunctionsCore.Commons.Entities.Base;
using FunctionsCore.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionsCore.Utilities.Extension
{
    public static class BulkExtensions
    {
        public static void BulkInsertWithRekordNaplo<T>(this DbContext context, IList<T> entities) where T : ExtendedEntity
        {
            List<T> entitiesWithRekordNapo = CreateRekordNaplo(entities);
            var bulkConfig = new BulkConfig { SqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers };
            context.BulkInsert(entitiesWithRekordNapo, bulkConfig);
        }

        public static void BulkUpdateWithRekordNaplo<T>(this DbContext context, IList<T> entities, List<string> mezoKihagyas = null) where T : ExtendedEntity
        {
            List<T> entitiesWithRekordNapo = CreateRekordNaplo(entities);

            if (mezoKihagyas != null)
            {
                var bulkConfig = new BulkConfig { PropertiesToExclude = mezoKihagyas, SqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers };
                context.BulkUpdate(entitiesWithRekordNapo, bulkConfig);
            }
            else
            {
                var bulkConfig = new BulkConfig { SqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers };
                context.BulkUpdate(entitiesWithRekordNapo, bulkConfig);
            }


        }

        public static void BulkInsertOrUpdateWithRekordNaplo<T>(this DbContext context, IList<T> entities, List<string> mezoKihagyas = null) where T : ExtendedEntity
        {
            List<T> entitiesWithRekordNapo = CreateRekordNaplo(entities);

            if (mezoKihagyas != null)
            {
                var bulkConfig = new BulkConfig { PropertiesToExclude = mezoKihagyas, SqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers };
                context.BulkInsertOrUpdate(entitiesWithRekordNapo, bulkConfig);
            }
            else
            {
                var bulkConfig = new BulkConfig { SqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers };
                context.BulkInsertOrUpdate(entitiesWithRekordNapo, bulkConfig);
            }

        }

        private static List<T> CreateRekordNaplo<T>(IList<T> entities) where T : ExtendedEntity
        {
            var rekordNaplo = new Commons.EntitiesJson.RekordNaploJson();
            //if (BaseAppContext.Instance.CurrentHttpContext == null)
            rekordNaplo.UtolsoModositoBaratId = -1;


            rekordNaplo.UtolsoModositasDatuma = DateTime.Now;
            var entitiesWithRekordNapo = entities.ToList();
            entitiesWithRekordNapo.ForEach(f => f.RekordNaplo = rekordNaplo);
            return entitiesWithRekordNapo;
        }


    }
}
