using NATS.Client;
using System.Text;
using System.Text.Json;
using tour_service.Models;

namespace tour_service.Saga
{
    public class PublishTourOrchestrator
    {
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _commandSubject = "tour.publish.command";
        private readonly string _replySubject = "tour.publish.reply";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public PublishTourOrchestrator(IConfiguration config, IServiceProvider serviceProvider)
        {
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = config["NATS_URL"] ?? "nats://localhost:4222";
            _connection = new ConnectionFactory().CreateConnection(opts);
            _serviceProvider = serviceProvider;

            // Pretplata na odgovore (Replies)
            var subscription = _connection.SubscribeAsync(_replySubject, "tour-service-group");
            subscription.MessageHandler += HandleReply;
            subscription.Start();
        }

        public void Start(Tour tour)
        {
            var command = new PublishTourCommand
            {
                TourId = tour.Id.ToString(),
                AuthorId = tour.AuthorId.ToString(),
                Title = tour.Name,
                Description = tour.Description,
                ImageUrl = tour.KeyPoints.FirstOrDefault()?.ImageUrl ?? "",
                Type = PublishTourCommandType.CreateBlog
            };

            Publish(command);
        }

        private void HandleReply(object sender, MsgHandlerEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.Message.Data);
            var reply = JsonSerializer.Deserialize<PublishTourReply>(json, _jsonOptions);

            if (reply == null)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var tourService = scope.ServiceProvider.GetRequiredService<Services.TourService>();

            if (reply.Type == PublishTourReplyType.BlogCreationFailed)
            {
                // KOMPENZACIJA: Blog nije uspeo, vraćamo turu na Draft
                tourService.RollbackTour(Guid.Parse(reply.TourId));
            }
        }

        private void Publish(PublishTourCommand command)
        {
            var json = JsonSerializer.Serialize(command, _jsonOptions);
            _connection.Publish(_commandSubject, Encoding.UTF8.GetBytes(json));
        }
    }
}