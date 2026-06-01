package com.example.stakeholders.service.impl;

import com.example.stakeholders.dto.UserRequest;
import com.example.stakeholders.model.User;
import com.example.stakeholders.saga.BlogClient;
import com.example.stakeholders.saga.TourClient;
import com.example.stakeholders.service.inter.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.UUID;

import static com.example.stakeholders.model.RoleType.GUIDE;

@Service
public class RegistrationSagaService {

    @Autowired
    private UserService userService;

    @Autowired
    private BlogClient blogClient;

    @Autowired
    private TourClient tourClient;

    public User register(UserRequest request) {

        User user = userService.save(request);

        if(user.getRole() != GUIDE){
            return user;
        }

        String blog = null;
        UUID tour = null;


        try {
            blog = blogClient.createWelcomeBlog(user.getId());
            tour = tourClient.createDraftTour(user.getId());

            return user;

        } catch (Exception e) {


            if (tour != null) {
                tourClient.deleteTour(tour);
            }

            if (blog != null) {
                blogClient.deleteBlog(blog);
            }

            userService.deleteById(user.getId());

            throw new RuntimeException("Saga failed: rollback executed");
        }
    }
}
