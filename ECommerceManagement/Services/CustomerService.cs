using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CustomerService
{
    private readonly CustomerRepository _repository;

    public CustomerService(CustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> RegisterCustomerAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FullName))
            throw new ArgumentException("Customer full name is required.");

        if (string.IsNullOrWhiteSpace(customer.Email) || !customer.Email.Contains('@'))
            throw new ArgumentException("A valid email address is required.");

        if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
            throw new ArgumentException("Phone number is required.");

        if (await _repository.EmailExistsAsync(customer.Email))
            throw new InvalidOperationException($"The email '{customer.Email}' is already registered.");

        return await _repository.AddAsync(customer);
    }

    public Task<List<Customer>> GetAllCustomersAsync() => _repository.GetAllAsync();
    public Task<Customer?> GetCustomerByIdAsync(int id) => _repository.GetByIdAsync(id);
}