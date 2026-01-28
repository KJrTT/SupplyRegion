using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using SupplyRegion.Model;

namespace SupplyRegion.Services;

public class CsvExportService
{
    public async Task ExportToCsvAsync(List<PurchaseRequest> requests, string filePath)
    {
        System.Text.Encoding encoding;
        try
        {
            encoding = System.Text.Encoding.GetEncoding(1251);
        }
        catch
        {
            encoding = new System.Text.UTF8Encoding(true);
        }

        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            Delimiter = ";",
            Encoding = encoding
        };

        using var writer = new StreamWriter(filePath, false, encoding);
        using var csv = new CsvWriter(writer, config);

        csv.WriteField("Номер");
        csv.WriteField("Дата создания");
        csv.WriteField("Инициатор");
        csv.WriteField("Подразделение");
        csv.WriteField("Наименование товара");
        csv.WriteField("Количество");
        csv.WriteField("Ориентировочная цена");
        csv.WriteField("Статус");
        csv.WriteField("Общая стоимость");
        csv.NextRecord();

        foreach (var request in requests)
        {
            csv.WriteField(request.Id);
            csv.WriteField(request.CreatedDate.ToString("dd.MM.yyyy HH:mm"));
            csv.WriteField(request.Initiator);
            csv.WriteField(request.Department);
            csv.WriteField(request.ProductName);
            csv.WriteField(request.Quantity);
            csv.WriteField(request.EstimatedPrice.ToString("N2"));
            csv.WriteField(request.Status);
            csv.WriteField(request.TotalPrice.ToString("N2"));
            csv.NextRecord();
        }

        await Task.CompletedTask;
    }
}
