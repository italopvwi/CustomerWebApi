using System.Globalization;
using System.Text;
using CsvHelper;

namespace CustomerWebApi.Services;

public class FileService
{
    private readonly ILogger<FileService> _logger;
    private static readonly object _locker = new ();

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public bool WriteCustomers(IEnumerable<Customer.Core.Models.Customer> customers)
    {
        try
        {
            lock (_locker)
            {
                using var fs = new FileStream("customers.csv", FileMode.Create);
                using var sw = new StreamWriter(fs, Encoding.UTF8);
                using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
                cw.WriteRecords(customers);


                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to read csv of customers.");
            return false;
        }
    }

    public IEnumerable<Customer.Core.Models.Customer> ReadCustomers()
    {
        lock (_locker)
        {
            using var fs = new FileStream("customers.csv", FileMode.OpenOrCreate);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
            while (cr.Read())
            {
                yield return cr.GetRecord<Customer.Core.Models.Customer>();
            }
        }
    }
}