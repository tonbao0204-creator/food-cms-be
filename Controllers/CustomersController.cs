using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.DTOs;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetAllCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var customers = await _context.Customers
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        PhoneNumber = c.PhoneNumber,
                        Address = c.Address,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<CustomerDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách khách hàng thành công",
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<CustomerDto>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new ApiResponse<CustomerDto>
                    {
                        Success = false,
                        Message = "Khách hàng không tồn tại"
                    });
                }

                var dto = new CustomerDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    CreatedAt = customer.CreatedAt
                };

                return Ok(new ApiResponse<CustomerDto>
                {
                    Success = true,
                    Message = "Lấy thông tin khách hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FullName))
                {
                    return BadRequest(new ApiResponse<CustomerDto>
                    {
                        Success = false,
                        Message = "Tên khách hàng không được để trống"
                    });
                }

                var customer = new Customer
                {
                    FullName = request.FullName.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim() ?? "",
                    Address = request.Address?.Trim() ?? "",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var dto = new CustomerDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    CreatedAt = customer.CreatedAt
                };

                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, new ApiResponse<CustomerDto>
                {
                    Success = true,
                    Message = "Tạo khách hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> UpdateCustomer(int id, UpdateCustomerRequest request)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new ApiResponse<CustomerDto>
                    {
                        Success = false,
                        Message = "Khách hàng không tồn tại"
                    });
                }

                customer.FullName = request.FullName?.Trim() ?? customer.FullName;
                customer.PhoneNumber = request.PhoneNumber?.Trim() ?? customer.PhoneNumber;
                customer.Address = request.Address?.Trim() ?? customer.Address;

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();

                var dto = new CustomerDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    CreatedAt = customer.CreatedAt
                };

                return Ok(new ApiResponse<CustomerDto>
                {
                    Success = true,
                    Message = "Cập nhật khách hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Khách hàng không tồn tại"
                    });
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa khách hàng thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }
    }
}
