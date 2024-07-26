using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers
{
    public static class ClearTable
    {
        /// <summary>
        /// Clear the table of the specified entity.
        /// </summary>
        public static void Clear(DbContext context, System.Type entitySysType)
        {
            var entityType = context.Model.FindEntityType(entitySysType);
            context.Database.ExecuteSqlRaw($"DELETE FROM {entityType.GetSchema()}.{entityType.GetTableName()}");
            context.SaveChanges();
        }

        /// <summary>
        /// Clears the tables in the correct order based on foreign key dependencies.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityTypes"></param>
        public static void ClearTables(this DbContext context, List<System.Type> entityTypes)
        {
            var sortedEntities = GetEntitiesInDependencyOrder(context, entityTypes);
            foreach (var entity in sortedEntities)
            {
                Clear(context, entity.ClrType);
            }
        }

        private static List<IEntityType> GetEntitiesInDependencyOrder(DbContext context, List<System.Type> entityTypes)
        {
            var entities = context.Model.GetEntityTypes()
            .Where(e => entityTypes.Contains(e.ClrType))
            .ToList();

            var sortedEntities = new List<IEntityType>();
            var visitedEntities = new HashSet<IEntityType>();

            void Visit(IEntityType entity)
            {
                if (!visitedEntities.Contains(entity))
                {
                    visitedEntities.Add(entity);

                    foreach (var foreignKey in entity.GetForeignKeys())
                    {
                        if (entityTypes.Contains(foreignKey.PrincipalEntityType.ClrType))
                        {
                            Visit(foreignKey.PrincipalEntityType);
                        }
                    }
                    sortedEntities.Add(entity);
                }
            }

            foreach (var entity in entities)
            {
                Visit(entity);
            }

            sortedEntities.Reverse();

            return sortedEntities;
        }
    }
}
