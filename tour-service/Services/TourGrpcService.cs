using Grpc.Core;
using tour_service.Services;
using TourGrpc;

public class TourGrpcService : TourService.TourServiceBase
{
    private readonly TourDomainService _tourService;

    public TourGrpcService(TourDomainService tourService)
    {
        _tourService = tourService;
    }

    public override Task<CreateDraftTourResponse> CreateDraftTour(
        CreateDraftTourRequest request,
        ServerCallContext context)
    {
        var tourId = _tourService.CreateDraftTour(request.UserId);

        return Task.FromResult(new CreateDraftTourResponse
        {
            TourId = tourId.ToString()
        });
    }
}