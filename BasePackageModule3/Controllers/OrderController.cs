﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using BasePackageModule3.Utility;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailsender;

        public OrderController(ApplicationDbContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailsender = emailSender;
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDeatilsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.Include(o => o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _context.OrderDetails.Where(o => o.OrderId == id).ToListAsync()


        };

            return View(orderDeatilsViewModel);

        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<OrderDetailsViewModel> orderList = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList = await _context.OrderHeaders.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToListAsync();
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _context.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderList.Add(individual);
            }
            return View(orderList);
        }

        public async Task<IActionResult> GetOrderDeatails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _context.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);
            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }


        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ManageOrder()
        {
            
            List<OrderDetailsViewModel> orderDetailsVm = new List<OrderDetailsViewModel>();

            List<OrderHeader> OrderHeaderList = await _context.OrderHeaders.Where(o=>o.status == SD.StatusSubmitted || o.status==SD.StatusInProcess).OrderByDescending(d=>d.PickUpTime).ToListAsync();
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _context.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVm.Add(individual);
            }
            return View(orderDetailsVm);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.status = SD.StatusInProcess;
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.status = SD.StatusReady;
            await _context.SaveChangesAsync();

            //Email Logic  to notify User
            await _emailsender.SendEmailAsync(_context.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Base-Package - Order Ready for Pickup" + orderHeader.Id.ToString(), "Order Ready for Pickup");
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.status = SD.StatusCancel;
            await _context.SaveChangesAsync();

            //Email Logic  to notify User

            await _emailsender.SendEmailAsync(_context.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Base-Package - Order Cancelled" + orderHeader.Id.ToString(), "Order has been Cancelled successfully");
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickUp()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<OrderDetailsViewModel> orderList = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList = await _context.OrderHeaders.Include(o => o.ApplicationUser).Where(u => u.status == SD.StatusReady).ToListAsync();
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _context.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderList.Add(individual);
            }
            
;            return View(orderList);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ActionName("OrderPickUp")]
        public async Task<IActionResult> OrderPickUpPost(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.status = SD.StatusCompleted;
            await _context.SaveChangesAsync();
            await _emailsender.SendEmailAsync(_context.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Base-Package - Order Completed" + orderHeader.Id.ToString(), "Order Completed Successfully");
            return RedirectToAction("OrderPickup", "Order");
        }

    }
}