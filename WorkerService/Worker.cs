using Services.Consumers;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        public Worker(IMessageConsumer messageConsumer)
        {
            _messageConsumer = messageConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _messageConsumer.Consume();

            return Task.CompletedTask;
        }
    }
}
