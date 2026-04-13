package com.example.stakeholders.service.inter;

import java.util.List;

import com.example.stakeholders.dto.AdminUserDto;
import com.example.stakeholders.dto.UpdateUserInfoRequest;
import com.example.stakeholders.dto.UserInfo;
import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.User;

public interface UserService {
    User findById(Long id);
    User findByUsername(String username);
    List<User> findAll ();
    User save(UserRequest userRequest);
    UserInfo getUserInfo(String username);
    UserInfo updateUserInfo(String username, UpdateUserInfoRequest updateUserInfoRequest);
    List<AdminUserDto> getAllUsersForAdmin();
    AdminUserDto blockUser(Long userId);
}
