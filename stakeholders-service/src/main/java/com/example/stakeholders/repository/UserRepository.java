package com.example.stakeholders.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import com.example.stakeholders.model.RoleType;
import com.example.stakeholders.model.User;

public interface UserRepository extends JpaRepository<User, Long> {
    User findByUsername(String username);
    long countByRole(RoleType role);
    List<User> findAllByOrderByIdAsc();
}

