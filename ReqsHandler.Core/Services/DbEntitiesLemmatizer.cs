using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Services.Models;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public partial class DbEntitiesLemmatizer(ISpacyInstance spacyInstance, IClientSystemConfig clientConfig)
	: IDbEntitiesLemmatizer
{
	private const string RegexSplitNames = "[А-ЯЁЇІЄҐ][^А-ЯЁЇІЄҐ]*";
	private Lang Nlp => spacyInstance.LangDocument;

	public IEnumerable<ReqsTable> MapTablesLemma(TableCollection? collection)
	{
		if (collection == null)
		{
			return Enumerable.Empty<ReqsTable>();
		}

		var tables = new List<ReqsTable>();
		foreach (Table table in collection)
		{
			var isSplitName = SplitAndLemmatize(table.Name, out var splitList, out var lemmas);
			tables.Add(new ReqsTable
			{
				Name = table.Name,
				Lemmas = lemmas,
				IsSplitName = isSplitName,
				SplitNames = splitList,
				BaseEntity = table,
				Columns = MapColumnLemma(table.Columns)
			});
		}

		return tables;
	}

	private bool SplitIfNeed(string entityName, out IList<string> result)
	{
		var isSplit = false;
		var parts = EntityNameRegex().Matches(CleanUpName(entityName));
		if (parts.Count > 1)
		{
			isSplit = true;
		}

		result = new List<string>();
		foreach (Match part in parts)
		{
			result.Add(part.Value);
		}

		return isSplit;
	}

	private IEnumerable<ReqsColumn> MapColumnLemma(ColumnCollection tableColumns)
	{
		var columns = new List<ReqsColumn>();
		foreach (Column column in tableColumns)
		{
			var isSplitName = SplitAndLemmatize(column.Name, out var splitList, out var lemmas);
			columns.Add(new ReqsColumn
			{
				Name = column.Name,
				Lemmas = lemmas,
				BaseEntity = column,
				IsSplitName = isSplitName,
				SplitNames = splitList
			});
		}

		return columns;
	}

	private bool SplitAndLemmatize(string name, out IList<string> splitList, out IEnumerable<string> lemmas)
	{
		var isSplitName = SplitIfNeed(name, out splitList);
		var doc = Nlp.GetDocument(string.Join(" ", splitList));
		lemmas = doc.Tokens.Where(t => !t.IsPunct && !t.IsStop).Select(t => t.Lemma);
		return isSplitName;
	}

	private string CleanUpName(string name)
	{
		return name.Replace(clientConfig.ColumnIdentifierPrefix, "");
	}

	[GeneratedRegex(RegexSplitNames)]
	private static partial Regex EntityNameRegex();
}