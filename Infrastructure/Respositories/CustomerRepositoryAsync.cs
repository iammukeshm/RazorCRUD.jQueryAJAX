using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Respositories
{
    public class CustomerRepositoryAsync : GenericRepositoryAsync<Customer>, ICustomerRepositoryAsync
    {
        private readonly DbSet<Customer> _customer;

        public CustomerRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _customer = dbContext.Set<Customer>();
        }
    }
}
