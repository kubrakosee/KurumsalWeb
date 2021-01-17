using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KurumsalWeb.Models
{
    //yorum veritabanı oluşturma
    [Table("Yorum")]
    public class Yorum
    {
        public int YorumId { get; set; }
        [Required,StringLength(50,ErrorMessage ="50 Karakter Olabilir.")]
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        [DisplayName("Yorumunuz")]
        public string İcerik { get; set; }
        public bool Onay { get; set; }

        //Bu kayıt hangi blog ıd sine ait 2.aşama sonra blog.cs ye gel
        public int? BlogId { get; set; }
        //Blog tablosundan Bir Blog bilgisi içeriyor 1.aşama
        public Blog Blog { get; set; }
    }
}