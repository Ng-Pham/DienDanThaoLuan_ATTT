//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DienDanThaoLuan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ThongBao
    {
        public string MaTB { get; set; }
        public string NoiDung { get; set; }
        public Nullable<System.DateTime> NgayTB { get; set; }
        public string LoaiTB { get; set; }
        public string MaTV { get; set; }
        public string MaDoiTuong { get; set; }
        public string LoaiDoiTuong { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public string MaQTV { get; set; }
    
        public virtual ThanhVien ThanhVien { get; set; }
        public virtual QuanTriVien QuanTriVien { get; set; }
    }
}
