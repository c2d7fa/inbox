using System;
using System.Collections.Generic;
using System.Net;
using Npgsql;

namespace Inbox.Server {
    public class PostgresStorage : IStorage {
        private readonly string connectionString;

        private NpgsqlConnection Connection {
            get {
                var result = new NpgsqlConnection(connectionString);
                result.Open();
                return result;
            }
        }

        public PostgresStorage(string connectionString) {
            this.connectionString = connectionString;
        }

        public IEnumerable<Message> Unread {
            get {
                var result = new List<Message>();

                using var command = new NpgsqlCommand("SELECT id, created, HOST(author), content FROM unread ORDER BY created", Connection);
                using var reader = command.ExecuteReader();
                while (reader.Read()) {
                    result.Add(
                        new Message(
                            reader.GetGuid(0),
                            reader.GetDateTime(1),
                            IPAddress.Parse(reader.GetString(2)),
                            reader.GetString(3)
                        )
                    );
                }

                return result;
            }
        }

        public void Create(Guid uuid, IPAddress author, string content) {
            using var command = new NpgsqlCommand("INSERT INTO message (id, created, author, content) VALUES (@id, NOW(), @author, @content)", Connection);
            command.Parameters.AddWithValue("id", uuid);
            command.Parameters.AddWithValue("author", author);
            command.Parameters.AddWithValue("content", content);
            command.ExecuteNonQuery();
        }

        public void Read(Guid uuid) {
            using var command = new NpgsqlCommand("INSERT INTO read (message_id) VALUES (@id)", Connection);
            command.Parameters.AddWithValue("id", uuid);
            command.ExecuteNonQuery();
        }
    }
}
