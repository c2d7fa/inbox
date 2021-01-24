# Building and deploying server

Build and optionally push the Docker image:

    $ docker build . -t c2d7fa/inbox
    $ docker push c2d7fa/inbox

Run the image with the environment variable `ConnectionString` set to your Azure
Storage connection string. For example:

    $ export ConnectionString='DefaultEndpointsProtocol=https;AccountName=<your account name>;AccountKey=<your account key>;EndpointSuffix=core.windows.net'
    $ docker run -ti --rm -e ConnectionString -p 8080:80 inbox

You can find the connection string under *Access keys* under your storage
account in the Azure portal.

You may need to manually create the tables `Authentication`, `UnreadMessages`
and `ReadMessages`.

You'll need to manually add an entry to the `Authentication` table whose row key
is a token that you want to use to authenticate. Then you'll also need to
manually set the cookie `InboxAuthenticationToken` to this same value in your
browser.
