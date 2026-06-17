package com.example.stakeholders.saga;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import java.util.UUID;

@FeignClient(name = "tour-service", url = "http://localhost:55816/api")
public interface TourClient {

    @PostMapping("/tours/draft/{userId}")
    UUID createDraftTour(@PathVariable Long userId);

    @DeleteMapping("/tours/{tourId}")
    void deleteTour(@PathVariable UUID tourId);
}
