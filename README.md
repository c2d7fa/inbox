# Buliding

Build and optionally push the Docker image:

    $ docker build . -t c2d7fa/inbox
    $ docker push c2d7fa/inbox

# Azure Table Storage

Set the environment variable `AzureConnectionString` to your Azure Storage
connection string; for example:

    $ export AzureConnectionString='DefaultEndpointsProtocol=https;AccountName=<your account name>;AccountKey=<your account key>;EndpointSuffix=core.windows.net'

You can find the connection string under *Access keys* under your storage
account in the Azure portal.

You may need to manually create the tables `Authentication`, `UnreadMessages`
and `ReadMessages`.

You'll need to manually add an entry to the `Authentication` table whose row key
is a token that you want to use to authenticate. Then you'll also need to
manually set the cookie `InboxAuthenticationToken` to this same value in your
browser.

# Postgres

Additionally, `PostgresConnectionString` should be set to a connection string
for a Postgres instance; for example:

    $ export PostgresConnectionString='Host=localhost;Username=postgres;Password=postgres;Database=postgres'

To initialize the Postrges database, run the commands in the `schema.sql` script
in the top-level directory of this repository.

# Deployment

After setting the environment variables mentioned above, run the image:

    $ docker run -ti --rm -e AzureConnectionString -e PostgresConnectionString -p 8080:80 c2d7fa/inbox
