namespace SystemReqsHandlerApi.Services;

public static class HashSetExtensions
{
	public static bool AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
	{
		return items.Aggregate(true, (current, item) => current & source.Add(item));
	}
}