using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KurumsalWeb.Models.Model
{
    [Table("Blog")]
    public class Blog
    {
        public int BlogId { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public string ResimURL { get; set; }
        public int? KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        //3.aşama da yorumun koleksiyonu içeriyor diyeceğiz
        //Bu işlemlerin veri tabanı da yansıması için package consolunu açıyorsun
        //update-database -force veri tabanına zorla 
        public ICollection <Yorum> Yorums { get; set; }
    }
}