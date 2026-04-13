package com.example.stakeholders.controller;

import com.example.stakeholders.dto.JwtAuthenticationRequest;
import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.RoleType;
import com.example.stakeholders.model.User;
import com.example.stakeholders.repository.UserRepository;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.sql.Timestamp;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class AuthenticationFlowIntegrationTest {

    private final ObjectMapper objectMapper = new ObjectMapper();
    private final HttpClient httpClient = HttpClient.newHttpClient();

    @Value("${local.server.port}")
    private int port;

    @org.springframework.beans.factory.annotation.Autowired
    private UserRepository userRepository;

    @org.springframework.beans.factory.annotation.Autowired
    private PasswordEncoder passwordEncoder;

    @Test
    void guideLoginAndGetMyInfoStillWorksForExistingAccount() throws Exception {
        String username = "guide_" + uniqueSuffix();
        User guide = new User();
        guide.setUsername(username);
        guide.setPassword(passwordEncoder.encode("guide123"));
        guide.setLastPasswordResetDate(new Timestamp(System.currentTimeMillis() - 5_000));
        guide.setFirstName("Guide");
        guide.setLastName("Tester");
        guide.setEmail(username + "@example.com");
        guide.setEnabled(true);
        guide.setRole(RoleType.GUIDE);
        userRepository.save(guide);

        String token = loginAndGetToken(username, "guide123");

        HttpResponse<String> myInfoResponse = sendJsonRequest("GET", "/api/getMyInfo", null, token);
        assertEquals(200, myInfoResponse.statusCode());

        JsonNode myInfoJson = objectMapper.readTree(myInfoResponse.body());
        assertEquals("Guide", myInfoJson.get("firstName").asText());
        assertEquals("Tester", myInfoJson.get("lastName").asText());
        assertTrue(myInfoJson.get("biography").isNull());
        assertTrue(myInfoJson.get("moto").isNull());
        assertTrue(myInfoJson.get("profilePicture").isNull());
    }

    @Test
    void touristSignupAndLoginStillWorks() throws Exception {
        String username = "tourist_" + uniqueSuffix();

        UserRequest touristRequest = new UserRequest();
        touristRequest.setUsername(username);
        touristRequest.setPassword("tourist123");
        touristRequest.setFirstname("Tourist");
        touristRequest.setLastname("Tester");
        touristRequest.setEmail(username + "@example.com");
        touristRequest.setRole(RoleType.TOURIST);

        HttpResponse<String> signupResponse = sendJsonRequest("POST", "/auth/signup",
                objectMapper.writeValueAsString(touristRequest), null);
        assertEquals(201, signupResponse.statusCode());

        JsonNode signupJson = objectMapper.readTree(signupResponse.body());
        assertEquals(username, signupJson.get("username").asText());
        assertEquals("TOURIST", signupJson.get("role").asText());
        assertFalse(signupJson.has("password"));

        String token = loginAndGetToken(username, "tourist123");
        assertNotNull(token);
        assertFalse(token.isBlank());
    }

    private String loginAndGetToken(String username, String password) throws Exception {
        JwtAuthenticationRequest loginRequest = new JwtAuthenticationRequest(username, password);
        HttpResponse<String> loginResponse = sendJsonRequest("POST", "/auth/login",
                objectMapper.writeValueAsString(loginRequest), null);

        assertEquals(200, loginResponse.statusCode());

        JsonNode loginJson = objectMapper.readTree(loginResponse.body());
        assertTrue(loginJson.hasNonNull("accessToken"));
        assertTrue(loginJson.hasNonNull("expiresIn"));
        return loginJson.get("accessToken").asText();
    }

    private HttpResponse<String> sendJsonRequest(String method, String path, String body, String token) throws Exception {
        HttpRequest.Builder builder = HttpRequest.newBuilder()
                .uri(URI.create("http://localhost:" + port + path))
                .header(HttpHeaders.ACCEPT, MediaType.APPLICATION_JSON_VALUE);

        if (body != null) {
            builder.header(HttpHeaders.CONTENT_TYPE, MediaType.APPLICATION_JSON_VALUE);
        }

        if (token != null) {
            builder.header(HttpHeaders.AUTHORIZATION, "Bearer " + token);
        }

        if ("POST".equals(method)) {
            builder.POST(HttpRequest.BodyPublishers.ofString(body != null ? body : ""));
        } else {
            builder.GET();
        }

        return httpClient.send(builder.build(), HttpResponse.BodyHandlers.ofString());
    }

    private String uniqueSuffix() {
        return UUID.randomUUID().toString().replace("-", "").substring(0, 8);
    }
}
