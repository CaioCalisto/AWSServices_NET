using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Listener;

public class CustomBackgroundService: BackgroundService
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly ILogger<CustomBackgroundService> _logger;
    private const string QUEUENAME = "mycustomqueue";
    
    public CustomBackgroundService(IAmazonSQS amazonSqs, ILogger<CustomBackgroundService> logger)
    {
        _amazonSqs = amazonSqs;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string queueUrl = await GetQueueUrl();
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _amazonSqs.ReceiveMessageAsync(new ReceiveMessageRequest{
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 1
            });

            if (response.HttpStatusCode == HttpStatusCode.OK && response.Messages.Any())
            {
                foreach (var message in response.Messages)
                {
                    _logger.LogInformation($"Received {message.Body}");
                    await _amazonSqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                }
            }
        }
    }

    private async Task<string> GetQueueUrl()
    {
        string queueUrl = string.Empty;
        while (string.IsNullOrEmpty(queueUrl))
        {
            var existentQueues = await GetExistentQueuesAsync();
            if (existentQueues.QueueUrls.Exists(q => q.Contains(QUEUENAME)))
            {
                queueUrl = existentQueues.QueueUrls.First();
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        return queueUrl;
    }
    
    private async Task<ListQueuesResponse> GetExistentQueuesAsync() =>
        await _amazonSqs.ListQueuesAsync("");
}