using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Domain.Entities.OldDatas
{
    public class OldEmployees
    {
        [Key]
        public int Id { get; set; }
        public int? Personel_ID { get; set; }
        public string? Tc { get; set; }
        public string? AdSoyad{ get; set; }
        public string? BabaAdi{ get; set; }
        public string? AnneAdi { get; set; }
        public string? DogumYeri{ get; set; }
        public DateTime? DogumTarihi { get; set; }
        public DateTime? IseBaslama { get; set; }
        public int? Kan_ID{ get; set; }
        public int? Cin_ID { get; set; }
        public string? Email{ get; set; }
        public string? Pass{ get; set; }
        public int? Yetki{ get; set; }
        public int? Tizin { get; set; }
        public string? SicilNo{ get; set; }
        public int? Unvan_Id { get; set; }
        public int? Birim_ID { get; set; }
        public int? Amir_ID { get; set; }
        public bool? Durum { get; set; }
        public bool? Degerlendirme { get; set; }
        public bool? cikisMi { get; set; }

    }
}
