using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Anime_Info {
    class favorit {
        public static string file = @"favorit.db";
        private static string data;
        private static string[] dataPerBaris;

        public static void load() {
            database.check(file);           //Membuat file jika tidak ada
            dataPerBaris = File.ReadAllLines(file);  //load db
            data = File.ReadAllText(file);
        }

        public static string baca() {
            return data;
        }

        public static string[] bacaPerBaris() {
            return dataPerBaris;
        }

        public static void add(string link) {   //menambah favorit
            string teks = link.Split('/')[5];
            teks = Regex.Replace(teks, "_", " ");      //pemformatan data agar sesuai
            link = Regex.Replace(link, lokasi.malUrls, string.Empty);
            link = Regex.Replace(link, lokasi.malUrl, string.Empty);
            using (StreamWriter daftar = new StreamWriter(file, true)) {    //nulis data
                daftar.WriteLine("<a href=\"" + link + "\" style=\"color:black;text-decoration:none;\">" + teks + "</a>  <a href=\"/hapus" + link + "\" style=\"color:grey;text-decoration:none;\">(remove)</a><br>");
                daftar.Close();
            }
            load();                     //update db
        }
        public static void hapus(string link) {             //menghapus baris yang berisi link tertentu
            string[] ygBaru = dataPerBaris.Where(baris => !baris.Contains(link)).ToArray();   //salin baris yg perlu saja
            File.WriteAllLines(file, ygBaru);               //tulis lagi salinannya
            load();                                         //update db
        }

        public static bool isAnimeFav(string animeUrl) { //cek anime sekarang sudah di favorit atau tdk
            string[] url = animeUrl.Split('/');
            return data.Contains(url[4] + "/" + url[5]); //contoh value : "1535/Death_Note" <<kasih id & nama biar yakin
        }

        public static bool upload(string id) {
            return request.upload(data, id, "favorit.db");
        }
    }
}
