namespace QueueConsumer.Extensions
{
	public static class GenericExtensions
	{
		public static string SerializeToJson<T>(this T item)
		{
			if (typeof(T) == typeof(string)) return item.ToString();

			return System.Text.Json.JsonSerializer.Serialize(item);
		}
	}
}
