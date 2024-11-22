CREATE KEYSPACE IF NOT EXISTS chatapp
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};