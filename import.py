# Before running this script:
#
# 1. Install the packages `azure-cosmosdb-table` and `psycopg2`.
#
# 2. Set the environment variable `AzureConnectionString` as described in the
#    README.
#
# 3. Set the environment variables `import_connection_string` to a keyword/value
#    connection string in the form `host=localhost port=5432 dbname=mydb
#    user=postgres password=postgres`.

import os
import psycopg2
from azure.cosmosdb.table.tableservice import TableService

azure = TableService(connection_string=os.getenv("AzureConnectionString"))
postgres_connection = psycopg2.connect(os.getenv("import_connection_string"))
postgres = postgres_connection.cursor()

i = 0
for unread in azure.query_entities("UnreadMessages"):
  i += 1
  if i % 20 == 0:
    print("Inserted {} unread messages".format(i))
  author = unread["PartitionKey"]
  uuid = unread["RowKey"]
  created = unread["Created"]
  content = unread["Content"]
  postgres.execute("INSERT INTO message (id, author, created, content) VALUES (%s, %s, %s, %s)", (uuid, author, created, content))

i = 0
for read in azure.query_entities("ReadMessages"):
  i += 1
  if i % 20 == 0:
    print("Inserted {} read messages".format(i))
  author = read["PartitionKey"]
  uuid = read["RowKey"]
  created = read["Created"]
  updated = read["Timestamp"]
  content = read["Content"]
  postgres.execute("INSERT INTO message (id, author, created, content) VALUES (%s, %s, %s, %s)", (uuid, author, created, content))
  postgres.execute("INSERT INTO read (message_id, updated) VALUES (%s, %s)", (uuid, updated))

postgres_connection.commit()
postgres.close()
postgres_connection.close()
