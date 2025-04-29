using DienDanThaoLuan.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using DienDanThaoLuan.Filters;
using Ganss.XSS;

namespace DienDanThaoLuan.Areas.Admin.Controllers
{
    [SessionTimeout]
    [Authorize]
    public class QLThanhVienController : Controller
    {
        DienDanThaoLuanEntities db = new DienDanThaoLuanEntities();
        // GET: Admin/QLThanhVien
        public ActionResult Index()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult QLThanhVien(int? page, string searchInput = null, string sortOrder = null)
        {
            var ds = db.ThanhViens.OrderBy(l => l.TenDangNhap).ToList();
            searchInput = XuLyNoiDung(searchInput);
            if (!string.IsNullOrEmpty(searchInput))
            {
                ViewBag.SearchInput = searchInput;
                ds = ds.Where(l => l.TenDangNhap.Contains(searchInput)).ToList();
            }
            switch (sortOrder)
            {
                case "namez-a":
                    ds = ds.OrderByDescending(m => m.TenDangNhap).ToList(); // Sắp xếp tên từ Z-A
                    break;
                case "datenew":
                    ds = ds.OrderByDescending(m => m.NgayThamGia).ToList(); // Sắp xếp ngày tham gia mới nhất
                    break;
                case "dateold":
                    ds = ds.OrderBy(m => m.NgayThamGia).ToList(); // Sắp xếp ngày tham gia lâu nhất
                    break;
                default:
                    break;
            }
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.SL = ds.Count();
            int iSize = 6;
            int iPageNumber = (page ?? 1);
            return View(ds.ToPagedList(iPageNumber, iSize));
        }
        public ActionResult KhoaTaiKhoan(string id)
        {
            var ds = db.ThanhViens.Where(l => l.MaTV == id).SingleOrDefault();
            if(ds != null)
            {
                ds.MatKhau = null;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã khóa tài khoản";
            }
            else
            {
                TempData["ThongBao"] = "Không tìm thấy tài khoản này";
            }
            return RedirectToAction("QLThanhVien");
        }
        public static string XuLyNoiDung(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear(); // Không cho phép bất kỳ thẻ HTML nào
            sanitizer.AllowedAttributes.Clear();

            return sanitizer.Sanitize(input);
        }
    }
}