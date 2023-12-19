namespace CustomerWebApi.Services;

public class CustomerService
{
    private readonly ILogger<CustomerService> _logger;
    private readonly FileService _fileService;
    private readonly List<Customer.Core.Models.Customer> _customers;

    public CustomerService(ILogger<CustomerService> logger, FileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
        _customers = fileService.ReadCustomers().ToList();
    }

    public (bool success, int[] usedIds) TryToInsert(Customer.Core.Models.Customer[] customers)
    {
        try
        {
            var usedIds = customers
                .Where(
                    x =>
                        _customers
                            .Exists(
                                y =>
                                    y.id == x.id)
                )
                .Select(x => x.id)
                .ToArray();
            if (usedIds.Length > 0)
            {
                return (false, usedIds);
            }

            foreach (var customer in customers)
            {
                var index = _customers
                    .FindIndex(
                        x =>
                            string.CompareOrdinal(x.lastName, customer.lastName) > 0 ||
                            (
                                x.lastName == customer.lastName &&
                                string.CompareOrdinal(x.firstName, customer.firstName) > 0)
                    );

                if (index >= 0)
                {
                    _customers.Insert(index, customer);
                }
                else
                {
                    _customers.Add(customer);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to insert into _customers");
            return (false, customers.Select(x => x.id).ToArray());
        }
        finally
        {
            _ = Task.Run(() => _fileService.WriteCustomers(_customers.ToArray()));
        }

        return (true, null);
    }

    public IEnumerable<Customer.Core.Models.Customer> GetCustomers()
    {
        return _customers;
    }
}