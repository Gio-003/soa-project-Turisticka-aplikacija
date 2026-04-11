package com.example.stakeholders.service.impl;

import java.util.List;

import com.example.stakeholders.dto.UserInfo;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.User;
import com.example.stakeholders.repository.UserRepository;
import com.example.stakeholders.service.inter.UserService;

@Service
public class UserServiceImpl implements UserService {

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Override
    public User findByUsername(String username) throws UsernameNotFoundException {
        return userRepository.findByUsername(username);
    }

    public User findById(Long id) throws AccessDeniedException {
        return userRepository.findById(id).orElseGet(null);
    }

    public List<User> findAll() throws AccessDeniedException {
        return userRepository.findAll();
    }

    @Override
    public User save(UserRequest userRequest) {

        User u = new User();

        u.setUsername(userRequest.getUsername());

        u.setPassword(passwordEncoder.encode(userRequest.getPassword()));

        u.setFirstName(userRequest.getFirstname());
        u.setLastName(userRequest.getLastname());
        u.setEmail(userRequest.getEmail());

        u.setEnabled(true);

        u.setRole(userRequest.getRole());

        return userRepository.save(u);
    }

    @Override
    public UserInfo getUserInfo(String username){
        User user = userRepository.findByUsername(username);
        UserInfo userInfo = new UserInfo();
        userInfo.setFirstName(user.getFirstName());
        userInfo.setLastName(user.getLastName());
        userInfo.setProfilePicture(user.getProfilePicture());
        userInfo.setBiography(user.getBiography());
        userInfo.setMoto(user.getMoto());
        return userInfo;
    }


}
