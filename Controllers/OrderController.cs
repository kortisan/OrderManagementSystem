using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;


namespace OrderManagementSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDataContext _context;

        public OrderController(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string keyword, DateTime? orderDate)
        {
            var query = _context.SO_ORDER
                .Include(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o => o.ORDER_NO.Contains(keyword));
            }

            if (orderDate.HasValue)
            {
                query = query.Where(o => o.ORDER_DATE.Date == orderDate.Value.Date);
            }

            var orders = await query.ToListAsync();
            return View(orders);
        }

        public IActionResult Create()
        {
            var customerList = _context.COM_CUSTOMER
                .AsNoTracking()
                .Select(c => new SelectListItem
                {
                    Value = c.COM_CUSTOMER_ID.ToString(),
                    Text = c.CUSTOMER_NAME
                }).ToList();

            var model = new OrderCreateViewModel
            {
                ORDER_DATE = DateTime.Now,
                Items = new List<ItemViewModel> { new ItemViewModel() },
                Customers = customerList
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Customers = _context.COM_CUSTOMER
                    .AsNoTracking()
                    .Select(c => new SelectListItem
                    {
                        Value = c.COM_CUSTOMER_ID.ToString(),
                        Text = c.CUSTOMER_NAME
                    }).ToList();

                return View(model);
            }

            var items = JsonSerializer.Deserialize<List<ItemViewModel>>(model.ItemsJson ?? "[]");

            var order = new Order
            {
                ORDER_NO = model.ORDER_NO,
                ORDER_DATE = model.ORDER_DATE,
                COM_CUSTOMER_ID = model.COM_CUSTOMER_ID,
                Items = items.Select(i => new Item
                {
                    ITEM_NAME = i.ITEM_NAME,
                    QUANTITY = i.QUANTITY,
                    PRICE = i.PRICE
                }).ToList()
            };

            _context.SO_ORDER.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(long id)
        {
            var order = await _context.SO_ORDER
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order == null)
                return NotFound();

            var model = new OrderCreateViewModel
            {
                ORDER_NO = order.ORDER_NO,
                ORDER_DATE = order.ORDER_DATE,
                COM_CUSTOMER_ID = order.COM_CUSTOMER_ID,
                Items = order.Items.Select(i => new ItemViewModel
                {
                    ITEM_NAME = i.ITEM_NAME,
                    QUANTITY = i.QUANTITY,
                    PRICE = i.PRICE
                }).ToList(),
                Customers = _context.COM_CUSTOMER
                    .Select(c => new SelectListItem
                    {
                        Value = c.COM_CUSTOMER_ID.ToString(),
                        Text = c.CUSTOMER_NAME
                    }).ToList()
            };

            ViewBag.OrderId = id;
            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Customers = _context.COM_CUSTOMER
                    .Select(c => new SelectListItem
                    {
                        Value = c.COM_CUSTOMER_ID.ToString(),
                        Text = c.CUSTOMER_NAME
                    }).ToList();
                ViewBag.OrderId = id;
                return View(model);
            }

            var order = await _context.SO_ORDER
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order == null)
                return NotFound();

            order.ORDER_NO = model.ORDER_NO;
            order.ORDER_DATE = model.ORDER_DATE;
            order.COM_CUSTOMER_ID = model.COM_CUSTOMER_ID;

            _context.SO_ITEM.RemoveRange(order.Items);

            var items = JsonSerializer.Deserialize<List<ItemViewModel>>(model.ItemsJson ?? "[]") ?? new();
            order.Items = items.Select(i => new Item
            {
                ITEM_NAME = i.ITEM_NAME,
                QUANTITY = i.QUANTITY,
                PRICE = i.PRICE,
                SO_ORDER_ID = order.SO_ORDER_ID
            }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.SO_ORDER
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order == null)
                return NotFound();

            _context.SO_ITEM.RemoveRange(order.Items); 
            _context.SO_ORDER.Remove(order);           

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Export()
        {
            var orders = await _context.SO_ORDER
                .Include(o => o.Customer)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sales Orders");

            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Order Number";
            worksheet.Cell(1, 3).Value = "Order Date";
            worksheet.Cell(1, 4).Value = "Customer";

            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                worksheet.Cell(i + 2, 1).Value = i + 1;
                worksheet.Cell(i + 2, 2).Value = order.ORDER_NO;
                worksheet.Cell(i + 2, 3).Value = order.ORDER_DATE.ToString("yyyy-MM-dd");
                worksheet.Cell(i + 2, 4).Value = order.Customer?.CUSTOMER_NAME;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SalesOrders.xlsx");
        }
    }
}
