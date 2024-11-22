CREATE KEYSPACE IF NOT EXISTS chatapp
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};

CREATE TABLE IF NOT EXISTS chatapp.messages (
    chat_id UUID,
    message_id TIMEUUID,
    sender_id UUID,
    body TEXT,
    timestamp TIMESTAMP,
    PRIMARY KEY ((chat_id), message_id)
) WITH CLUSTERING ORDER BY (message_id DESC);

CREATE TABLE IF NOT EXISTS chatapp.chat_access (
    user_id UUID,
    chat_id UUID,
    PRIMARY KEY (user_id, chat_id)
);

INSERT INTO chatapp.chat_access (user_id, chat_id) VALUES (3f2504e0-4f89-11d3-9a0c-0305e82c3301, f47ac10b-58cc-4372-a567-0e02b2c3d479);