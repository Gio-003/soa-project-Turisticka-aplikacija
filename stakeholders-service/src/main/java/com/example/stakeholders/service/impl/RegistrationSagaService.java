package com.example.stakeholders.service.impl;

import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.RoleType;
import com.example.stakeholders.model.User;
import com.example.stakeholders.saga.NatsPublisher;
import com.example.stakeholders.service.inter.UserService;
import io.nats.client.Message;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.Duration;
import java.util.logging.Logger;

import static com.example.stakeholders.model.RoleType.GUIDE;

@Service
public class RegistrationSagaService {

    private static final Logger log = Logger.getLogger(RegistrationSagaService.class.getName());

    @Autowired private UserService userService;
    @Autowired private NatsPublisher natsPublisher;

    // Subject konstante
    private static final String BLOG_COMMAND  = "registration.blog.command";
    private static final String BLOG_REPLY    = "registration.blog.reply";
    private static final String TOUR_COMMAND  = "registration.tour.command";
    private static final String TOUR_REPLY    = "registration.tour.reply";

    public User register(UserRequest request) {
        // Korak 1: sačuvaj usera
        User user = userService.save(request);

        // Saga se pokreće samo za GUIDE (ili za sve — zavisi od zahtjeva)
        if (user.getRole() != RoleType.GUIDE) {
            return user;
        }

        String userId   = user.getId().toString();
        String username = user.getUsername();

        // Korak 2: kreira blog — čekamo reply (sync request-reply)
        String blogId = sendBlogCommand(userId, username);

        if (blogId == null) {
            // Blog nije uspio — rollback usera
            userService.deleteById(user.getId());
            throw new RuntimeException("Registration SAGA failed: could not create welcome blog");
        }

        // Korak 3: kreira draft turu — čekamo reply
        boolean tourOk = sendTourCommand(userId, username);

        if (!tourOk) {
            // Tour nije uspio — kompenzacija: obriši blog, pa usera
            rollbackBlog(userId, blogId);
            userService.deleteById(user.getId());
            throw new RuntimeException("Registration SAGA failed: could not create draft tour");
        }

        return user;
    }

    // Šalje komandu blog-serviceu i čeka reply (2 sekunde timeout)
    private String sendBlogCommand(String userId, String username) {
        try {
            String json = """
                {"userId":"%s","username":"%s","type":0}
                """.formatted(userId, username);

            // request-reply: NATS čeka reply na privremeni inbox subject
            Message reply = natsPublisher.getConnection()
                    .request(BLOG_COMMAND, json.getBytes(), Duration.ofSeconds(5));// 5s timeout

            var replyObj = parseReply(reply.getData());

            if ("WelcomeBlogCreated".equals(replyObj.type) || replyObj.typeInt == 0) {
                log.info("Blog created: " + replyObj.blogId);
                return replyObj.blogId;
            }
        } catch (Exception e) {
            log.warning("Blog command failed: " + e.getMessage());
        }
        return null;
    }

    // Šalje komandu tour-serviceu i čeka reply
    private boolean sendTourCommand(String userId, String username) {
        try {
            String json = """
                {"userId":"%s","username":"%s","type":0}
                """.formatted(userId, username);

            Message reply = natsPublisher.getConnection()
                    .request(TOUR_COMMAND, json.getBytes(), Duration.ofSeconds(5));

            var replyObj = parseReply(reply.getData());
            return "DraftTourCreated".equals(replyObj.type) || replyObj.typeInt == 0;

        } catch (Exception e) {
            log.warning("Tour command failed: " + e.getMessage());
            return false;
        }
    }

    // Kompenzacija: briše blog ako tura nije uspjela
    private void rollbackBlog(String userId, String blogId) {
        try {
            String json = """
                {"userId":"%s","blogId":"%s","type":2}
                """.formatted(userId, blogId);
            natsPublisher.getConnection().publish(BLOG_COMMAND, json.getBytes());
            log.info("Rollback blog command sent for blogId: " + blogId);
        } catch (Exception e) {
            log.severe("Failed to send rollback command: " + e.getMessage());
        }
    }

    // Jednostavan parser
    private ReplyData parseReply(byte[] data) throws Exception {
        var node = new com.fasterxml.jackson.databind.ObjectMapper()
                .readTree(data);
        var r = new ReplyData();
        r.blogId  = node.path("blogId").asText(null);
        r.tourId  = node.path("tourId").asText(null);
        r.type    = node.path("type").asText(null);
        r.typeInt = node.path("type").asInt(-1);
        return r;
    }

    private static class ReplyData {
        String blogId, tourId, type;
        int typeInt;
    }
}