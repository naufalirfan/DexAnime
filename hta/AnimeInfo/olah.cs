using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Anime_Info {
    class olah {
        /* Class berisi method-method untuk pengolahan string */
        public static string pisah(string isi, string awal, string akhir) {
            string hasil = "-"; 
            if (isi.Contains(awal) && isi.Contains(akhir)) {
                string setengah = potong(isi, awal)[1];   //Potong awal
                hasil = potong(setengah, akhir)[0];       //Potong akhir
            }
            return hasil;   //Hasil merupakan string diantara awal dan akhir
        }
        public static string[] potong(string isi, string pemisah) { //method buat potongnya
            return isi.Split(new string[] { pemisah }, StringSplitOptions.None);
        }
        public static string tanpaHtml(string input, string awal, int pemotongan) {
            string mentah = pisah(input, awal, "</div>");
            return Regex.Replace(mentah, "<.*?>", String.Empty).Substring(pemotongan);
        }

        public static bool bisaDiDownload(string key) {
            //Ngecek di DexAnime ada link download dgn key tsb tidak
            string isi;
            if (File.Exists(@"list.db")) {
                isi = File.ReadAllText(@"list.db");       //Faster loading
            } else {
                isi = request.baca(lokasi.dexList);       //Jaga2 jika no source
                string list = Regex.Replace(System.Net.WebUtility.HtmlDecode(isi), "<.*?>", string.Empty);
                File.WriteAllText(@"list.db", list);      //bikin file
            }
            return isi.Contains(key);
        }

        public static string[] search(string sumber) {
            //Olah data dari halaman search result
            string[] hasil = new string[10];
            string[] mentah = potong(sumber, "<div class=\"picSurround\">");
            for (int i = 0; i < 10; i++) {
                string link = pisah(mentah[i + 1], "hoverinfo_trigger\" href=\"", "\" id=\"");
                string tipe = pisah(mentah[i + 1], "width=\"45\">", "</td><td");
                tipe = Regex.Replace(tipe, @"\s+", string.Empty);
                hasil[i] = link + " (" + tipe + ")";
            }
            return hasil;
        }
    }
}
