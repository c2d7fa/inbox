using System.Collections.Generic;
using System.Linq;

using Inbox.Core.TableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox.Server {
    internal class AzureTable : ITable {
        private readonly CloudTable cloudTable;

        public AzureTable(CloudTable cloudTable) {
            this.cloudTable = cloudTable;
        }

        public void Insert(IEntity entity) {
            var properties = new Dictionary<string, EntityProperty>();
            foreach (var (key, value) in entity.Properties) {
                properties[key] = new EntityProperty(value);
            }

            var azureEntity = new DynamicTableEntity(entity.Partition, entity.Row, "", properties);
            cloudTable.Execute(TableOperation.Insert(azureEntity));
        }

        private DynamicTableEntity? GetAzureEntity(string key) {
            var matchingEntities = new List<DynamicTableEntity>(
                cloudTable.ExecuteQuery(
                    new TableQuery().Where(
                        TableQuery.GenerateFilterCondition("RowKey", "eq", key)
                    )
                )
            );

            return matchingEntities.Count != 1 ? null : matchingEntities[0];
        }

        public IEntity? Get(string key) {
            if (!(GetAzureEntity(key) is { } entity)) return null;
            return new AzureDynamicEntity(entity);
        }

        public void Delete(string key) {
            if (!(GetAzureEntity(key) is { } entity)) return;
            cloudTable.Execute(TableOperation.Delete(entity));
        }

        public bool HasRow(string key) {
            var query = new TableQuery().Where(
                TableQuery.GenerateFilterCondition("RowKey", "eq", key)
            );
            return cloudTable.ExecuteQuery(query).Any();
        }

        public IEnumerable<IEntity> AllEntities {
            get {
                if (!(cloudTable.ExecuteQuery(new TableQuery()) is { } entities)) return new List<IEntity>();
                return entities.Select(entity => new AzureDynamicEntity(entity));
            }
        }

        private class AzureDynamicEntity : IEntity {
            public AzureDynamicEntity(DynamicTableEntity entity) {
                Partition = entity.PartitionKey;
                Row = entity.RowKey;
                var properties = new Dictionary<string, dynamic>();
                foreach (var (key, value) in entity.Properties) {
                    properties[key] = value.PropertyAsObject;
                }

                Properties = properties;
            }

            public string Partition { get; }

            public string Row { get; }

            public object? Property(string key) {
                return Properties[key];
            }

            public IReadOnlyDictionary<string, dynamic> Properties { get; }
        }
    }
}
