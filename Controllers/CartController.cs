using Gymwebproject.DB;
using Gymwebproject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gymwebproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly Gymdbcontext obj;

        public CartController(Gymdbcontext context)
        {
            obj = context;
        }

        [HttpGet("List/{userEmail}")]
        public async Task<ActionResult<IEnumerable<object>>> GetCart(string userEmail)
        {
            Console.WriteLine($"Received userEmail: {userEmail}");

            var cartItems = await (from cart in obj.Cart
                                   join eq in obj.Equipmentsdb on cart.ProductID equals eq.ID
                                   where cart.UserEmail == userEmail
                                   select new
                                   {
                                       cart.OrderID,
                                       cart.ProductID,
                                       EquipmentName = eq.Equipments,
                                       eq.Description,
                                       eq.Imageurl,
                                       eq.Price,
                                       cart.Quantity,
                                       cart.TotalPrice,
                                       cart.UserEmail,
                                       cart.OrderDate
                                   }).ToListAsync();

            if (!cartItems.Any())
            {
                Console.WriteLine("No items found for the user.");
                return NotFound("No items in the cart for this user.");
            }

            return Ok(cartItems);
        }



        // ✅ POST: Add Item to Cart (Ensure Unique Entry per User & Product)
        [HttpPost("Add")]
        public async Task<ActionResult<Cart>> AddToCart([FromBody] Cart cartItem)
        {
            if (cartItem == null || cartItem.Quantity <= 0)
            {
                return BadRequest("Invalid cart item or quantity.");
            }

            // ✅ Check if product already exists in cart for the same user
            var existingCartItem = await obj.Cart
                .FirstOrDefaultAsync(c => c.ProductID == cartItem.ProductID && c.UserEmail == cartItem.UserEmail);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItem.Quantity;
                existingCartItem.TotalPrice = existingCartItem.Quantity * cartItem.TotalPrice;
            }
            else
            {
                cartItem.TotalPrice = cartItem.Quantity * cartItem.TotalPrice; // Calculate total price
                obj.Cart.Add(cartItem);
            }

            await obj.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully!", cartItem });
        }

        // ✅ PUT: Update Quantity of an Item
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] Cart updatedItem)
        {
            var cartItem = await obj.Cart.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            cartItem.Quantity = updatedItem.Quantity;
            cartItem.TotalPrice = cartItem.Quantity * updatedItem.TotalPrice; // ✅ Recalculate total price

            await obj.SaveChangesAsync();
            return Ok(new { message = "Cart item updated successfully!", cartItem });
        }

        // ✅ DELETE: Remove Item from Cart
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteCartItem(int id)
        {
            var item = obj.Cart.FirstOrDefault(x => x.OrderID == id);
            if (item == null)
            {
                return NotFound(new { message = "Item not found" });
            }

            obj.Cart.Remove(item);
            obj.SaveChanges(); // ✅ Ensure changes are saved
            return Ok(new { message = "Item deleted successfully" });
        }

    }
}
