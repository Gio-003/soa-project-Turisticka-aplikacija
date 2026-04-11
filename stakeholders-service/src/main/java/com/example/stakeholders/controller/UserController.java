package com.example.stakeholders.controller;

import java.security.Principal;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.example.stakeholders.dto.UserInfo;
import org.springframework.security.core.Authentication;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.web.bind.annotation.*;

import com.example.stakeholders.model.User;
import com.example.stakeholders.service.inter.UserService;



@RestController
@RequestMapping(value = "/api", produces = MediaType.APPLICATION_JSON_VALUE)
@CrossOrigin
public class UserController {


    @Autowired
    private AuthenticationManager authenticationManager;

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

}
