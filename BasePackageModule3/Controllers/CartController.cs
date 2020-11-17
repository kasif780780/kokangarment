using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using BasePackageModule3.Utility;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BasePackageModule3.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailsender;

        [BindProperty]
        public OrderDetailsCart deatilsCart { get; set; }
        public CartController( ApplicationDbContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailsender = emailSender;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            deatilsCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            deatilsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _context.ShoppingCarts.Where(c => c.ApplicationUserId == claims.Value);
            if(cart != null)
            {
                deatilsCart.listCart = cart.ToList();
            }

            foreach(var list in deatilsCart.listCart)
            {
                list.Item = await _context.Items.FirstOrDefaultAsync(m => m.Id == list.ItemId);
                deatilsCart.OrderHeader.OrderTotal = deatilsCart.OrderHeader.OrderTotal + (list.Item.Price * list.Count);
                list.Item.Description = SD.ConvertToRawHtml(list.Item.Description);
                if(list.Item.Description.Length>100)
                {
                    list.Item.Description = list.Item.Description.Substring(0, 99) + "...";
                }
            }
            deatilsCart.OrderHeader.orderToatlOriginal = deatilsCart.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode)!=null)
            {
                deatilsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _context.Coupons.Where(c => c.Name.ToLower() == deatilsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                deatilsCart.OrderHeader.OrderTotal = SD.discountedPrice(couponFromDb, deatilsCart.OrderHeader.orderToatlOriginal);
            }
            return View(deatilsCart);


        }

        public IActionResult AddCoupon()
        {
            if(deatilsCart.OrderHeader.CouponCode ==null)
            {
                deatilsCart.OrderHeader.CouponCode = "";
            }

            HttpContext.Session.SetString(SD.ssCouponCode, deatilsCart.OrderHeader.CouponCode);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
          

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> plus(int cartId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> minus(int cartId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            if(cart.Count ==1)
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();

                var cnt = _context.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> remove(int cartId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            _context.ShoppingCarts.Remove(cart);
            await _context.SaveChangesAsync();

            var cnt = _context.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);


          
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            deatilsCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            deatilsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _context.ApplicationUser.Where(c => c.Id == claims.Value).FirstOrDefaultAsync();

            var cart = _context.ShoppingCarts.Where(c => c.ApplicationUserId == claims.Value);
            if (cart != null)
            {
                deatilsCart.listCart = cart.ToList();
            }

            foreach (var list in deatilsCart.listCart)
            {
                list.Item = await _context.Items.FirstOrDefaultAsync(m => m.Id == list.ItemId);
                deatilsCart.OrderHeader.OrderTotal = deatilsCart.OrderHeader.OrderTotal + (list.Item.Price * list.Count);
               
            }
            deatilsCart.OrderHeader.orderToatlOriginal = deatilsCart.OrderHeader.OrderTotal;
            deatilsCart.OrderHeader.Pickupname = applicationUser.Name;
            deatilsCart.OrderHeader.PickUpPhone = applicationUser.PhoneNumber;
            deatilsCart.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                deatilsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _context.Coupons.Where(c => c.Name.ToLower() == deatilsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                deatilsCart.OrderHeader.OrderTotal = SD.discountedPrice(couponFromDb, deatilsCart.OrderHeader.orderToatlOriginal);
            }
            return View(deatilsCart);


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult>  SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            deatilsCart.listCart = await _context.ShoppingCarts.Where(c => c.ApplicationUserId == claims.Value).ToListAsync();
          
            deatilsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            deatilsCart.OrderHeader.OrderDate = DateTime.Now;
            deatilsCart.OrderHeader.UserId = claims.Value;
            deatilsCart.OrderHeader.status = SD.PaymentStatusPending;
           // deatilsCart.OrderHeader.PickUpTime = Convert.ToDateTime(deatilsCart.OrderHeader.PickUpDate.ToString() + " " + deatilsCart.OrderHeader.PickUpTime.ToShortTimeString());
            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _context.OrderHeaders.Add(deatilsCart.OrderHeader);
            await _context.SaveChangesAsync();

            deatilsCart.OrderHeader.orderToatlOriginal = 0;
            


         

            foreach (var list in deatilsCart.listCart)
            {
                list.Item = await _context.Items.FirstOrDefaultAsync(m => m.Id == list.ItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    ItemId = list.ItemId,
                    OrderId = deatilsCart.OrderHeader.Id,
                    Description = list.Item.Description,
                    Name = list.Item.Name,
                    Price = list.Item.Price,
                    Count = list.Count

                };
                deatilsCart.OrderHeader.orderToatlOriginal += orderDetails.Count * orderDetails.Price;
                _context.OrderDetails.Add(orderDetails);
            }
          

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                deatilsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _context.Coupons.Where(c => c.Name.ToLower() == deatilsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                deatilsCart.OrderHeader.OrderTotal = SD.discountedPrice(couponFromDb, deatilsCart.OrderHeader.orderToatlOriginal);
            }
            else
            {
                deatilsCart.OrderHeader.OrderTotal = deatilsCart.OrderHeader.orderToatlOriginal; 
            }
            deatilsCart.OrderHeader.CouponCodeDiscount = deatilsCart.OrderHeader.orderToatlOriginal - deatilsCart.OrderHeader.OrderTotal;
          
            _context.ShoppingCarts.RemoveRange(deatilsCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _context.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(deatilsCart.OrderHeader.OrderTotal*100),
                Currency = "INR",
                Description = "Order ID : " + deatilsCart.OrderHeader.Id,
            
                Source = stripeToken

            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
            if(charge.BalanceTransactionId ==null)
            {
                deatilsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                deatilsCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }
            if(charge.Status.ToLower() == "succeeded")
            {

                await _emailsender.SendEmailAsync(_context.Users.Where(u => u.Id == claims.Value).FirstOrDefault().Email, "Base-Package - Order Created" + deatilsCart.OrderHeader.Id.ToString(), "Order has been successfully");

                deatilsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                deatilsCart.OrderHeader.status = SD.StatusSubmitted;
            }
            else
            {
                deatilsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            await _context.SaveChangesAsync();
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Confirm", "Order", new { id = deatilsCart.OrderHeader.Id });


        }

            
    }
}