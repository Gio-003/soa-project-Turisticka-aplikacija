package com.example.stakeholders.repository;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.stakeholders.model.User;

public interface UserRepository extends JpaRepository<User, Long> {
    User findByUsername(String username);
}

