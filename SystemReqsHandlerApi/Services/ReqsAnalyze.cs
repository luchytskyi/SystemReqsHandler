using System.IO.Compression;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using PlantUml.Net;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Services;
using ReqsHandler.Core.Services.Models;
using SystemReqsHandlerApi.Models;

namespace SystemReqsHandlerApi.Services;

public class ReqsAnalyze(IReqsService reqsService, IAppConfiguration appConfiguration) : IReqsAnalyzer
{
	public IEnumerable<TableDto> GetSchema()
	{
		return ModelToDto(reqsService.GetTables().ToList());
	}

	public string Tokenize(string text)
	{
		var result = new StringBuilder();
		var doc = reqsService.GetDocument(text);

		foreach (var token in doc.Tokens)
		{
			result.AppendLine(
				$"{token.Text} {token.Lemma} {token.PoS} {token.Tag} {token.Dep} {token.Shape} {token.IsAlpha} {token.IsStop}");
		}

		return result.ToString();
	}

	public string BuildUml(string text)
	{
		var lemmas = RetrieveLemmaFromText(text);
		if (lemmas.Count == 0)
		{
			return "Lemmas weren't find in the speech.";
		}

		var tables = reqsService.GetTables();
		var dto = ModelToDto(tables.ToList()).ToList();

		var generator = new PlantUmlBuilder(dto);
		return generator.Build(lemmas);
	}

	public ReqsDiagramResponse GetDiagram(string text) 
	{
		var uml = BuildUml(text);
		var encodedUrl = PlantUmlTextEncoding.EncodeUrl(uml);

		return new ReqsDiagramResponse
		{
			Uml = BuildUml(uml),
			RemoteUrl = CreateRemotePlantUmlUrl(encodedUrl)
		};
	}

	private string CreateRemotePlantUmlUrl(string encodedUrl)
	{
		var remoteServerUrl = appConfiguration.RemoteUmlServerUrl.TrimEnd('/');
		return $"{remoteServerUrl}/{encodedUrl}";
	}

	private List<string> RetrieveLemmaFromText(string text)
	{
		var doc = reqsService.GetDocument(text);
		var lemmas = new List<string>();
		foreach (var token in doc.Tokens)
		{
			if (!token.IsStop && !token.IsPunct) 
			{
				lemmas.Add(token.Lemma);
			}
		}

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