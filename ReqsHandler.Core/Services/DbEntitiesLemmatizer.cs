using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Smo;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Services.Models;

namespace ReqsHandler.Core.Services;

public class DbEntitiesLemmatizer(ISpacyInstance spacyInstance, ICurrentContext context)
	: IDbEntitiesLemmatizer
{
	private Regex ColumnSplitRegex => new Regex(context.DataSet.ColumnSplitRegex);

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
		result = Array.Empty<string>();
		var parts = ColumnSplitRegex.Matches(CleanUpName(entityName));
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
		splitList = Array.Empty<string>();
		lemmas = Array.Empty<string>();
		if (name.IsNullOrEmpty())
		{
			return false;
		}

		var isSplitName = SplitIfNeed(name, out splitList);
		var doc = spacyInstance.GetDocument(string.Join(" ", splitList));
		lemmas = doc.Tokens.Where(t => !t.IsPunct).Select(t => t.Lemma.ToLower());

		return isSplitName;
	}

	private string CleanUpName(string name)
	{
		return name.Replace(context.DataSet.ColumnIdentifierPrefix, "");
	}
}