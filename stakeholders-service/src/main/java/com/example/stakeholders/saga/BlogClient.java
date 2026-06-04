package com.example.stakeholders.saga;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.UUID;

@FeignClient(name = "blog-service", url = "http://api-gateway:8000/api")
public interface BlogClient {

    @PostMapping("/blogs/welcome")
    String createWelcomeBlog(@RequestParam("userId") Long id);

    @DeleteMapping("/blogs/{blogId}")
    void deleteBlog(@PathVariable String blogId);
}
