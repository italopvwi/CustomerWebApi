using Customer.Tester;
using Serilog;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter(renderMessage: true))
    .CreateLogger();

var customer = new CustomerTester("http://localhost:5133");
Console.WriteLine("Press any key to start processing async");
Console.ReadKey();
await customer.StartAsync();