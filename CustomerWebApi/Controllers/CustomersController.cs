using CustomerWebApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Customer.Core.Models;

namespace CustomerWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly CustomerService _customerService;
        private readonly IValidator<Customer.Core.Models.Customer> _validator;

        public CustomersController(
            ILogger<CustomersController> logger, 
            CustomerService customerService, 
            IValidator<Customer.Core.Models.Customer> validator)
        {
            _logger = logger;
            _customerService = customerService;
            _validator = validator;
        }

        [HttpPost]
        public IActionResult Customers([FromBody] Customer.Core.Models.Customer[] customersRequest)
        {
            if (customersRequest == null || customersRequest.Length < 1)
            {
                return BadRequest(new
                {
                    Error = "CustomerRequest must have at least one"
                });
            }

            var validations = customersRequest
                .Select(
                    x => 
                        _validator
                            .Validate(x))
                .ToArray();
            
            if (validations.Any(x => !x.IsValid))
            {
                return BadRequest(
                    new
                    {
                        Error = "Some customers are not valid.",
                        Errors = validations.SelectMany(x => x.Errors)
                    });
            }

            var (success, usedIds) = _customerService.TryToInsert(customersRequest);
            
            if (success)
            {
                return Created();
            }

            return BadRequest(new
            {
                Error = "Not possible to insert customers with the provided ids",
                UsedIds = usedIds
            });
        }

        [HttpGet]
        public IActionResult Customers()
        {
            return Ok(_customerService.GetCustomers());
        }
    }
}
