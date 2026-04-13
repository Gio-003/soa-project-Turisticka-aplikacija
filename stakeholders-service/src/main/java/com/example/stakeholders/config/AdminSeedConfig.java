package com.example.stakeholders.config;

import com.example.stakeholders.model.RoleType;
import com.example.stakeholders.model.User;
import com.example.stakeholders.repository.UserRepository;
import org.springframework.boot.CommandLineRunner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.crypto.password.PasswordEncoder;

@Configuration
public class AdminSeedConfig {

    private static final String DEFAULT_ADMIN_USERNAME = "admin";
    private static final String DEFAULT_ADMIN_PASSWORD = "admin123";
    private static final String DEFAULT_ADMIN_EMAIL = "admin@soa.local";

    @Bean
    public CommandLineRunner seedDefaultAdmin(
            UserRepository userRepository,
            PasswordEncoder passwordEncoder
    ) {
        return args -> {
            if (userRepository.countByRole(RoleType.ADMIN) > 0) {
                return;
            }

            User admin = new User();
            admin.setUsername(DEFAULT_ADMIN_USERNAME);
            admin.setPassword(passwordEncoder.encode(DEFAULT_ADMIN_PASSWORD));
            admin.setFirstName("System");
            admin.setLastName("Admin");
            admin.setEmail(DEFAULT_ADMIN_EMAIL);
            admin.setEnabled(true);
            admin.setRole(RoleType.ADMIN);

            userRepository.save(admin);
        };
    }
}
