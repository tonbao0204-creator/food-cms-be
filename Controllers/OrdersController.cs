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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(o => o.Status == status);
                }

                var orders = await query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer != null ? o.Customer.FullName : "Unknown",
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        OrderDate = o.OrderDate,
                        OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                        {
                            Id = od.Id,
                            FoodId = od.FoodId,
                            FoodName = od.Food != null ? od.Food.Name : "Unknown",
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Total = od.Quantity * od.UnitPrice
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<OrderDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách đơn hàng thành công",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<OrderDto>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new ApiResponse<OrderDto>
                    {
                        Success = false,
                        Message = "Đơn hàng không tồn tại"
                    });
                }

                var dto = new OrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.FullName ?? "Unknown",
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                    {
                        Id = od.Id,
                        FoodId = od.FoodId,
                        FoodName = od.Food?.Name ?? "Unknown",
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Total = od.Quantity * od.UnitPrice
                    }).ToList()
                };

                return Ok(new ApiResponse<OrderDto>
                {
                    Success = true,
                    Message = "Lấy thông tin đơn hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<OrderDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder(CreateOrderRequest request)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null)
                {
                    return NotFound(new ApiResponse<OrderDto>
                    {
                        Success = false,
                        Message = "Khách hàng không tồn tại"
                    });
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    return BadRequest(new ApiResponse<OrderDto>
                    {
                        Success = false,
                        Message = "Đơn hàng phải có ít nhất 1 sản phẩm"
                    });
                }

                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0
                };

                decimal totalAmount = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var item in request.Items)
                {
                    var food = await _context.Foods.FindAsync(item.FoodId);
                    if (food == null)
                    {
                        return NotFound(new ApiResponse<OrderDto>
                        {
                            Success = false,
                            Message = $"Sản phẩm với ID {item.FoodId} không tồn tại"
                        });
                    }

                    if (!food.IsAvailable)
                    {
                        return BadRequest(new ApiResponse<OrderDto>
                        {
                            Success = false,
                            Message = $"Sản phẩm {food.Name} không khả dụng"
                        });
                    }

                    var orderDetail = new OrderDetail
                    {
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        UnitPrice = food.Price
                    };

                    orderDetails.Add(orderDetail);
                    totalAmount += item.Quantity * food.Price;
                }

                order.TotalAmount = totalAmount;
                order.OrderDetails = orderDetails;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var dto = new OrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = customer.FullName,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    OrderDetails = orderDetails.Select(od => new OrderDetailDto
                    {
                        Id = od.Id,
                        FoodId = od.FoodId,
                        FoodName = _context.Foods.Find(od.FoodId)?.Name ?? "Unknown",
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Total = od.Quantity * od.UnitPrice
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, new ApiResponse<OrderDto>
                {
                    Success = true,
                    Message = "Tạo đơn hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<OrderDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, UpdateOrderStatusRequest request)
        {
            try
            {
                var validStatuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };
                if (!validStatuses.Contains(request.Status))
                {
                    return BadRequest(new ApiResponse<OrderDto>
                    {
                        Success = false,
                        Message = $"Trạng thái không hợp lệ. Các trạng thái hợp lệ: {string.Join(", ", validStatuses)}"
                    });
                }

                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new ApiResponse<OrderDto>
                    {
                        Success = false,
                        Message = "Đơn hàng không tồn tại"
                    });
                }

                order.Status = request.Status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                var dto = new OrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.FullName ?? "Unknown",
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                    {
                        Id = od.Id,
                        FoodId = od.FoodId,
                        FoodName = od.Food?.Name ?? "Unknown",
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Total = od.Quantity * od.UnitPrice
                    }).ToList()
                };

                return Ok(new ApiResponse<OrderDto>
                {
                    Success = true,
                    Message = "Cập nhật trạng thái đơn hàng thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<OrderDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Đơn hàng không tồn tại"
                    });
                }

                _context.OrderDetails.RemoveRange(order.OrderDetails);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa đơn hàng thành công"
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

        [HttpGet("statistics/summary")]
        public async Task<ActionResult<ApiResponse<object>>> GetStatisticsSummary()
        {
            try
            {
                var totalOrders = await _context.Orders.CountAsync();
                var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
                var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
                var totalRevenue = await _context.Orders.Where(o => o.Status == "Completed").SumAsync(o => o.TotalAmount);

                var summary = new
                {
                    TotalOrders = totalOrders,
                    PendingOrders = pendingOrders,
                    CompletedOrders = completedOrders,
                    TotalRevenue = totalRevenue
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy thống kê thành công",
                    Data = summary
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
