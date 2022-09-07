using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctions.Shared.Middleware.Models
{
	public class OperationResult<T>
	{
		public bool IsSuccess { get; set; }
		public T Payload { get; set; }
		public Exception FailureReason { get; set; }

		public bool IsFailure => !IsSuccess;

		public static OperationResult<T> Success<T>(T result)
		{
			return new OperationResult<T> { IsSuccess = true, Payload = result };
		}

		public static OperationResult<T> Success()
		{
			return new OperationResult<T> { IsSuccess = true, Payload = default };
		}

		public static OperationResult<T> Failure(Exception e)
		{
			return new OperationResult<T> { IsSuccess = false, FailureReason = e };
		}

		public static OperationResult<T> Failure(Exception e, T defaultValue)
		{
			return new OperationResult<T> { IsSuccess = false, FailureReason = e, Payload = defaultValue };
		}
	}
}
