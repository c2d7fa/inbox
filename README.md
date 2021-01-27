# Inbox

This is a public inbox. It's an alternative to sending emails to yourself that
doesn't require logging in to anything. You can add links and messages from
anywhere, and then read through them later.

I originally made this to try out
[Azure Functions](https://azure.microsoft.com/en-us/services/functions/),
but later decided to run it on my own server instead, because the cold-start
latency was too annoying.

## Building

Build and optionally push the Docker image:

    $ docker build . -t c2d7fa/inbox
    $ docker push c2d7fa/inbox

## Configuration

Set the environment variable `AuthenticationToken` to a token that you want to
use for authentication. Then manually set the cookie `InboxAuthenticationToken`
to this same value in your browser.

    $ export AuthenticationToken='<secret token>'

The environment variable `PostgresConnectionString` should be set to a
connection string for a Postgres instance; for example:

    $ export PostgresConnectionString='Host=localhost;Username=postgres;Password=postgres;Database=postgres'

To initialize the Postrges database, run the commands in the `schema.sql` script
in the top-level directory of this repository.

## Deployment

After setting the environment variables and database mentioned above, run the
image:

    $ docker run -ti --rm -e AuthenticationToken -e PostgresConnectionString -p 8080:80 c2d7fa/inbox
