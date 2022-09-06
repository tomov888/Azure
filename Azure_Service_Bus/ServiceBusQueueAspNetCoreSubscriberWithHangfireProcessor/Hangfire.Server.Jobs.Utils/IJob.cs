namespace Hangfire.Server.Jobs.Utils
{
	public interface IJob
	{
		string Execute(params object[] args);

		Task<string> ExecuteAsync(params object[] args);
	}
}
