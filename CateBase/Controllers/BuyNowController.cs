using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BasePackageModule2.Data;
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
using Order = BasePackageModule2.Models.Order;

namespace BasePackageModule1.Controllers
{
    [Authorize]
    public class BuyNowController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private IInstaMojoConfiguration _instaMojo;
        public BuyNowController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IInstaMojoConfiguration instaMojo)
        {
            _context = context;
            _userManager = userManager;
            _instaMojo = instaMojo;
        }

        public async Task<IActionResult> Index(int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var addresses = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();
            ViewData["productId"] = productId;
            ViewData["qty"] = qty;

            ViewData["Addresses"] = addresses;

            return View();
        }
        public IActionResult ThankYou()
        {
            return View();
        }
        public async Task<IActionResult> Address(Address address, int productId, int qty)
        {
            if (!ModelState.IsValid)
            {
                ViewData["productId"] = productId;
                ViewData["qty"] = qty;

                return View(nameof(Index), address);
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            address.UserId = user.Id;
            _context.Add(address);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PaymentOptions), new { addressId = address.Id, productId, qty});
        }

        public async Task<IActionResult> CheckOutOptions(int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }

            var cOptions = new ACheckoutoptions
            {
                ProductId = productId,
                Qty = qty
            };

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            var subtotal = (double)(product.FinalPrice* qty);

            ViewData["Items"] = subtotal;

            return View(cOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutOptions(string PaymentMode, int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }
            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;


            if (PaymentMode == null)
            {
                ModelState.AddModelError("PaymentMode", "Please Select Payment Mode.");
                var cOptions = new ACheckoutoptions
                {
                    ProductId = productId,
                    Qty = qty
                };

                return View(cOptions);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);



         


            if (PaymentMode == "payOnline")
            {
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
                        phone = user.PhoneNumber.Replace("+",""),
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
                            var order = new Order
                            {
                                UserId = user.Id,
                                PaymentStatus = "Pending",
                                TransactionId = transactionId,
                                Amount = subtotal,
                            };
                            _context.Add(order);
                            await _context.SaveChangesAsync();

                            OrderProduct orderProduct = new OrderProduct
                            {
                                ProductId = product.Id,
                                OrderId = order.Id,
                                Quantity = qty
                            };
                            _context.Add(orderProduct);

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
                var order = new Order
                {
                    UserId = user.Id,
                    AddressId = null,
                    PaymentStatus = "Pay At Shop",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = product.Id,
                        OrderId = order.Id,
                        Quantity = qty
                    };
                    _context.Add(orderProduct);
                

                await _context.SaveChangesAsync();

               
                return View(nameof(ThankYou));
            }

            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> PaymentOptions(int addressId, int productId, int qty)
        {
            if (addressId == 0 || productId == 0 || qty == 0)
            {
                return BadRequest();
            }

            var aPaymentOptions = new APaymentOptions
            {
                AddressId = addressId,
                ProductId = productId,
                Qty = qty
            };
            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;

            return View(aPaymentOptions);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentOptions(int AddressId, string PaymentOption, int productId, int qty)
        {
            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;

            if (PaymentOption == null || AddressId == 0 || productId == 0 || qty == 0)
            {
                ModelState.AddModelError("PaymentOption", "Please Select Payment option.");

                var aPaymentOptions = new APaymentOptions
                {
                    AddressId = AddressId,
                    ProductId = productId,
                    Qty = qty
                };
                return View(aPaymentOptions);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var add = await _context.Addresses.FirstOrDefaultAsync(i => i.Id == AddressId);

            if (add == null)
            {
                return BadRequest();
            }

           
          
            if (PaymentOption == "payOnline")
            {

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
                        phone = add.MobileNumber.Replace("+",""),
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
                            var order = new Order
                            {
                                UserId = user.Id,
                                AddressId = add.Id,
                                PaymentStatus = "Pending",
                                TransactionId = transactionId,
                                Amount = subtotal,
                            };
                            _context.Add(order);
                            await _context.SaveChangesAsync();

                            
                            OrderProduct orderProduct = new OrderProduct
                            {
                                ProductId = product.Id,
                                OrderId = order.Id,
                                Quantity = qty
                            };
                            _context.Add(orderProduct);

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
                var order = new Order
                {
                    UserId = user.Id,
                    AddressId = add.Id,
                    PaymentStatus = "Cash on Delivery",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                OrderProduct orderProduct = new OrderProduct
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Quantity = qty
                };
                _context.Add(orderProduct);

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

            public int ProductId { get; set; }
            public int Qty { get; set; }
        }
        public class ACheckoutoptions
        {
            [Required]
            public string PaymentMode { get; set; }

            public int ProductId { get; set; }
            public int Qty { get; set; }
        }
    }
}
