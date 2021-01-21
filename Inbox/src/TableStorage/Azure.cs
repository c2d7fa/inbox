using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox.TableStorage {
    public class AzureTable : ITable {
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

        public bool HasRow(string key) {
            var query = new TableQuery().Where(
                TableQuery.GenerateFilterCondition("RowKey", "eq", key)
            );
            return cloudTable.ExecuteQuery(query).Any();
        }

        public IEnumerable<IEntity> AllEntities {
            get {
                if (cloudTable.ExecuteQuery(new TableQuery()) is IEnumerable<DynamicTableEntity> entities) {
                    return entities.Select(entity => new AzureDynamicEntity(entity));
                } else {
                    return new List<IEntity>();
                }
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
