CREATE TABLE IF NOT EXISTS chatapp.messages (
    chat_id UUID,
    message_id TIMEUUID,
    sender_id UUID,
    body TEXT,
    timestamp TIMESTAMP,
    PRIMARY KEY ((chat_id), message_id)
) WITH CLUSTERING ORDER BY (message_id DESC);