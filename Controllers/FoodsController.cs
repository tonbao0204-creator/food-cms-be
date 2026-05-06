using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.DTOs;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FoodDto>>>> GetAllFoods([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? available = null)
        {
            try
            {
                var query = _context.Foods.AsQueryable();

                if (available.HasValue)
                {
                    query = query.Where(f => f.IsAvailable == available.Value);
                }

                var foods = await query
                    .OrderByDescending(f => f.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => new FoodDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Description = f.Description,
                        Price = f.Price,
                        IsAvailable = f.IsAvailable,
                        CreatedAt = f.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<FoodDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = foods
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<FoodDto>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FoodDto>>> GetFoodById(int id)
        {
            try
            {
                var food = await _context.Foods.FindAsync(id);
                if (food == null)
                {
                    return NotFound(new ApiResponse<FoodDto>
                    {
                        Success = false,
                        Message = "Sản phẩm không tồn tại"
                    });
                }

                var dto = new FoodDto
                {
                    Id = food.Id,
                    Name = food.Name,
                    Description = food.Description,
                    Price = food.Price,
                    IsAvailable = food.IsAvailable,
                    CreatedAt = food.CreatedAt
                };

                return Ok(new ApiResponse<FoodDto>
                {
                    Success = true,
                    Message = "Lấy thông tin sản phẩm thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FoodDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<FoodDto>>> CreateFood(CreateFoodRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new ApiResponse<FoodDto>
                    {
                        Success = false,
                        Message = "Tên sản phẩm không được để trống"
                    });
                }

                if (request.Price < 0)
                {
                    return BadRequest(new ApiResponse<FoodDto>
                    {
                        Success = false,
                        Message = "Giá sản phẩm phải lớn hơn 0"
                    });
                }

                var food = new Food
                {
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim() ?? "",
                    Price = request.Price,
                    IsAvailable = request.IsAvailable,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Foods.Add(food);
                await _context.SaveChangesAsync();

                var dto = new FoodDto
                {
                    Id = food.Id,
                    Name = food.Name,
                    Description = food.Description,
                    Price = food.Price,
                    IsAvailable = food.IsAvailable,
                    CreatedAt = food.CreatedAt
                };

                return CreatedAtAction(nameof(GetFoodById), new { id = food.Id }, new ApiResponse<FoodDto>
                {
                    Success = true,
                    Message = "Tạo sản phẩm thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FoodDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<FoodDto>>> UpdateFood(int id, UpdateFoodRequest request)
        {
            try
            {
                var food = await _context.Foods.FindAsync(id);
                if (food == null)
                {
                    return NotFound(new ApiResponse<FoodDto>
                    {
                        Success = false,
                        Message = "Sản phẩm không tồn tại"
                    });
                }

                food.Name = request.Name?.Trim() ?? food.Name;
                food.Description = request.Description?.Trim() ?? food.Description;
                food.Price = request.Price;
                food.IsAvailable = request.IsAvailable;

                _context.Foods.Update(food);
                await _context.SaveChangesAsync();

                var dto = new FoodDto
                {
                    Id = food.Id,
                    Name = food.Name,
                    Description = food.Description,
                    Price = food.Price,
                    IsAvailable = food.IsAvailable,
                    CreatedAt = food.CreatedAt
                };

                return Ok(new ApiResponse<FoodDto>
                {
                    Success = true,
                    Message = "Cập nhật sản phẩm thành công",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FoodDto>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFood(int id)
        {
            try
            {
                var food = await _context.Foods.FindAsync(id);
                if (food == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Sản phẩm không tồn tại"
                    });
                }

                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa sản phẩm thành công"
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
