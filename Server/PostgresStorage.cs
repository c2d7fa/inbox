using System;
using System.Collections.Generic;
using System.Net;
using Npgsql;

namespace Inbox.Server {
    public class PostgresStorage : IStorage {
        private readonly string connectionString;

        private NpgsqlConnection Connect() {
            var result = new NpgsqlConnection(connectionString);
            result.Open();
            return result;
        }

        public PostgresStorage(string connectionString) {
            this.connectionString = connectionString;
        }

        public IEnumerable<Message> Unread {
            get {
                var result = new List<Message>();

                using var connection = Connect();
                using var command = new NpgsqlCommand("SELECT id, created, HOST(author), content, urgent FROM unread ORDER BY created", connection);
                using var reader = command.ExecuteReader();
                while (reader.Read()) {
                    result.Add(
                        new Message(
                            reader.GetGuid(0),
                            reader.GetDateTime(1),
                            IPAddress.Parse(reader.GetString(2)),
                            reader.GetString(3),
                            reader.GetBoolean(4)
                        )
                    );
                }

                return result;
            }
        }

        public void Create(Guid uuid, IPAddress author, string content, bool isUrgent) {
            using var connection = Connect();
            using var command = new NpgsqlCommand("INSERT INTO message (id, created, author, content, urgent) VALUES (@id, NOW(), @author, @content, @urgent)", connection);
            command.Parameters.AddWithValue("id", uuid);
            command.Parameters.AddWithValue("author", author);
            command.Parameters.AddWithValue("content", content);
            command.Parameters.AddWithValue("urgent", isUrgent);
            command.ExecuteNonQuery();
        }

        public void Read(Guid uuid) {
            using var connection = Connect();
            using var command = new NpgsqlCommand("INSERT INTO read (message_id) VALUES (@id)", connection);
            command.Parameters.AddWithValue("id", uuid);
            command.ExecuteNonQuery();
        }
    }
}
