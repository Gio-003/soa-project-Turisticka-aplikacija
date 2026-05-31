using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using tour_service.DTO;
using tour_service.Models;
using tour_service.Services;

namespace tour_service.Controllers;

[ApiController]
[Route("rpc")]
public class RpcController : ControllerBase
{
    private readonly TourService _tourService;

    public RpcController(TourService tourService)
    {
        _tourService = tourService;
    }

    [HttpPost]
    public IActionResult Handle([FromBody] JsonElement body)
    {
        var request = JsonSerializer.Deserialize<JsonRpcRequest>(body);

        var response = new JsonRpcResponse
        {
            jsonrpc = "2.0",
            id = request.id
        };

        try
        {
            switch (request.method)
            {
                case "GetAllTours":
                    response.result = _tourService.GetAllTours();
                    break;

                case "GetMyTours":
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        request.@params.ToString()
                    );

                    var authorId = int.Parse(dict["authorId"]);

                    response.result = _tourService.GetToursByAuthor(authorId);
                    break;

                case "CreateTour":
                    var createTour = JsonSerializer.Deserialize<CreateTourRequest>(
                        request.@params.ToString()
                    );

                    response.result = _tourService.CreateTour(createTour);
                    break;

                case "PublishTour":
                    var publishDict = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        request.@params.ToString()
                    );

                    response.result = _tourService.PublishTour(
                        Guid.Parse(publishDict["tourId"])
                    );
                    break;

                case "ArchiveTour":
                    var archiveDict = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        request.@params.ToString()
                    );

                    response.result = _tourService.ArchiveTour(
                        Guid.Parse(archiveDict["tourId"])
                    );
                    break;

                default:
                    response.error = new
                    {
                        code = -32601,
                        message = "Method not found"
                    };
                    break;
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.error = new
            {
                code = -32603,
                message = ex.Message
            };

            return Ok(response);
        }
    }
}
