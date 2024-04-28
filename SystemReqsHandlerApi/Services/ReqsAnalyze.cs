using System.Text;
using Microsoft.SqlServer.Management.Smo;
using PlantUml.Net;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Services;
using ReqsHandler.Core.Services.Models;
using SystemReqsHandlerApi.Models;

namespace SystemReqsHandlerApi.Services;

public class ReqsAnalyze(IReqsService reqsService, IClientSystemConfig systemConfig) : IReqsAnalyzer
{
	public IEnumerable<TableDto> GetSchema()
	{
		return ModelToDto(reqsService.GetDecoratedTables().ToList());
	}

	public string Tokenize(string text)
	{
		var result = new StringBuilder();
		var doc = reqsService.GetDocument(text);
		result.AppendLine($"|{"Text",-5}|{"Lemma",-5}|{"Pos",-5}|{"Tag",-5}");
		foreach (var token in doc.Tokens)
		{
			result.AppendLine(
				$"|{token.Text,-5}|{token.Lemma,-5}|{token.PoS,-5}|{token.Tag,-5}");
		}

		return result.ToString();
	}

	public ReqsDiagramResponse GetDiagram(string text)
	{
		var lemmas = RetrieveLemmaFromText(text, out var fullSet);
		if (lemmas.Count == 0)
		{
			throw new ArgumentException("Lemmas weren't find in the speech.");
		}

		var tables = reqsService.GetDecoratedTables();
		var tableDto = ModelToDto(tables.ToList()).ToList();
		var generator = new PlantUmlBuilder(tableDto);
		var uml = generator.Build(lemmas);
		var encodedUrl = PlantUmlTextEncoding.EncodeUrl(uml);

		return new ReqsDiagramResponse
		{
			Uml = uml,
			Tokens = fullSet,
			DataSetDto = tableDto,
			RemoteUrl = CreateRemotePlantUmlUrl(encodedUrl)
		};
	}

	private string CreateRemotePlantUmlUrl(string encodedUrl)
	{
		var remoteServerUrl = systemConfig.RemoteUmlServerUrl.TrimEnd('/');
		return $"{remoteServerUrl}/{encodedUrl}";
	}

	private List<string> RetrieveLemmaFromText(string text, out string fullSet)
	{
		var doc = reqsService.GetDocument(text);
		var lemmas = new List<string>();
		var fillSetResult = new StringBuilder();
		fillSetResult.AppendLine($"|{"Text",-5}|{"Lemma",-5}|{"Pos",-5}|{"Tag",-5}");
		foreach (var token in doc.Tokens)
		{
			fillSetResult.AppendLine($"|{token.Text,-5}|{token.Lemma,-5}|{token.PoS,-5}|{token.Tag,-5}");
			if (token is { IsStop: false, IsPunct: false }) 
			{
				lemmas.Add(token.Lemma);
			}
		}

		fullSet = fillSetResult.ToString();
		return lemmas;
	}

	private IEnumerable<TableDto> ModelToDto(List<ReqsTable> tables)
	{
		foreach (var table in tables)
		{
			var keys = GetForeignKeys(table.BaseEntity);
			yield return new TableDto
			{
				Name = table.Name,
				Lemmas = table.Lemmas,
				IsSplitName = table.IsSplitName,
				ForeignKeyDto = keys.ToList(),
				SplitNames = table.SplitNames,
				Columns = ColumnToDto(table.Columns.ToList())
			};
		}
	}

	private static IEnumerable<ColumnDto> ColumnToDto(IEnumerable<ReqsColumn> columns)
	{
		return columns.Select(c => new ColumnDto
		{
			Name = c.Name,
			Lemmas = c.Lemmas,
			IsSplitName = c.IsSplitName,
			SplitNames = c.SplitNames,
			IsPrimaryKey = c.BaseEntity.InPrimaryKey,
			DataType = c.BaseEntity.DataType.Name,
			SqlDataType = c.BaseEntity.DataType.SqlDataType,
			MaximumLength = c.BaseEntity.DataType.MaximumLength,
			NumericPrecision = c.BaseEntity.DataType.NumericPrecision,
			NumericScale = c.BaseEntity.DataType.NumericScale,
			Nullable = c.BaseEntity.Nullable
		});
	}

	private IEnumerable<ForeignKeyDto> GetForeignKeys(Table entity)
	{
		foreach (ForeignKey fk in entity.ForeignKeys)
		{
			yield return new ForeignKeyDto()
			{
				Name = fk.Name,
				ReferencedTable = fk.ReferencedTable,
				ReferencedTableSchema = fk.ReferencedTableSchema,
				Columns = fk.Columns.Cast<ForeignKeyColumn>().Select(column => new ForeignKeyColumnDto
				{
					Name = column.Name,
					ReferencedColumn = fk.ReferencedTable
				})
			};
		}
	}
}