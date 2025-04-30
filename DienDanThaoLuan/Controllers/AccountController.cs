﻿using DienDanThaoLuan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DienDanThaoLuan.Controllers;
using System.Web.Helpers;
using Serilog;

namespace DienDanThaoLuan.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        DienDanThaoLuanEntities db = new DienDanThaoLuanEntities();

        //Dang Nhap && Dang Ky
        [HttpGet]
        public ActionResult Login()
        {
            // Kiểm tra nếu đã đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "DienDanThaoLuan");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            //check null tài khoản && mật khẩu
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.error = "*Không được để trống tài khoản hoặc mật khẩu!!!";
                return View();
            }
            //Lấy dữ liệu tài khoản mật khẩu
            var memberAcc = db.ThanhViens.SingleOrDefault(m => m.TenDangNhap.ToLower() == username.ToLower());

            //Check tồn tại tài khoản
            if (memberAcc == null)
            {
                var adminAcc = db.QuanTriViens.SingleOrDefault(m => m.TenDangNhap.ToLower() == username.ToLower());

                // Check tồn tại tài khoản trong bảng QuanTriVien
                if (adminAcc == null)
                {
                    ViewBag.error = "Tài khoản không tồn tại!!";
                    ViewBag.username = username;
                    return View();
                }

                // Check đúng sai tài khoản mật khẩu của QuanTriVien
                if (!BCrypt.Net.BCrypt.Verify(password, adminAcc.MatKhau) || adminAcc.TenDangNhap != username)
                {
                    ViewBag.error = "Sai tên tài khoản hoặc mật khẩu!! Vui lòng thử lại";
                    ViewBag.username = username;
                    return View();
                }

                // Đăng nhập thành công với tài khoản QuanTriVien
                FormsAuthentication.SetAuthCookie(username, false);
                Session["AdminId"] = adminAcc.MaQTV;
                Session["Role"] = "Admin"; // lưu quyền
                Log.Information("AdminId {Username} đã đăng nhập thành công", username);
                return RedirectToAction("Index", "DienDanThaoLuan");
            }

            if (memberAcc.MatKhau == null)
            {
                ViewBag.error = "Tài khoản này đã bị khóa!!";
                ViewBag.username = username;
                return View();
            }
            //Check đúng sai tài khoản mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(password, memberAcc.MatKhau) || memberAcc.TenDangNhap != username)
            {
                ViewBag.error = "Sai tên tài khoản hoặc mật khẩu!! Vui lòng thử lại";
                ViewBag.username = username;
                return View();
            }
            //Đăng nhập thành công
            FormsAuthentication.SetAuthCookie(username, false);
            Session["UserId"] = memberAcc.MaTV;
            Session["Role"] = "Member"; // lưu quyền
            Log.Information("UserId {Username} đã đăng nhập thành công", username);
            return RedirectToAction("Index", "DienDanThaoLuan");
        }//---Hoàn thành chức năng đăng nhập
        //Chức năng Đăng xuất 
        public ActionResult Logout()
        {
            var username = User.Identity.Name;
            Session["UserId"] = null;
            Session["AdminId"] = null;
            Session["Role"] = null;
            FormsAuthentication.SignOut();

            Log.Information("User {Username} đã đăng xuất thành công", username);

            return RedirectToAction("Index", "DienDanThaoLuan");
        }

        //Chức năng Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(ThanhVien tv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var lastTV = db.ThanhViens.OrderByDescending(t => t.MaTV).FirstOrDefault();
                    string newMaTV = "TV" + (Convert.ToInt32(lastTV.MaTV.Substring(2)) + 1).ToString("D3");
                    // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                    var existingUser = db.ThanhViens.FirstOrDefault(x => x.TenDangNhap == tv.TenDangNhap);
                    var existingEmail = db.ThanhViens.FirstOrDefault(x => x.Email == tv.Email);
                    if (existingUser != null)
                    {
                        ViewBag.error = "Tên đăng nhập đã tồn tại!! Vui lòng thử lại";
                        ViewBag.tv.TenDangNhap = tv.TenDangNhap;
                        return View(tv);
                    }
                    else if (existingEmail != null)
                    {
                        ViewBag.error = "Email đã được sử dụng!! Vui lòng thử lại";
                        ViewBag.tv.Email = tv.Email;
                        return View(tv);
                    }
                    else if(tv.MatKhau.Length < 8)
                    {
                        ViewBag.error = "Mật khẩu phải có độ dài ít nhất 8 ký tự";
                        ViewBag.tv.TenDangNhap = tv.TenDangNhap;
                        return View(tv);
                    }
                    tv.NgayThamGia = DateTime.Now;
                    tv.MaTV = newMaTV;
                    tv.AnhDaiDien = "avatar.jpg";
                    tv.MatKhau = BCrypt.Net.BCrypt.HashPassword(tv.MatKhau);
                    // Thêm thành viên mới vào database
                    Log.Information("User {ViewBag.tv.TenDangNhap} đã đăng kí thành công", ViewBag.tv.TenDangNhap);
                    db.ThanhViens.Add(tv);
                    db.SaveChanges();

                    // Điều hướng đến trang thành công hoặc đăng nhập
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại! " + ex.Message);
                }
            }
            return View(tv); ;
        }//---Hoàn thành chức năng đăng ký
        //Chức năng quên mật khẩu---
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập email." });
            }

            // Kiểm tra email có tồn tại trong hệ thống hay không
            var user = db.ThanhViens.SingleOrDefault(m => m.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                return Json(new { success = false, message = "Email không tồn tại trong hệ thống!" });
            }

            if (user.MatKhau == null)
            {
                return Json(new { success = false, message = "Tài khoản với email này đã bị khóa!!" });
            }
            // Kiểm tra thời gian từ lần gửi yêu cầu cuối cùng
            if (user.LastPasswordResetRequest.HasValue &&
                (DateTime.Now - user.LastPasswordResetRequest.Value).TotalSeconds < 30)
            {
                int remainingTime = 30 - (int)(DateTime.Now - user.LastPasswordResetRequest.Value).TotalSeconds;
                return Json(new { success = false, message = "Vui lòng đợi 30 giây trước khi gửi yêu cầu mới.", remainingTime = remainingTime });
            }

            // Tạo mật khẩu mới ngẫu nhiên
            var newPassword = GenerateRandomPassword();
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword); // Cập nhật mật khẩu mới vào database
            user.LastPasswordResetRequest = DateTime.Now; // Cập nhật thời gian gửi yêu cầu cuối cùng
            db.SaveChanges();
            Log.Information("User {user.TenDangNhap} đã đổi mật khẩu thành công", user.TenDangNhap);

            // Gửi email mật khẩu mới cho người dùng
            SendPasswordResetEmail(user.Email, newPassword);

            return Json(new { success = true, message = "Mật khẩu mới đã được gửi đến email của bạn!", remainingTime = 30 });
        }//---Hoàn thành chức năng quên mật khẩu

        // Hàm tạo mật khẩu ngẫu nhiên
        private string GenerateRandomPassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 8; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        // Hàm gửi email chứa mật khẩu mới
        private void SendPasswordResetEmail(string toEmail, string newPassword)
        {
            var fromEmail = "2224802010927@student.tdmu.edu.vn"; // Email của bạn
            var fromPassword = "juak jlej ftgf ncpk"; // Mật khẩu của bạn
            var subject = "Mật khẩu mới của bạn";
            var body = $"Mật khẩu mới của bạn là: {newPassword}";

            // Cấu hình SMTP trong controller
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            // Tạo và gửi email
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true, // Nếu muốn gửi mail dạng HTML
            };

            mailMessage.To.Add(toEmail);

            // Gửi mail
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu việc gửi mail thất bại
                ViewBag.Message = $"Đã xảy ra lỗi khi gửi email: {ex.Message}";
            }
        }//--- Hoàn thành chức năng gửi mail khi quên mật khẩu
    }
}