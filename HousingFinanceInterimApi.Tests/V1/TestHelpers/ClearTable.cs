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

            var dependencyCount = new Dictionary<IEntityType, int>();
            var dependents = new Dictionary<IEntityType, List<IEntityType>>();
            var queue = new Queue<IEntityType>();
            var sortedEntities = new List<IEntityType>();

            // Initialize the dictionaries
            foreach (var entity in entities)
            {
                dependencyCount[entity] = 0;
                dependents[entity] = new List<IEntityType>();
            }

            // Fill the dependency count and dependents
            foreach (var entity in entities)
            {
                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    var principalEntity = foreignKey.PrincipalEntityType;
                    if (entityTypes.Contains(principalEntity.ClrType))
                    {
                        dependencyCount[entity]++;
                        dependents[principalEntity].Add(entity);
                    }
                }
            }

            // Add entities with no dependencies to the queue
            foreach (var entity in entities)
            {
                if (dependencyCount[entity] == 0)
                {
                    queue.Enqueue(entity);
                }
            }

            // Process the entities
            while (queue.Count > 0)
            {
                var entity = queue.Dequeue();
                sortedEntities.Add(entity);

                foreach (var dependent in dependents[entity])
                {
                    dependencyCount[dependent]--;
                    if (dependencyCount[dependent] == 0)
                    {
                        queue.Enqueue(dependent);
                    }
                }
            }
            sortedEntities.Reverse();

            return sortedEntities;
        }
    }
}
