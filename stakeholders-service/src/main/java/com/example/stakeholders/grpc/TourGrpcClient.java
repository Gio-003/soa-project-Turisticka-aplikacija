package com.example.stakeholders.grpc;

import net.devh.boot.grpc.client.inject.GrpcClient;
import org.springframework.stereotype.Service;
import tour.TourServiceGrpc;
import tour.Tour;

import java.util.UUID;

@Service
public class TourGrpcClient {

    @GrpcClient("tour-service")
    private TourServiceGrpc.TourServiceBlockingStub tourStub;

    public UUID createDraftTour(Long userId) {

        Tour.CreateDraftTourRequest request =
                Tour.CreateDraftTourRequest.newBuilder()
                        .setUserId(userId)
                        .build();

        Tour.CreateDraftTourResponse response =
                tourStub.createDraftTour(request);

        return UUID.fromString(response.getTourId());
    }

    // =========================
    // DELETE TOUR (ROLLBACK)
    // =========================
    public boolean deleteTour(UUID tourId) {

        Tour.DeleteTourRequest request =
                Tour.DeleteTourRequest.newBuilder()
                        .setTourId(tourId.toString())
                        .build();

        Tour.DeleteTourResponse response =
                tourStub.deleteTour(request);

        return response.getSuccess();
    }
}