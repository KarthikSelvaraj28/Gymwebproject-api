using Gymwebproject.DB;
using Gymwebproject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gymwebproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly Gymdbcontext _context;
        private readonly ILogger<OrdersController> _logger;
        private const string DefaultImageUrl = "https://via.placeholder.com/150"; // ✅ Default Image

        public OrdersController(Gymdbcontext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET: Retrieve User Orders (Grouped by OrderID)
        [HttpGet("List/{userEmail}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserOrders(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest(new { message = "User email is required." });

            var userOrders = await _context.Orders
                .Where(order => order.UserEmail == userEmail)
                .GroupBy(order => order.OrderID)
                .Select(group => new
                {
                    OrderID = group.Key,
                    CustomerName = _context.Userinformation
                        .Where(u => u.Email == userEmail)
                        .Select(u => u.Fullname)
                        .FirstOrDefault(),
                    UserEmail = userEmail,
                    Quantity = group.Sum(order => order.Quantity),
                    TotalPrice = group.Sum(order => order.TotalPrice),
                    OrderDate = group.First().OrderDate,
                    Products = group.Select(order => new
                    {
                        order.Equipments,
                        order.Description,
                        order.Quantity,
                        Price = _context.Equipmentsdb
                            .Where(e => e.Equipments == order.Equipments)
                            .Select(e => e.Price)
                            .FirstOrDefault(), // ✅ Fetch Price from Equipmentsdb
                        order.TotalPrice,
                        Imageurl = !string.IsNullOrEmpty(order.Imageurl) ? order.Imageurl : DefaultImageUrl
                    }).ToList()
                })
                .ToListAsync();

            if (!userOrders.Any())
                return NotFound(new { message = "No orders found for this user." });

            return Ok(userOrders);
        }

        // ✅ POST: Place Order (Multiple Items Under One OrderID)
        [HttpPost("PlaceOrder/{userEmail}")]
        public async Task<IActionResult> PlaceOrder(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest(new { message = "User email is required." });

            var cartItems = await _context.Cart
                .Where(c => c.UserEmail == userEmail)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest(new { message = "Cart is empty. Add items before placing an order." });

            var orderId = Guid.NewGuid(); // ✅ Single OrderID for all items in the cart
            var newOrders = new List<Ordersdb>();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in cartItems)
                    {
                        var equipment = await _context.Equipmentsdb
                            .FirstOrDefaultAsync(e => e.Equipments == item.Equipments);

                        if (equipment == null)
                        {
                            _logger.LogWarning($"Equipment not found: {item.Equipments}");
                            return BadRequest(new { message = $"Equipment not found: {item.Equipments}" });
                        }

                        var order = new Ordersdb
                        {
                            UserEmail = item.UserEmail,
                            OrderID = orderId,
                            Equipments = item.Equipments,
                            Description = item.Description ?? "N/A",
                            Quantity = item.Quantity,
                            TotalPrice = Convert.ToDecimal(equipment.Price * item.Quantity),
                            OrderDate = DateTime.UtcNow,
                            Imageurl = !string.IsNullOrEmpty(equipment.Imageurl) ? equipment.Imageurl : DefaultImageUrl // ✅ Default Image
                        };

                        _context.Orders.Add(order);
                        newOrders.Add(order);
                    }

                    await _context.SaveChangesAsync();

                    // ✅ Clear the cart after placing the order
                    _context.Cart.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { message = "✅ Order placed successfully!", orders = newOrders });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"❌ Error while placing order: {ex.Message}");
                    return StatusCode(500, new { message = "❌ Failed to place order. Please try again later." });
                }
            }
        }

        // ✅ DELETE: Cancel Order by OrderID
        [HttpDelete("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderID == orderId)
                .ToListAsync();

            if (!orders.Any())
                return NotFound(new { message = "Order not found." });

            _context.Orders.RemoveRange(orders);
            await _context.SaveChangesAsync();

            return Ok(new { message = "❌ Order cancelled successfully!" });
        }
    }
}