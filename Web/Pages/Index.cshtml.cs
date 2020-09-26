using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Web.Services;

namespace Web.Pages
{
public class IndexModel : PageModel
{
    private readonly ICustomerRepositoryAsync _customer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRazorRenderService _renderService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, ICustomerRepositoryAsync customer, IUnitOfWork unitOfWork, IRazorRenderService renderService)
    {
        _logger = logger;
        _customer = customer;
        _unitOfWork = unitOfWork;
        _renderService = renderService;
    }
    public IEnumerable<Customer> Customers { get; set; }
    public void OnGet()
    {
    }
    public async Task<PartialViewResult> OnGetViewAllPartial()
    {
        Customers = await _customer.GetAllAsync();
        return new PartialViewResult
        {
            ViewName = "_ViewAll",
            ViewData = new ViewDataDictionary<IEnumerable<Customer>>(ViewData, Customers)
        };
    }
    public async Task<JsonResult> OnGetCreateOrEditAsync(int id = 0)
    {
        if (id == 0)
            return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", new Customer()) });
        else
        {
            var thisCustomer = await _customer.GetByIdAsync(id);
            return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", thisCustomer) });
        }
    }
    public async Task<JsonResult> OnPostCreateOrEditAsync(int id, Customer customer)
    {
        if (ModelState.IsValid)
        {
            if (id == 0)
            {
                await _customer.AddAsync(customer);
                await _unitOfWork.Commit();
            }
            else
            {
                await _customer.UpdateAsync(customer);
                await _unitOfWork.Commit();
            }
            Customers = await _customer.GetAllAsync();
            var html = await _renderService.ToStringAsync("_ViewAll", Customers);
            return new JsonResult(new { isValid = true, html = html });
        }
        else
        {
            var html = await _renderService.ToStringAsync("_CreateOrEdit", customer);
            return new JsonResult(new { isValid = false, html = html });
        }
    }
    public async Task<JsonResult> OnPostDeleteAsync(int id)
    {
        var customer = await _customer.GetByIdAsync(id);
        await _customer.DeleteAsync(customer);
        await _unitOfWork.Commit();
        Customers = await _customer.GetAllAsync();
        var html = await _renderService.ToStringAsync("_ViewAll", Customers);
        return new JsonResult(new { isValid = true, html = html });
    }
}
}
