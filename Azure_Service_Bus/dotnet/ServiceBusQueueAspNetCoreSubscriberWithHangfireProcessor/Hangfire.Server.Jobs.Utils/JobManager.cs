using Hangfire;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Hangfire.Server.Jobs.Utils
{
	public class JobManager
	{
		private readonly ILogger<JobManager> _logger;

		public JobManager(ILogger<JobManager> logger)
		{
			_logger = logger;
		}


		public string EnqueueJob<T>(Expression<Action<T>> callback) where T : class
		{
			var jobId = BackgroundJob.Enqueue(callback);
			_logger.LogWarning($"Job of type {typeof(T).Name} with id : {jobId} is enqueued.");
			return jobId;
		}
	}
}
