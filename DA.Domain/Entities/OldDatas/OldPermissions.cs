using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Domain.Entities.OldDatas
{
    public class OldPermissions
    {
        [Key]
        public int Id { get; set; }
        public int? IzinId { get; set; }
        public int? Personel_ID { get; set; }
        public string? IzinNedeni { get; set; }
        public DateTime? IzinBaslangic { get; set; }
        public DateTime? IzinBitis { get; set; }
        public int? Ad{ get; set; }
        public int? Toplam { get; set; }
        public int? VekID{ get; set; }
        public bool Onay{ get; set; }
        public bool? MazaretMi{ get; set; }
        public string? MazaretKodu { get; set; }

    }
}
