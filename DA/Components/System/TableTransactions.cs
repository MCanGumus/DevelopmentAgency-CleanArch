using DA.Domain.Entities;

namespace DA.Components.System
{
    public class TableTransactions
    {
        public static string AddToTable(List<string> lstString, Guid id)
        {
            string result = "$('.dataTable').DataTable().row.add([{0}]).draw(false).node().id='{1}'; " +
                            "var addedRow = $('.dataTable').DataTable().row(':last').node(); " +
                            "$(addedRow).prependTo('.dataTable tbody');";

            string values = "";
            foreach (string str in lstString)
                values += $"'{str}',";

            return string.Format(result, values.TrimEnd(','), id);
        }


        public static string UpdateTable(List<string> lstString, Guid id)
        {
            string result = @"var table = $("".dataTable"").DataTable();var rowData = [{0}];var row = table.row(""[id='{1}']"");row.data(rowData).draw();";

            string values = "";
            foreach (string str in lstString)
                values += $"'{str}',";

            return string.Format(result, values.TrimEnd(','), id);
        }

        public static string DeleteTable(Guid id)
        {
            return $@"var table = $("".dataTable"").DataTable();var row = table.row(""[id='{id}']"");row.remove().draw();";
        }

        public static string TableCreate_ByGuid(string tabloAdi, string controllerName, string basliklarParm, string dugmelerBaslikParm, string dugmelerOperationParm, List<string> data)
        {
            string[] basliklar = basliklarParm.Split('-');
            string[] dugmeler = string.IsNullOrEmpty(dugmelerBaslikParm) ? null : dugmelerBaslikParm.Split('-');
            string[] dugmelerOperation = string.IsNullOrEmpty(dugmelerOperationParm) ? null : dugmelerOperationParm.Split('-');

            int baslikSayisi = basliklar.Count();
            int satirSayisi = data.Count() / baslikSayisi;

            string Taslak = string.Format(@"<table id=""{0}"" class=""table dataTable table-hover table-striped"">
                                    <thead>
                                        <tr>
                                           <basliklar> 
                                        </tr>
                                    </thead> 
                                    <tbody> 
                                           <data>
                                    </tbody> 
                                    </table>
                                ", tabloAdi);
            Taslak = Taslak.Replace("\r\n", string.Empty);

            #region başlıkları oluşturalım
            string baslikTemp = null;

            foreach (string baslik in basliklar)
                baslikTemp += string.Format(@"<th>{0}</th>", baslik);

            if (dugmeler != null)
            {
                baslikTemp += string.Format(@"<th>{0}</th>", "İşlemler");
            }

            Taslak = Taslak.Replace("<basliklar>", baslikTemp);
            #endregion

            #region data oluşturalım

            int index = 0;
            string dataRegion = null;
            Guid ObjectId = Guid.Empty;
            foreach (string d in data)
            {
                if (index % (baslikSayisi + 1) == 0)//satır başı
                {
                    ObjectId = Guid.Parse(d);
                    dataRegion += string.Format(@"<tr id=""{0}Tr_{1}"">", tabloAdi, ObjectId);

                    index++;
                    continue;//ilk data object olduğu için devam ederiz
                }

                dataRegion += string.Format("<td>{0}</td>", d);

                if (index % (baslikSayisi + 1) == baslikSayisi)//satırdaki son data
                {

                    dataRegion += "</tr>";
                }

                index++;
            }

            Taslak = Taslak.Replace("<data>", dataRegion);
            #endregion


            return Taslak;
        }
    }
}
