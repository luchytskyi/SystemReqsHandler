using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Smo;
using SystemReqsHandlerApi.Models;

namespace SystemReqsHandlerApi.Services;

public class PlantUmlBuilder(List<TableDto> tables)
{
	private const string DefinedFunctions = """
		!define primary_key() <color:#b8861b><&key></color>
		!define foreign_key() <color:#aaaaaa><&key></color>
		!define dataType_color(x) <color:#8a8b8e>x</color>
		!define selected_entity(x) <b><color:#094a25>x</color></b>
		""";

	public string Build(IList<string> inputLemmas)
	{
		var includedTables = GetRelativeTablesByLemma(inputLemmas);
		if (includedTables.Count == 0)
		{
			return string.Empty;
		}

		return BuildUml(inputLemmas, includedTables);
	}

	private string BuildUml(IList<string> inputLemmas, HashSet<string> includedTables)
	{
		var diagram = new StringBuilder();
		diagram.AppendLine("@startuml");
		diagram.AppendLine(DefinedFunctions);
		diagram.AppendLine();

		// Definition of tables
		var result = BuildTableEntities(inputLemmas, includedTables);
		if (result.IsNullOrEmpty())
		{
			return string.Empty;
		}

		diagram.Append(result);

		// Definition of relationships between tables
		diagram.Append(BuildDependentTables(inputLemmas, includedTables));

		diagram.AppendLine("@enduml");
		return diagram.ToString();
	}

	private string BuildDependentTables(IList<string> inputLemmas, HashSet<string> includedTables)
	{
		var sb = new StringBuilder();
		foreach (var table in tables)
		{
			if (!includedTables.Contains(table.Name))
			{
				continue;
			}

			var styledTableName = GetEntityStyledName(table.Name, IsSelectedEntity(table.Lemmas, inputLemmas));
			foreach (var fk in table.ForeignKeyDto)
			{
				if (!includedTables.Contains(fk.ReferencedTable))
				{
					continue;
				}

				var tbl = tables.FirstOrDefault(t => t.Name == fk.ReferencedTable);
				var styledReferencedTable = table.Name;
				if (tbl != null)
				{
					styledReferencedTable =
						GetEntityStyledName(fk.ReferencedTable, IsSelectedEntity(tbl.Lemmas, inputLemmas));
				}

				sb.AppendLine($"\"{styledTableName}\" --> \"{styledReferencedTable}\" : {fk.Name}");
			}
		}

		return sb.ToString();
	}

	private string BuildTableEntities(IList<string> inputLemmas, IReadOnlySet<string> includedTables)
	{
		var sb = new StringBuilder();
		foreach (var table in tables)
		{
			if (!includedTables.Contains(table.Name))
			{
				continue;
			}

			var isSelected = IsSelectedEntity(table.Lemmas, inputLemmas);
			sb.AppendLine($"entity \"{GetEntityStyledName(table.Name, isSelected)}\" {{");

			// Definition of columns
			sb.Append(BuildColumnEntities(inputLemmas, table));

			sb.AppendLine("}");
			sb.AppendLine();
		}

		return sb.ToString();
	}

	private string BuildColumnEntities(IList<string> inputLemmas, TableDto table)
	{
		var sb = new StringBuilder();
		foreach (var column in table.Columns)
		{
			var iconKey = "";
			var keyType = "";
			if (column.IsPrimaryKey)
			{
				iconKey = "primary_key() ";
				keyType = " (PK)";
			}

			var isForeignKey = table.ForeignKeyDto.Any(fk => fk.Columns.Any(fkCol => fkCol.Name == column.Name));
			if (isForeignKey)
			{
				iconKey = "foreign_key() ";
				keyType += " (FK)";
			}

			var selected = IsSelectedEntity(column.Lemmas, inputLemmas);
			sb.AppendLine(
				$"{iconKey}{GetEntityStyledName(column.Name, selected)}{keyType}: {GetColumnType(column)}");
		}
		
		return sb.ToString();
	}

	private HashSet<string> GetRelativeTablesByLemma(IList<string> inputLemmas)
	{
		// Визначаємо, які таблиці включати
		var includedTables = new HashSet<string>();
		includedTables.AddRange(GetTables(inputLemmas));

		// Додаємо таблиці, на які посилаються включені таблиці
		includedTables.AddRange(GetTablesByKeyRelations(includedTables));
		return includedTables;
	}

	private IEnumerable<string> GetTablesByKeyRelations(IReadOnlySet<string> includedTables)
	{
		foreach (var table in tables)
		{
			foreach (var fk in table.ForeignKeyDto)
			{
				if (includedTables.Contains(table.Name))
				{
					yield return fk.ReferencedTable;
				}
			}
		}
	}

	private IEnumerable<string> GetTables(IList<string> inputLemmas)
	{
		foreach (var table in tables)
		{
			if (IsSelectedEntity(table.Lemmas, inputLemmas)
			    || table.Columns.Any(col => col.Lemmas.Intersect(inputLemmas).Any()))
			{
				yield return table.Name;
			}
		}
	}

	private static string GetEntityStyledName(string entityName, bool isSelected)
	{
		
		var pattern = isSelected ? "selected_entity({0})" : "{0}";
		return string.Format(pattern, entityName);
	}

	private static bool IsSelectedEntity(IEnumerable<string> entityLemmas, IEnumerable<string> inputLemmas)
	{
		return entityLemmas.Intersect(inputLemmas).Any();
	}

	private static string GetColumnType(ColumnDto column)
	{
		var data = column.SqlDataType switch
		{
			SqlDataType.NVarChar or SqlDataType.VarChar or SqlDataType.VarBinary or SqlDataType.Char =>
				$"{column.DataType}[{column.MaximumLength}]",
			SqlDataType.Decimal or SqlDataType.Numeric =>
				$"{column.DataType}[{column.NumericPrecision}, {column.NumericScale}]",
			SqlDataType.Int
				or SqlDataType.Money
				or SqlDataType.DateTimeOffset
				or SqlDataType.DateTime
				or SqlDataType.Time
				or SqlDataType.SmallInt
				or SqlDataType.TinyInt
				or SqlDataType.BigInt
				or SqlDataType.Bit
				or SqlDataType.Date => column.DataType,
			_ => string.Empty
		};

		return $"dataType_color({(column.Nullable ? $"{data} null" : data)})";
	}
}