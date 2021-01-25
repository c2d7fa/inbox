CREATE TABLE message (
  id UUID PRIMARY KEY,
  author INET CHECK (HOST(author) = ABBREV(author)) NOT NULL, -- Just the host
  created TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
  content TEXT NOT NULL DEFAULT ''
);

CREATE TABLE read (
	message_id UUID PRIMARY KEY REFERENCES message (id),
	updated TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE VIEW unread AS (
	WITH unread_ids AS (
		SELECT id FROM message
		EXCEPT
		SELECT message_id FROM read
	)
	SELECT *
	FROM message
	WHERE id IN (SELECT id FROM unread_ids)
);
