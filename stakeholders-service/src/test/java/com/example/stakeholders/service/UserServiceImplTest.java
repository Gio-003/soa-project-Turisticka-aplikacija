package com.example.stakeholders.service;

import java.util.List;
import java.util.Optional;

import com.example.stakeholders.dto.AdminUserDto;
import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.RoleType;
import com.example.stakeholders.model.User;
import com.example.stakeholders.repository.UserRepository;
import com.example.stakeholders.service.impl.UserServiceImpl;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.server.ResponseStatusException;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

@ExtendWith(MockitoExtension.class)
class UserServiceImplTest {

    @Mock
    private UserRepository userRepository;

    @Mock
    private PasswordEncoder passwordEncoder;

    @InjectMocks
    private UserServiceImpl userService;

    @Test
    void getAllUsersForAdminReturnsMappedDtos() {
        User admin = createUser(1L, "admin", RoleType.ADMIN, true);
        admin.setPassword("secret");
        User tourist = createUser(2L, "tourist", RoleType.TOURIST, true);
        tourist.setPassword("other-secret");

        when(userRepository.findAllByOrderByIdAsc()).thenReturn(List.of(admin, tourist));

        List<AdminUserDto> users = userService.getAllUsersForAdmin();

        assertEquals(2, users.size());
        assertEquals("admin", users.get(0).getUsername());
        assertEquals(RoleType.TOURIST, users.get(1).getRole());
        assertTrue(users.get(0).isEnabled());
    }

    @Test
    void blockUserDisablesGuideAccount() {
        User guide = createUser(4L, "guide", RoleType.GUIDE, true);

        when(userRepository.findById(4L)).thenReturn(Optional.of(guide));
        when(userRepository.save(any(User.class))).thenAnswer(invocation -> invocation.getArgument(0));

        AdminUserDto blockedUser = userService.blockUser(4L);

        assertFalse(blockedUser.isEnabled());
        assertFalse(guide.isEnabled());
        verify(userRepository).save(guide);
    }

    @Test
    void blockUserRejectsAdminAccount() {
        User admin = createUser(5L, "admin", RoleType.ADMIN, true);

        when(userRepository.findById(5L)).thenReturn(Optional.of(admin));

        assertThrows(ResponseStatusException.class, () -> userService.blockUser(5L));
    }

    @Test
    void saveRejectsAdminSignup() {
        UserRequest request = new UserRequest();
        request.setUsername("new-admin");
        request.setPassword("admin123");
        request.setRole(RoleType.ADMIN);

        assertThrows(ResponseStatusException.class, () -> userService.save(request));
    }

    private User createUser(Long id, String username, RoleType role, boolean enabled) {
        User user = new User();
        user.setId(id);
        user.setUsername(username);
        user.setFirstName(username);
        user.setLastName("User");
        user.setEmail(username + "@example.com");
        user.setRole(role);
        user.setEnabled(enabled);
        return user;
    }
}
