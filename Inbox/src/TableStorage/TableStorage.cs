using System.Collections.Generic;

namespace Inbox.TableStorage {
    public class Entity : IEntity {
        public string Partition { get; }
        public string Row { get; }

        private readonly Dictionary<string, dynamic> properties = new Dictionary<string, dynamic>();

        public Entity(string partition, string row) {
            Partition = partition;
            Row = row;
        }

        public IReadOnlyDictionary<string, dynamic> Properties => properties;

        public void Set(string key, dynamic value) {
            properties[key] = value;
        }

        public object? Property(string key) {
            return properties[key];
        }
    }

    public interface IEntity {
        string Partition { get; }
        string Row { get; }
        object? Property(string key);
        IReadOnlyDictionary<string, dynamic> Properties { get; }
    }

    public interface ITable {
        void Insert(IEntity entity);
        bool HasRow(string key);
        IEnumerable<IEntity> AllEntities { get; }
    }
}
