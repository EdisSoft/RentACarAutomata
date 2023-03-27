using FunctionsCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FunctionsCore.Utilities
{
    public static class ModelBuilderExtensions
    {
        public static void SetConverterOnAllJsonProperty(this ModelBuilder builder)
        {
            foreach (var dbSetType in builder.Model.GetEntityTypes().Select(t => t.ClrType))
            {
                if (dbSetType.GetCustomAttributes(typeof(KeylessAttribute), true).Length > 0)
                {
                    builder.Entity(dbSetType).HasNoKey();
                }

                // minden json property kigyűjtés
                //var properties = dbSetType.GetProperties().Where(prop => prop.PropertyType.GetCustomAttributes(typeof(JsonEntityAttribute), true).FirstOrDefault() != null).ToList();
                var properties = dbSetType.GetProperties().Where(prop => prop.PropertyType.GetCustomAttributes(typeof(JsonEntityAttribute), true).FirstOrDefault() != null &&
                    prop.GetCustomAttributes(typeof(NotMappedAttribute), true).FirstOrDefault() == null).ToList();

                if (properties.Count == 0)
                    continue;
           

                // modelbuilder.Entity<> method kikeresése
                var entityMethod = builder.GetType().GetMethods().Single(x => x.Name == nameof(ModelBuilder.Entity) && x.IsGenericMethod && x.GetParameters().Length == 0);
                // entity method hívása
                var entity = entityMethod.MakeGenericMethod(dbSetType).Invoke(builder, null);

                // json extension method megkeresése
                var jsonmethod = typeof(ValueConversionExtensions).GetMethod(nameof(ValueConversionExtensions.HasJsonConversion));

                foreach (var property in properties)
                {
                    // linq paraméter létrehozása, lambda bal oldala: x=>
                    ParameterExpression p = Expression.Parameter(dbSetType, "x");

                    // linq property létrehozása, lambda jobb oldala: => x.propname
                    MemberExpression prop = Expression.Property(p, property.Name);

                    // linq kifejezés előkészítése
                    var delegateType = typeof(Func<,>).MakeGenericType(dbSetType, property.PropertyType);

                    // linq teljes összeállítása
                    var lambda = Expression.Lambda(delegateType, prop, p);

                    // .Property<> method kikeresése
                    var propMethod = entity.GetType().GetMethods().Single(x => x.Name == nameof(EntityTypeBuilder.Property) && x.IsGenericMethod && x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType.Name != nameof(String));

                    // .Property<> method elkészítés
                    var m = propMethod.MakeGenericMethod(property.PropertyType);

                    // .Property<x => x.prop> method hívás
                    var jsonprop = m.Invoke(entity, new[] { lambda });

                    // .HasJsonConversion() hívás
                    jsonmethod.MakeGenericMethod(property.PropertyType).Invoke(jsonprop, new[] { jsonprop });
                }
            }
        }
        static void SetEntityQueryFilter<TEntityInterface>(this ModelBuilder builder, Type entityType, Expression<Func<TEntityInterface, bool>> filterExpression)
        {
            SetQueryFilterMethod
              .MakeGenericMethod(entityType, typeof(TEntityInterface))
              .Invoke(null, new object[] { builder, filterExpression });
        }
        public static void SetQueryFilterOnAllEntities<TEntityInterface>(this ModelBuilder builder, Expression<Func<TEntityInterface, bool>> filterExpression)
        {
            foreach (var type in builder.Model.GetEntityTypes().Where(t => t.BaseType == null).Select(t => t.ClrType).Where(t => typeof(TEntityInterface).IsAssignableFrom(t)))
            {
                builder.SetEntityQueryFilter<TEntityInterface>(type, filterExpression);
            }
        }

        static void SetQueryFilter<TEntity, TEntityInterface>(this ModelBuilder builder, Expression<Func<TEntityInterface, bool>> filterExpression)
            where TEntityInterface : class
            where TEntity : class, TEntityInterface
        {
            var concreteExpression = filterExpression.Convert<TEntityInterface, TEntity>();
            builder.Entity<TEntity>().HasQueryFilter(concreteExpression);
        }

        static readonly MethodInfo SetQueryFilterMethod = typeof(ModelBuilderExtensions)
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                        .Single(t => t.IsGenericMethod && t.Name == nameof(SetQueryFilter));
    }
}
