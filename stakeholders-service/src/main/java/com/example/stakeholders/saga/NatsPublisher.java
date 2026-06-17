package com.example.stakeholders.saga;

import io.nats.client.Connection;
import io.nats.client.Nats;
import io.nats.client.Options;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.nio.charset.StandardCharsets;
import java.time.Duration;
import java.util.concurrent.TimeoutException;
import java.util.logging.Level;
import java.util.logging.Logger;

@Component
public class NatsPublisher {

    private static final Logger logger = Logger.getLogger(NatsPublisher.class.getName());

    private final Connection nc;

    public NatsPublisher(
            @Value("${nats.url}") String natsUrl
    ) {
        try {
            Options options = Options.builder()
                    .server(natsUrl)
                    .connectionTimeout(Duration.ofSeconds(5))
                    .build();

            this.nc = Nats.connect(options);

            logger.info("Connected to NATS successfully: " + natsUrl);

        } catch (Exception e) {
            throw new RuntimeException("Failed to connect to NATS", e);
        }
    }

    public void publish(String subject, String userId, String username) {

        String json = """
        {
            "userId": "%s",
            "username": "%s"
        }
        """.formatted(userId, username);

        try {
            nc.publish(subject, json.getBytes(StandardCharsets.UTF_8));
            nc.flush(Duration.ofSeconds(1));

            logger.info("Published event to NATS subject: " + subject);

        } catch (TimeoutException e) {
            logger.log(Level.WARNING,
                    "NATS flush timeout (message likely delivered): " + subject, e);

        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            logger.log(Level.WARNING,
                    "NATS interrupted: " + subject, e);

        } catch (Exception e) {
            logger.log(Level.SEVERE,
                    "Failed to publish NATS message: " + subject, e);
        }
    }

    public Connection getConnection() {
        return this.nc;
    }
}