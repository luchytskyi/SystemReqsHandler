namespace ReqsHandler.Core.Services.Models;

public abstract class ReqsEntityBase
{
	public string Name { get; init; } = string.Empty;

	public IEnumerable<string> Lemmas { get; init; } = Enumerable.Empty<string>();

	public bool IsSplitName { get; set; }

	public IEnumerable<string> SplitNames { get; set; } = Enumerable.Empty<string>();
}