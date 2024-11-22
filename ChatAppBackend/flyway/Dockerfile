# Use the official Flyway image as the base
FROM flyway/flyway:latest

# Set working directory
WORKDIR /flyway

# Download Cassandra JDBC driver and Netty dependency during build
RUN curl -L -o /flyway/drivers/cassandra-jdbc-wrapper.jar https://repo1.maven.org/maven2/com/ing/data/cassandra-jdbc-wrapper/4.11.1/cassandra-jdbc-wrapper-4.11.1.jar && \
    curl -L -o /flyway/drivers/netty-all.jar https://repo1.maven.org/maven2/io/netty/netty-all/4.1.90.Final/netty-all-4.1.90.Final.jar && \
    curl -L -o /flyway/drivers/slf4j-api.jar https://repo1.maven.org/maven2/org/slf4j/slf4j-api/2.0.7/slf4j-api-2.0.7.jar && \
    curl -L -o /flyway/drivers/logback-classic.jar https://repo1.maven.org/maven2/ch/qos/logback/logback-classic/1.4.11/logback-classic-1.4.11.jar && \
    curl -L -o /flyway/drivers/logback-core.jar https://repo1.maven.org/maven2/ch/qos/logback/logback-core/1.4.11/logback-core-1.4.11.jar

# Ensure proper permissions for all added files
RUN chmod +r /flyway/drivers/*.jar

# Default entrypoint for Flyway
ENTRYPOINT ["flyway"]