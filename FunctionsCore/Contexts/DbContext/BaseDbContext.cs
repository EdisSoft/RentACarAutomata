using FunctionsCore.Commons.Entities;
using FunctionsCore.Commons.Entities.Base;
using FunctionsCore.Contexts;
using FunctionsCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FunctionsCore
{
    public class BaseDbContext : DbContext
    {
        public static ConcurrentDictionary<string, ConcurrentDictionary<Type, BaseDbContext>> GlobalContexts { get; set; }

        static BaseDbContext()
        {
            GlobalContexts = new ConcurrentDictionary<string, ConcurrentDictionary<Type, BaseDbContext>>();
        }

        /// <summary>
        /// Kontextus elkérése vagy létrehozása
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultKey">Ha nincs HttpContext, akkor ezzel a kulccsal lehet hivatkozni a létrehozott kontextusra</param>
        /// <returns></returns>
        public static T GetContextInstance<T>(string defaultKey = null) where T : BaseDbContext
        {
            var key = BaseAppContext.Instance?.CurrentHttpContext?.TraceIdentifier;

            if (key == null && defaultKey != null)
                key = "static-" + defaultKey;

            if (key == null)
                return Activator.CreateInstance<T>();

            if (!GlobalContexts.ContainsKey(key))
                GlobalContexts.TryAdd(key, new ConcurrentDictionary<Type, BaseDbContext>());

            if (!GlobalContexts[key].ContainsKey(typeof(T)))
            {
                var context = Activator.CreateInstance<T>();
                GlobalContexts[key].TryAdd(typeof(T), context);
                return context;
            }

            GlobalContexts[key].TryGetValue(typeof(T), out BaseDbContext resultContext);
            return (T)resultContext;
        }

        /// <summary>
        /// Kontextus törlése
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultKey">Ha meg van adva kulcs, akkor ezzel hivatkozott kontextust törli, egyébként a HttpContext.TraceIdentifier értékéhez kapcsolódót</param>
        public static void DeleteContext<T>(string defaultKey = null) where T : BaseDbContext
        {
            string key = null;

            if (defaultKey != null)
                key = "static-" + defaultKey;

            if (key == null)
                key = BaseAppContext.Instance.CurrentHttpContext?.TraceIdentifier;

            if (key == null)
                return;

            if (!GlobalContexts.ContainsKey(key))
                return;

            GlobalContexts.TryRemove(key, out ConcurrentDictionary<Type, BaseDbContext> resultContext);
            if (resultContext != null)
                resultContext.Clear();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettingsBase.GetConnectionString("Fonix3DbContext"), x => x.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SetQueryFilterOnAllEntities<SoftDeleteEntity>(x => !x.ToroltFl);
            modelBuilder.SetConverterOnAllJsonProperty();
           
        }

        public int SaveChangesRekordNaplo(
            bool createRekordNaploIfNotExists = true,
            bool updateRekordNaploBaratId = true, 
            bool updateRekordNaploDatum = true)
        {
            this.ChangeTracker.DetectChanges();
            foreach (var entry in this.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).Where(x => x.Entity.GetType().IsSubclassOf(typeof(ExtendedEntity))))
            {
                ExtendedEntity entity = (ExtendedEntity)entry.Entity;
                if (createRekordNaploIfNotExists && entity.RekordNaplo == null)
                {
                    entity.RekordNaplo = new Commons.EntitiesJson.RekordNaploJson()
                    {
                        UtolsoModositoBaratId = -1,
                        UtolsoModositasDatuma = DateTime.Now
                    };
                }

                if (updateRekordNaploBaratId)
                {
                    //if (BaseAppContext.Instance.CurrentHttpContext == null)
                    {
                        entity.RekordNaplo.UtolsoModositoBaratId = -1;
                    }
                }

                if (updateRekordNaploDatum) { 
                    entity.RekordNaplo.UtolsoModositasDatuma = DateTime.Now;
                }

                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entity.ToroltFl = true;
                        Entry(entity).State = EntityState.Modified;
                        break;
                }
            }

            int result = base.SaveChanges();
            return result;
        }

        public int GetNextValueForSequence(string sequenceName)
        {
            try
            {
                var nextVal = this.ExecuteReader<int>($"SELECT NEXT VALUE FOR {sequenceName}").SingleOrDefault();

                return nextVal;
            }
            catch (Exception e)
            {
                Log.Error($"Hiba {sequenceName} szekvencia következő értékének lekérése közben: {e}");
                throw e;
            }
        }

        /// <summary>
        /// !! Mellékhatások: RekordNaplót frissíti HttpContext alapján (requesten kívül használva UtolsoModositoBaratId=-1), Törölt flaget állít valós törlés helyett
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return SaveChangesRekordNaplo(true, true, true);
        }

        public DbSet<Aktivitas> Aktivitasok { get; set; }
        public DbSet<Cimke> Cimkek { get; set; }
        public DbSet<Felho> Felhok { get; set; }
       
    }
}
