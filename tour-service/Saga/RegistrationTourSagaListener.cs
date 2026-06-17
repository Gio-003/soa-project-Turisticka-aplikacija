using NATS.Client;
using System.Text;
using System.Text.Json;
using tour_service.Services;

namespace tour_service.Saga
{
    public class RegistrationTourSagaListener : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private IAsyncSubscription? _subscription;

        private const string CommandSubject = "registration.tour.command";
        

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RegistrationTourSagaListener(IConfiguration config, IServiceProvider serviceProvider)
        {
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = config["NATS_URL"] ?? "nats://localhost:4222";
            _connection = new ConnectionFactory().CreateConnection(opts);
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = _connection.SubscribeAsync(
                CommandSubject,
                "tour-service-registration-group",
                HandleCommand
            );
            Console.WriteLine("Registration SAGA listener (tour) started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Unsubscribe();
            _connection.Close();
            return Task.CompletedTask;
        }

        private void HandleCommand(object? sender, MsgHandlerEventArgs e)
        {
            Console.WriteLine($"Registration SAGA: received message on {CommandSubject}"); // DODAJ OVO

            var json = Encoding.UTF8.GetString(e.Message.Data);
            Console.WriteLine($"Registration SAGA: payload = {json}"); // DODAJ OVO
            var command = JsonSerializer.Deserialize<RegistrationTourCommand>(json, _jsonOptions);

            if (command == null) return;

            var reply = new RegistrationTourReply { UserId = command.UserId };

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var tourService = scope.ServiceProvider.GetRequiredService<TourDomainService>();

                if (!long.TryParse(command.UserId, out long userId))
                    throw new Exception("Invalid userId: " + command.UserId);

                var tourId = tourService.CreateDraftTour(userId);

                reply.TourId = tourId.ToString();
                reply.Type = RegistrationReplyType.DraftTourCreated;

                Console.WriteLine($"Draft tour {tourId} created for user {command.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create draft tour for user {command.UserId}: {ex.Message}");
                reply.Type = RegistrationReplyType.DraftTourFailed;
            }

            var replyJson = JsonSerializer.Serialize(reply, _jsonOptions);
            if (!string.IsNullOrEmpty(e.Message.Reply))
            {
                _connection.Publish(e.Message.Reply, Encoding.UTF8.GetBytes(replyJson));
            }
        }
    }
}