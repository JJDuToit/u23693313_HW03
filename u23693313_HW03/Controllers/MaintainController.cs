using u23693313_HW03.Models;
using u23693313_HW03.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BikeStoreSales.Controllers
{
    public class MaintainController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: Maintain
        public async Task<ActionResult> Index(int? brandId, int? categoryId, int staffPage = 1, int custPage = 1, int prodPage = 1)
        {
            var viewModel = new HomeViewModel();
            const int itemsPerPage = 1;

            // 1. Staff Paging
            IQueryable<staff> staffQuery = db.staffs.Include(s => s.store);
            viewModel.StaffPaging = new PagingInfo
            {
                TotalItems = await staffQuery.CountAsync(),
                CurrentPage = staffPage,
                ItemsPerPage = itemsPerPage
            };
            viewModel.StaffMember = await staffQuery.OrderBy(s => s.staff_id)
                .Skip((staffPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .FirstOrDefaultAsync();

            // Get Staff Sales
            if (viewModel.StaffMember != null)
            {
                viewModel.StaffSales = await db.order_items
                    .Include(oi => oi.product)
                    .Include(oi => oi.order)
                    .Where(oi => oi.order.staff_id == viewModel.StaffMember.staff_id)
                    .OrderByDescending(oi => oi.order.order_date)
                    .Take(5)
                    .ToListAsync();
            }
            else
            {
                viewModel.StaffSales = new List<order_items>();
            }

            // 2. Customer Paging
            IQueryable<customer> customerQuery = db.customers;
            viewModel.CustomerPaging = new PagingInfo
            {
                TotalItems = await customerQuery.CountAsync(),
                CurrentPage = custPage,
                ItemsPerPage = itemsPerPage
            };
            viewModel.Customer = await customerQuery.OrderBy(c => c.customer_id)
                .Skip((custPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .FirstOrDefaultAsync();

            // Get Customer Purchases
            if (viewModel.Customer != null)
            {
                viewModel.CustomerPurchases = await db.order_items
                    .Include(oi => oi.product)
                    .Include(oi => oi.order)
                    .Where(oi => oi.order.customer_id == viewModel.Customer.customer_id)
                    .OrderByDescending(oi => oi.order.order_date)
                    .Take(5)
                    .ToListAsync();
            }
            else
            {
                viewModel.CustomerPurchases = new List<order_items>();
            }

            // 3. Product Paging & Filtering
            IQueryable<product> productQuery = db.products.Include(p => p.brand).Include(p => p.category);

            if (brandId.HasValue)
            {
                productQuery = productQuery.Where(p => p.brand_id == brandId.Value);
            }
            if (categoryId.HasValue)
            {
                productQuery = productQuery.Where(p => p.category_id == categoryId.Value);
            }

            viewModel.ProductPaging = new PagingInfo
            {
                TotalItems = await productQuery.CountAsync(),
                CurrentPage = prodPage,
                ItemsPerPage = itemsPerPage
            };
            viewModel.Product = await productQuery.OrderBy(p => p.product_id)
                .Skip((prodPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .FirstOrDefaultAsync();

            // Populate filter dropdowns
            viewModel.Brands = new SelectList(await db.brands.OrderBy(b => b.brand_name).ToListAsync(), "brand_id", "brand_name");
            viewModel.Categories = new SelectList(await db.categories.OrderBy(c => c.category_name).ToListAsync(), "category_id", "category_name");

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}