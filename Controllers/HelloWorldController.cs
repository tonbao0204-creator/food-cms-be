using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HelloWorldController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/HelloWorld
        [HttpGet]
        public async Task<IActionResult> GetHelloWorlds()
        {
            try
            {
                // Thử lấy danh sách từ bảng
                var data = await _context.HelloWorlds.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi (ví dụ sai chuỗi kết nối, sai tên bảng) sẽ báo lỗi rõ ràng
                return StatusCode(500, $"Lỗi kết nối CSDL hoặc truy vấn: {ex.Message}");
            }
        }
    }
}
