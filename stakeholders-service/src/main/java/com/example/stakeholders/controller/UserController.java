package com.example.stakeholders.controller;

import java.util.List;

import com.example.stakeholders.dto.AdminUserDto;
import com.example.stakeholders.dto.UserInfo;
import com.example.stakeholders.dto.UpdateUserInfoRequest;
import org.springframework.security.core.Authentication;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import com.example.stakeholders.service.inter.UserService;



@RestController
@RequestMapping(value = "/api", produces = MediaType.APPLICATION_JSON_VALUE)
@CrossOrigin
public class UserController {
    @Autowired
    private UserService userService;

    //@GetMapping("/getMyInfo")
    //@PreAuthorize("hasAnyRole('GUIDE', 'TOURIST')")
    //public UserInfo userInfo(@RequestParam Long userId) {
       // return this.userService.getUserInfo(userId);
    //}

    @GetMapping("/getMyInfo")
    @PreAuthorize("hasAnyRole('GUIDE', 'TOURIST')")
    public UserInfo userInfo(Authentication authentication) {
        String username = authentication.getName();
        return userService.getUserInfo(username);
    }

    @GetMapping("/me")
    @PreAuthorize("hasAnyRole('GUIDE', 'TOURIST')")
    public UserInfo myInfo(Authentication authentication) {
        return userInfo(authentication);
    }

    @PutMapping("/updateMyInfo")
    @PreAuthorize("hasAnyRole('GUIDE', 'TOURIST')")
    public UserInfo updateUserInfo(Authentication authentication, @RequestBody UpdateUserInfoRequest updateUserInfoRequest) {
        String username = authentication.getName();
        return userService.updateUserInfo(username, updateUserInfoRequest);
    }

    @PutMapping("/me")
    @PreAuthorize("hasAnyRole('GUIDE', 'TOURIST')")
    public UserInfo updateMyInfo(Authentication authentication, @RequestBody UpdateUserInfoRequest updateUserInfoRequest) {
        return updateUserInfo(authentication, updateUserInfoRequest);
    }

    @GetMapping("/admin/users")
    @PreAuthorize("hasRole('ADMIN')")
    public List<AdminUserDto> getAllUsers() {
        return userService.getAllUsersForAdmin();
    }

    @PutMapping("/admin/users/{id}/block")
    @PreAuthorize("hasRole('ADMIN')")
    public AdminUserDto blockUser(@PathVariable Long id) {
        return userService.blockUser(id);
    }

}
