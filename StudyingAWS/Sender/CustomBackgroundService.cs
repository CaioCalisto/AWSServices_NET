using Amazon.SQS;
using Amazon.SQS.Model;

namespace Sender;

public class CustomBackgroundService : BackgroundService
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
        var queueUrl = await CreateQueueIfNotExists();

        while (!stoppingToken.IsCancellationRequested)
        {
            SendMessageResponse responseSendMsg =
                await _amazonSqs.SendMessageAsync(queueUrl, "Hello world!!");
            _logger.LogInformation($"Message sent, status {responseSendMsg.HttpStatusCode.ToString()}");
            
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    private async Task<string> CreateQueueIfNotExists()
    {
        string queueUrl = string.Empty;
        var existentQueues = await GetExistentQueuesAsync();
        if (!existentQueues.QueueUrls.Exists(q => q.Contains(QUEUENAME)))
        {
            queueUrl = await CreateQueueAsync();
            _logger.LogInformation($"Created QUEUE: {queueUrl}");
        }
        else
        {
            await LogQueuesAsync(existentQueues);
            queueUrl = existentQueues.QueueUrls.First();
        }

        return queueUrl;
    }

    private async Task<string> CreateQueueAsync()
    {
        CreateQueueResponse responseCreate = await _amazonSqs.CreateQueueAsync(
            new CreateQueueRequest{QueueName = QUEUENAME});
        return responseCreate.QueueUrl;
    }

    private async Task<ListQueuesResponse> GetExistentQueuesAsync() =>
        await _amazonSqs.ListQueuesAsync("");

    private async Task LogQueuesAsync(ListQueuesResponse queuesResponse)
    {
        foreach (var queueUrl in queuesResponse.QueueUrls)
        {
            var attributes = new List<string>{ QueueAttributeName.All };
            GetQueueAttributesResponse responseGetAtt =
                await _amazonSqs.GetQueueAttributesAsync(queueUrl, attributes);
            
            _logger.LogInformation($"QUEUE: {queueUrl}");
            foreach(var att in responseGetAtt.Attributes)
                Console.WriteLine($"\t{att.Key}: {att.Value}");
        }
    }
}