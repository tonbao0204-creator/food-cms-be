using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.DTOs;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreFrontController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StoreFrontController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StoreFront/Menu
        [HttpGet("Menu")]
        public async Task<ActionResult<IEnumerable<Food>>> GetMenu()
        {
            // Chỉ trả về các món ăn đang được bán
            return await _context.Foods
                .Where(f => f.IsAvailable)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // POST: api/StoreFront/Checkout
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { message = "Giỏ hàng trống!" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tìm hoặc tạo Khách hàng dựa vào Số điện thoại
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.PhoneNumber == request.PhoneNumber);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = request.CustomerName,
                        PhoneNumber = request.PhoneNumber,
                        Address = request.Address
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync(); // Lưu để lấy Id
                }
                else
                {
                    // Cập nhật lại tên và địa chỉ nếu có thay đổi
                    customer.FullName = request.CustomerName;
                    customer.Address = request.Address;
                    _context.Customers.Update(customer);
                }

                // 2. Khởi tạo Đơn hàng (Order)
                var order = new Order
                {
                    CustomerId = customer.Id,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu để lấy Id

                // 3. Xử lý chi tiết giỏ hàng và tính tiền
                decimal totalAmount = 0;
                foreach (var itemReq in request.Items)
                {
                    var food = await _context.Foods.FindAsync(itemReq.FoodId);
                    if (food == null || !food.IsAvailable)
                    {
                        return BadRequest(new { message = $"Món ăn với ID {itemReq.FoodId} không tồn tại hoặc đã hết hàng." });
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        FoodId = food.Id,
                        Quantity = itemReq.Quantity,
                        UnitPrice = food.Price // Lấy giá CHÍNH THỨC từ Database
                    };
                    _context.OrderDetails.Add(orderDetail);

                    totalAmount += orderDetail.Quantity * orderDetail.UnitPrice;
                }

                // Cập nhật lại tổng tiền cho đơn hàng
                order.TotalAmount = totalAmount;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Đặt hàng thành công!",
                    orderId = order.Id,
                    totalAmount = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi hệ thống khi xử lý đơn hàng", details = ex.Message });
            }
        }

        // GET: api/StoreFront/Order/5
        [HttpGet("Order/{id}")]
        public async Task<IActionResult> TrackOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Food)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng này." });
            }

            return Ok(new
            {
                OrderId = order.Id,
                CustomerName = order.Customer?.FullName,
                Status = order.Status,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderDetails.Select(od => new
                {
                    FoodName = od.Food?.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.Quantity * od.UnitPrice
                })
            });
        }
    }
}
