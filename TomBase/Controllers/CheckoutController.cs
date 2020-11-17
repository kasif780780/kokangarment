using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
using BasePackageModule2.Helpers;
using BasePackageModule2.Models;
using InstaSharp;
using InstaSharp.Exceptions;
using InstaSharp.Interface;
using InstaSharp.Model;
using InstaSharp.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule1.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IInstaMojoConfiguration _instaMojo;
        

        public CheckoutController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IInstaMojoConfiguration instaMojo)
        {
            _userManager = userManager;
            _context = context;
            _instaMojo = instaMojo;
        }


        
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var addresses = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();

            ViewData["Addresses"] = addresses;

            return View();
        }


        public async Task<IActionResult> Address(Address address)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index),address);
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            address.UserId = user.Id;
            _context.Add(address);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PaymentOptions), new { addressId = address.Id });
        }
        //public async Task<IActionResult> PaymentOld(int address)
        //{
        //    var add = await _context.Addresses.FirstOrDefaultAsync(i => i.Id == address);

        //    if (add == null)
        //    {
        //        return BadRequest();
        //    }
            
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);


        //    if (! await _context.Carts.AnyAsync(u => u.UserId == user.Id))
        //    {
        //        return RedirectToAction("Index", "Cart");
        //    }

        //    string instaEndpoint,
        //        instaAuthEndpoint;

            
        //    if (_instaMojo.ClientId.Contains("test") || _instaMojo.ClientSecret.Contains("test"))
        //    {
        //        instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT;
        //        instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
        //    }

        //    else
        //    {
        //        instaEndpoint = "https://api.instamojo.com/v2/";
        //        instaAuthEndpoint = "https://www.instamojo.com/oauth2/token/";
        //    }

        //    IInstamojo objClass;
        //    try
        //    {
        //        objClass = InstamojoImplementation.getApi(
        //            _instaMojo.ClientId,
        //            _instaMojo.ClientSecret,
        //            instaEndpoint,
        //            instaAuthEndpoint);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Something went wrong, Please try again after sometime");
        //    }


        //    double subtotal = 0;

        //    foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
        //    {
        //        if (item.Product.Price != null) subtotal += (double) item.Product.Price * item.Qty;
        //    }

        //    try
        //    {
        //        var transactionId = Guid.NewGuid().ToString();

        //        # region   1. Create Payment Order
        //        //  Create Payment Order
        //        var objPaymentRequest = new PaymentOrder
        //        {
        //            name = $"{user.FirstName} {user.LastName}",
        //            email = user.Email,
        //            phone = add.MobileNumber,
        //            amount = subtotal,
        //            currency = "INR",
        //            description = "",
        //            webhook_url = "https://shopforuniforms.com/Checkout/WebHook",
        //            //webhook_url =   Url.Action("WebHook", "Checkout", null, protocol: "https"),
        //            transaction_id = transactionId,
        //            redirect_url = Url.Action("ProcessPayment","Checkout",null, protocol: "https")
        //        };
        //        //Required POST parameters
        //        //Extra POST parameters 

        //        if (objPaymentRequest.validate())
        //        {

        //            if (objPaymentRequest.nameInvalid)
        //            {
        //                Console.Write("Name is not valid");
        //            }

        //            if (objPaymentRequest.redirectUrlInvalid)
        //            {
        //                Console.Write("Redirect Url is not valid");
        //            }

        //        }
        //        else
        //        {
        //            try
        //            {
        //                var order = new BasePackageModule2.Models.Order
        //                {
        //                    UserId = user.Id,
        //                    AddressId = add.Id,
        //                    PaymentStatus = "Pending",
        //                    TransactionId = transactionId,
        //                    Amount = subtotal,
        //                };
        //                _context.Add(order);
        //                await _context.SaveChangesAsync();

        //                foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
        //                {
        //                    OrderProduct orderProduct = new OrderProduct
        //                    {
        //                        ProductId = cart.Product.Id,
        //                        OrderId = order.Id,
        //                        Quantity = cart.Qty
        //                    };
        //                    _context.Add(orderProduct);
        //                }

        //                await _context.SaveChangesAsync();

        //                CreatePaymentOrderResponse objPaymentResponse = objClass.CreateNewPaymentRequest(objPaymentRequest);
        //                return Redirect(objPaymentResponse.payment_options.payment_url);
                        
        //            }

        //            catch (ArgumentNullException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (WebException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (IOException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (InvalidPaymentOrderException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (ConnectionException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (BaseException ex)
        //            {
        //                throw ex;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //        #endregion

        //    }
        //    catch (BaseException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return BadRequest();
        //}

        [HttpPost]
        [AllowAnonymous]
        public async void WebHook(
            string? payment_id,
            string? status
            )
        {
            var order = await _context.Orders.Where(o => o.PaymentId == payment_id).FirstOrDefaultAsync();

            if (order == null) return;
            order.PaymentId = payment_id;
            order.PaymentStatus = status;
            _context.Update(order);
            await _context.SaveChangesAsync();
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public async Task<IActionResult> ProcessPayment(
            string? payment_id, 
            string? payment_status, 
            string? id,
            string? transaction_id)
        {
            if (transaction_id == null)
            {
                return View("PaymentFailed").WithError("Payment Failed", null);
            }

            if (payment_status == "Failed")
            {
                BasePackageModule2.Models.Order order = await _context.Orders.Where(o => o.TransactionId == transaction_id).FirstOrDefaultAsync();

                if (order == null)
                {
                    return BadRequest();
                }

                order.PaymentId = payment_id;
                order.PaymentStatus = payment_status;

                _context.Update(order);
                await _context.SaveChangesAsync();

                return View("PaymentFailed").WithError("Payment Failed", null);
            }

            string Insta_Endpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT,
            Insta_Auth_Endpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;

            if (_instaMojo.ClientId.Contains("test") || _instaMojo.ClientSecret.Contains("test"))
            {
                Insta_Endpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT;
                Insta_Auth_Endpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
            }

            else
            {
                Insta_Endpoint = "https://api.instamojo.com/v2/";
                Insta_Auth_Endpoint = "https://www.instamojo.com/oauth2/token/";
            }



            IInstamojo objClass = InstamojoImplementation.getApi(_instaMojo.ClientId, _instaMojo.ClientSecret, Insta_Endpoint, Insta_Auth_Endpoint);

            PaymentOrderDetailsResponse objPaymentRequestDetailsResponse = objClass.GetPaymentOrderDetailsByTransactionId(transaction_id);
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
                _context.RemoveRange(carts);
                await _context.SaveChangesAsync();

                if (objPaymentRequestDetailsResponse != null) 
                {
                    BasePackageModule2.Models.Order order = await _context.Orders.Where(o => o.TransactionId == transaction_id).FirstOrDefaultAsync();

                    if (order == null)
                    {
                        return BadRequest();
                    }

                    order.PaymentId = payment_id;
                    order.PaymentStatus = payment_status;

                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            
            return RedirectToAction(nameof(ThankYou)).WithSuccess("Your Order has been placed.", null);
        }

        public async Task<IActionResult> CheckOutOptions()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;

            return View();

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutOptions(string PaymentMode)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;


            if (PaymentMode == null)
            {
                ModelState.AddModelError("PaymentMode", "Please Select Payment Mode.");
                return View();
            }

           

            if (PaymentMode == "payOnline")
            {

                if (!await _context.Carts.AnyAsync(u => u.UserId == user.Id))
                {
                    return RedirectToAction("Index", "Cart");
                }

                string instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT, instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;


                if (_instaMojo.ClientId.Contains("test") || _instaMojo.ClientSecret.Contains("test"))
                {
                    instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT;
                    instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
                }

                else
                {
                    instaEndpoint = "https://api.instamojo.com/v2/";
                    instaAuthEndpoint = "https://www.instamojo.com/oauth2/token/";
                }

                IInstamojo objClass;
                try
                {
                    objClass = InstamojoImplementation.getApi(
                        _instaMojo.ClientId,
                        _instaMojo.ClientSecret,
                        instaEndpoint,
                        instaAuthEndpoint);
                }
                catch (Exception e)
                {
                    return Content("Something went wrong, Please try again after sometime");
                }

                try
                {
                    var transactionId = Guid.NewGuid().ToString();

                    #region   1. Create Payment Order
                    //  Create Payment Order
                    var objPaymentRequest = new PaymentOrder
                    {
                        name = $"{user.FirstName} {user.LastName}",
                        email = user.Email,
                        phone = user.PhoneNumber.Replace("+", ""),
                        amount = subtotal,
                        currency = "INR",
                        description = "",
                        webhook_url = "https://poojacraft.in/Checkout/WebHook",
                        //webhook_url =   Url.Action("WebHook", "Checkout", null, protocol: "https"),
                        transaction_id = transactionId,
                        redirect_url = Url.Action("ProcessPayment", "Checkout", null, protocol: "https")
                    };
                    //Required POST parameters
                    //Extra POST parameters 

                    if (objPaymentRequest.validate())
                    {

                        if (objPaymentRequest.emailInvalid)
                        {
                            return Content("Email is not valid");
                        }
                        if (objPaymentRequest.nameInvalid)
                        {
                            return Content("Name is not valid");
                        }
                        if (objPaymentRequest.phoneInvalid)
                        {
                            return Content("Phone is not valid");
                        }
                        if (objPaymentRequest.amountInvalid)
                        {
                            return Content("Amount is not valid");
                        }
                        if (objPaymentRequest.currencyInvalid)
                        {
                            return Content("Currency is not valid");
                        }
                        if (objPaymentRequest.transactionIdInvalid)
                        {
                            return Content("Transaction Id is not valid");
                        }
                        if (objPaymentRequest.redirectUrlInvalid)
                        {
                            return Content("Redirect Url Id is not valid");
                        }
                        if (objPaymentRequest.webhookUrlInvalid)
                        {
                            return Content("Webhook URL is not valid");
                        }

                    }
                    else
                    {
                        try
                        {
                            var order = new BasePackageModule2.Models.Order
                            {
                                UserId = user.Id,
                                PaymentStatus = "Pending",
                                TransactionId = transactionId,
                                Amount = subtotal,
                            };
                            _context.Add(order);
                            await _context.SaveChangesAsync();

                            foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                            {
                                OrderProduct orderProduct = new OrderProduct
                                {
                                    ProductId = cart.Product.Id,
                                    OrderId = order.Id,
                                    Quantity = cart.Qty
                                };
                                _context.Add(orderProduct);
                            }

                            await _context.SaveChangesAsync();

                            CreatePaymentOrderResponse objPaymentResponse = objClass.CreateNewPaymentRequest(objPaymentRequest);
                            return Redirect(objPaymentResponse.payment_options.payment_url);

                        }

                        catch (ArgumentNullException ex)
                        {
                            throw ex;
                        }
                        catch (WebException ex)
                        {
                            throw ex;
                        }
                        catch (IOException ex)
                        {
                            throw ex;
                        }
                        catch (InvalidPaymentOrderException ex)
                        {
                            throw ex;
                        }
                        catch (ConnectionException ex)
                        {
                            throw ex;
                        }
                        catch (BaseException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    #endregion

                }
                catch (BaseException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return BadRequest();
            }

            if (PaymentMode == "payAtShop")
            {
                var order = new BasePackageModule2.Models.Order
                {
                    UserId = user.Id,
                    AddressId = null,
                    PaymentStatus = "Pay At Shop",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = cart.Product.Id,
                        OrderId = order.Id,
                        Quantity = cart.Qty
                    };
                    _context.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
                _context.RemoveRange(carts);
                await _context.SaveChangesAsync();

                return View(nameof(ThankYou));
            }

            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> PaymentOptions(int addressId)
        {
            if (addressId == 0)
            {
                return BadRequest();
            }

            var aPaymentOptions = new APaymentOptions
            {
                AddressId = addressId
            };

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;


            return View(aPaymentOptions);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentOptions(int AddressId, string PaymentOption)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;

            if (PaymentOption == null || AddressId == 0)
            {
                ModelState.AddModelError("PaymentOption", "Please Select Payment option.");
                return View();
            }

            var add = await _context.Addresses.FirstOrDefaultAsync(i => i.Id == AddressId);

            if (add == null)
            {
                return BadRequest();
            }

           


            if (PaymentOption == "payOnline")
            {
                
                if (!await _context.Carts.AnyAsync(u => u.UserId == user.Id))
                {
                    return RedirectToAction("Index", "Cart");
                }

                string instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT, instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;


                if (_instaMojo.ClientId.Contains("test") || _instaMojo.ClientSecret.Contains("test"))
                {
                    instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT;
                    instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
                }

                else
                {
                    instaEndpoint = "https://api.instamojo.com/v2/";
                    instaAuthEndpoint = "https://www.instamojo.com/oauth2/token/";
                }

                IInstamojo objClass;
                try
                {
                    objClass = InstamojoImplementation.getApi(
                        _instaMojo.ClientId,
                        _instaMojo.ClientSecret,
                        instaEndpoint,
                        instaAuthEndpoint);
                }
                catch (Exception e)
                {
                    return Content("Something went wrong, Please try again after sometime");
                }


               
                try
                {
                    var transactionId = Guid.NewGuid().ToString();

                    #region   1. Create Payment Order
                    //  Create Payment Order
                    var objPaymentRequest = new PaymentOrder
                    {
                        name = $"{user.FirstName} {user.LastName}",
                        email = user.Email,
                        phone = add.MobileNumber.Replace("+", ""),
                        amount = subtotal,
                        currency = "INR",
                        description = "",
                        webhook_url = "https://poojacraft.in/Checkout/WebHook",
                        //webhook_url =   Url.Action("WebHook", "Checkout", null, protocol: "https"),
                        transaction_id = transactionId,
                        redirect_url = Url.Action("ProcessPayment", "Checkout", null, protocol: "https")
                    };
                    //Required POST parameters
                    //Extra POST parameters 

                    if (objPaymentRequest.validate())
                    {

                        if (objPaymentRequest.emailInvalid)
                        {
                            return Content("Email is not valid");
                        }
                        if (objPaymentRequest.nameInvalid)
                        {
                            return Content("Name is not valid");
                        }
                        if (objPaymentRequest.phoneInvalid)
                        {
                            return Content("Phone is not valid");
                        }
                        if (objPaymentRequest.amountInvalid)
                        {
                            return Content("Amount is not valid");
                        }
                        if (objPaymentRequest.currencyInvalid)
                        {
                            return Content("Currency is not valid");
                        }
                        if (objPaymentRequest.transactionIdInvalid)
                        {
                            return Content("Transaction Id is not valid");
                        }
                        if (objPaymentRequest.redirectUrlInvalid)
                        {
                            return Content("Redirect Url Id is not valid");
                        }
                        if (objPaymentRequest.webhookUrlInvalid)
                        {
                            return Content("Webhook URL is not valid");
                        }

                    }
                    else
                    {
                        try
                        {
                            var order = new BasePackageModule2.Models.Order
                            {
                                UserId = user.Id,
                                AddressId = add.Id,
                                PaymentStatus = "Pending",
                                TransactionId = transactionId,
                                Amount = subtotal,
                            };
                            _context.Add(order);
                            await _context.SaveChangesAsync();

                            foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                            {
                                OrderProduct orderProduct = new OrderProduct
                                {
                                    ProductId = cart.Product.Id,
                                    OrderId = order.Id,
                                    Quantity = cart.Qty
                                };
                                _context.Add(orderProduct);
                            }

                            await _context.SaveChangesAsync();

                            CreatePaymentOrderResponse objPaymentResponse = objClass.CreateNewPaymentRequest(objPaymentRequest);
                            return Redirect(objPaymentResponse.payment_options.payment_url);

                        }

                        catch (ArgumentNullException ex)
                        {
                            throw ex;
                        }
                        catch (WebException ex)
                        {
                            throw ex;
                        }
                        catch (IOException ex)
                        {
                            throw ex;
                        }
                        catch (InvalidPaymentOrderException ex)
                        {
                            throw ex;
                        }
                        catch (ConnectionException ex)
                        {
                            throw ex;
                        }
                        catch (BaseException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    #endregion

                }
                catch (BaseException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return BadRequest();
            }

            if (PaymentOption == "cashOnDelivery")
            {
                var order = new BasePackageModule2.Models.Order
                {
                    UserId = user.Id,
                    AddressId = add.Id,
                    PaymentStatus = "Cash on Delivery",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = cart.Product.Id,
                        OrderId = order.Id,
                        Quantity = cart.Qty
                    };
                    _context.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
                _context.RemoveRange(carts);
                await _context.SaveChangesAsync();

                return View(nameof(ThankYou));
            }
            return BadRequest();

        }
        public class APaymentOptions
        {
            public int AddressId { get; set; }
            [Required]
            public string PaymentOption { get; set; }
        }
        public class PaymentModes
        {
            [Required]
            public string PaymentMode { get; set; }
        }
    }
}