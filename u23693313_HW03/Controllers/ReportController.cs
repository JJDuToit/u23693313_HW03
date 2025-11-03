using u23693313_HW03.Models;
using u23693313_HW03.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BikeStoreSales.Controllers
{
    public class ReportController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: Report
        public async Task<ActionResult> Index()
        {
            var archives = await db.DocumentArchives.OrderByDescending(d => d.ArchivedDate).ToListAsync();
            return View(archives);
        }

        // GET: Report/GetChartData
        public async Task<JsonResult> GetChartData()
        {
            var data = await db.order_items
                .GroupBy(oi => oi.product.product_name)
                .Select(g => new PopularProductViewModel
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(oi => oi.quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToListAsync();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: Report/SaveReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveReport(string reportName, string description, string fileType)
        {
            if (string.IsNullOrWhiteSpace(reportName))
            {
                TempData["ReportError"] = "Report name cannot be empty.";
                return RedirectToAction("Index");
            }

            var data = await db.order_items
                .GroupBy(oi => oi.product.product_name)
                .Select(g => new PopularProductViewModel
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(oi => oi.quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync();

            byte[] fileBytes;
            string fileExtension;

            switch (fileType)
            {
                case "csv":
                    fileExtension = ".csv";
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("ProductName,TotalSold");
                    foreach (var item in data)
                    {
                        sb.AppendLine($"\"{item.ProductName.Replace("\"", "\"\"")}\",{item.TotalSold}");
                    }
                    fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    break;

                case "pdf":
                default:
                    fileExtension = ".pdf";
                    var pdf = new ViewAsPdf("_PopularProductReportPdf", data)
                    {
                        FileName = "PopularProductReport.pdf",
                        PageSize = Rotativa.Options.Size.A4
                    };
                    fileBytes = pdf.BuildFile(this.ControllerContext);
                    break;
            }

            string uniqueFileName = $"{reportName.Replace(" ", "_")}_{Guid.NewGuid()}{fileExtension}";
            string serverPath = Path.Combine(Server.MapPath("~/App_Data/Archives/"), uniqueFileName);

            System.IO.File.WriteAllBytes(serverPath, fileBytes);

            var archive = new DocumentArchive
            {
                ReportName = reportName,
                FileName = uniqueFileName,
                FilePath = serverPath,
                ArchivedDate = DateTime.Now,
                Description = description
            };
            db.DocumentArchives.Add(archive);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Report/DownloadFile/5
        public async Task<ActionResult> DownloadFile(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var file = await db.DocumentArchives.FindAsync(id);
            if (file == null) return HttpNotFound();

            if (!System.IO.File.Exists(file.FilePath))
            {
                return HttpNotFound("File not found on server.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(file.FilePath);

            string fileExtension = Path.GetExtension(file.FileName);
            string mimeType = System.Web.MimeMapping.GetMimeMapping(file.FileName);

            return File(fileBytes, mimeType, file.ReportName + fileExtension);
        }

        // GET: Report/DeleteFile/5
        public async Task<ActionResult> DeleteFile(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var file = await db.DocumentArchives.FindAsync(id);
            if (file == null) return HttpNotFound();

            return View(file);
        }

        // POST: Report/DeleteFile/5
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteFileConfirmed(int id)
        {
            var file = await db.DocumentArchives.FindAsync(id);
            if (file == null) return HttpNotFound();

            try
            {
                if (System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }
            }
            catch
            {
            }

            db.DocumentArchives.Remove(file);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
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