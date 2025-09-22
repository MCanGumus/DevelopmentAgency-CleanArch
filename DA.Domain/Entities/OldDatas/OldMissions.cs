using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Domain.Entities.OldDatas
{
    public class OldMissions
    {
        [Key]
        public int Id { get; set; }
        public int? Gor_Id { get; set; }
        public int? Personel_ID { get; set; }
        public int? Tur_ID { get; set; }
        public string? Yer {  get; set; }
        public string? Konu {  get; set; }
        public DateTime? B_Tarih { get; set; }
        public DateTime? G_Tarih { get; set; }
        public int? G_Arac { get; set; }
        public int? D_Arac { get; set; }
        public int? VekID { get; set; }
        public string? GorNot { get; set; }
        public string? Avans { get; set; }
    }
}
