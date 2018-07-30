using System.IO;
using System.Linq;
using System.Net;

namespace Anime_Info {
    class database {        /*Bagian pengelolaan database
                              masih kurang efektif dalam penulisan*/
        private static string file = @"data.db";
        private static string DBdata;
        public static string privatefile = @"private.db";
        private static string privateData;

        public static void load() {
            check(file);
            check(privatefile);
            DBdata = File.ReadAllText(file);  //load db
            privateData = File.ReadAllText(privatefile);
        }

        public static void read(string anime) {//untuk mengeluarkan data dengan url anime dari db
            string id = getID(anime);  //ambil id anime dari url anime
            string tmp = olah.pisah(DBdata, "[" + id + "]", "[" + id + " end]");//ambil bagian id
            if (tmp != "-") {      /* Jika benar ada id di database maka ambil
                                      data-datanya dan disimpan di static class animeNow */                
                animeNow.judul = decode(olah.pisah(tmp, "[judul]", "[end]"));
                animeNow.judulAlt = decode(olah.pisah(tmp, "[judulAlt]", "[end]"));
                animeNow.sinopsis = decode(olah.pisah(tmp, "[sinopsis]", "[end]"));
                animeNow.member = decode(olah.pisah(tmp, "[member]", "[end]"));
                animeNow.score = decode(olah.pisah(tmp, "[score]", "[end]"));
                animeNow.genre = decode(olah.pisah(tmp, "[genre]", "[end]"));
                animeNow.tayang = decode(olah.pisah(tmp, "[tayang]", "[end]"));
                animeNow.durasi = decode(olah.pisah(tmp, "[durasi]", "[end]"));
                animeNow.studios = decode(olah.pisah(tmp, "[studios]", "[end]"));
                animeNow.laguOP = decode(olah.pisah(tmp, "[laguOP]", "[end]"));
                animeNow.laguED = decode(olah.pisah(tmp, "[laguED]", "[end]"));
                animeNow.eps = decode(olah.pisah(tmp, "[eps]", "[end]"));
                animeNow.related = decode(olah.pisah(tmp, "[related]", "[end]"));
                animeNow.review = decode(olah.pisah(tmp, "[review]", "[end]"));
                animeNow.DLsong = decode(olah.pisah(tmp, "[DLsong]","[end]"));
                selesai(anime);
            } else {    /* Jika tidak ada di database maka menulis data di database */
                bool berhasil = ambil.load(anime);
                if (berhasil) {
                    menulis(anime);
                    selesai(anime);
                    DBdata = File.ReadAllText(file);    // Update data
                } else {
                    animeNow.judul = "No internet connection!";                    
                }
            }           //menyimpan ke file tmp tanda anime terakhir dibuka
        }

        private static void selesai(string anime) { //fungsi ini yang selalu dijalankan selesai load data
            File.WriteAllText(@"tmp", "[lama]" + anime + "[end]"); //tanda sudah di load
            animeNow.url = anime;                   // tanda anime sudah berubah                    
            readPrivate();
        }

        public static void tulisSingkat(string anime) {   // Cuma menulis ke database
            bool berhasil = ambil.load(anime);
            if (berhasil) {
                menulis(anime);
            } else {                
                animeNow.judul = "No internet connection!";
            }
        }

        private static void menulis(string anime) {   //Bagian ini hanya menulis data ke database
            grabdata();
            string id = getID(anime);
            string lokasiGambar = @"img//" + anime.Split('/')[4] + ".jpg";
            if (!File.Exists(lokasiGambar)) {
                request.download(lokasiGambar, ambil.gambar());
            }
            using (StreamWriter db = new StreamWriter(file, true)) {
                db.Write("[" + id + "]");
                db.Write("[judul]" + encode(animeNow.judul) + "[end]");
                db.Write("[judulAlt]" + encode(animeNow.judulAlt) + "[end]");
                db.Write("[sinopsis]" + encode(animeNow.sinopsis) + "[end]");
                db.Write("[member]" + encode(animeNow.member) + "[end]");
                db.Write("[score]" + encode(animeNow.score) + "[end]");
                db.Write("[genre]" + encode(animeNow.genre) + "[end]");
                db.Write("[tayang]" + encode(animeNow.tayang) + "[end]");
                db.Write("[durasi]" + encode(animeNow.durasi) + "[end]");
                db.Write("[studios]" + encode(animeNow.studios) + "[end]");
                db.Write("[laguOP]" + encode(animeNow.laguOP) + "[end]");
                db.Write("[laguED]" + encode(animeNow.laguED) + "[end]");
                db.Write("[eps]" + encode(animeNow.eps) + "[end]");
                db.Write("[related]" + encode(animeNow.related) + "[end]");
                db.Write("[review]" + encode(animeNow.review) + "[end]");
                db.Write("[DLsong]" + encode(animeNow.DLsong) + "[end]");
                db.WriteLine("[" + id + " end]");
            }
        }

        private static void grabdata() {
            animeNow.judul = ambil.judul();
            animeNow.judulAlt = ambil.judulAlt();
            animeNow.sinopsis = ambil.sinopsis();
            animeNow.member = ambil.member();
            animeNow.score = ambil.score();
            animeNow.genre = ambil.genre();
            animeNow.tayang = ambil.tayang();
            animeNow.durasi = ambil.durasi();
            animeNow.studios = ambil.studios();
            animeNow.laguOP = ambil.laguOP();
            animeNow.laguED = ambil.laguED();
            animeNow.eps = ambil.eps();
            animeNow.related = ambil.related();
            animeNow.review = ambil.review();
            animeNow.DLsong = ambil.downloadLagu();
        }

        public static void addPrivate() {
            if (privateData.Contains(animeNow.url)) {
                string[] semuaBaris = File.ReadAllLines(privatefile); //baca
                string[] ygBaru = semuaBaris.Where(baris => !baris.Contains(animeNow.url)).ToArray();   //salin yg perlu
                File.WriteAllLines(privatefile, ygBaru);  //tulis lagi salinannya
            }
            using (StreamWriter db = new StreamWriter(privatefile, true)) {
                db.Write("[" + animeNow.url + "]");
                db.Write("[catatan]" + encode(animeNow.catatan) + "[end]");
                db.Write("[score]" + animeNow.scorePribadi + "[end]");
                db.WriteLine("naufalirfan");
            }
            privateData = File.ReadAllText(privatefile);

        }

        public static void readPrivate() {
            string tmp = olah.pisah(privateData, "[" + animeNow.url + "]", "naufalirfan");
            if (tmp != "-") {
                animeNow.catatan = decode(olah.pisah(tmp, "[catatan]", "[end]"));
                animeNow.scorePribadi = int.Parse(olah.pisah(tmp,"[score]","[end]"));
            }else {
                animeNow.catatan = "Write your notes about this anime here...";
                animeNow.scorePribadi = 0;
            }
        }

        public static bool upload(string id) {
            return request.upload(privateData, id, "private.db");
        }

        public static void check(string namaFile) {
            if (!File.Exists(namaFile)) {
                var bikin = File.Create(namaFile);    //jika blm ada file db maka dibuat dulu
                bikin.Close();
            }
        }

        private static string getID(string url) {   //Ambil ID dari url anime
            return url.Split('/')[4];
        }

        private static string encode(string teks) {
            var teksBytes = System.Text.Encoding.UTF8.GetBytes(teks);
            return System.Convert.ToBase64String(teksBytes);
        }

        private static string decode(string encodedData) {
            var encodedDataBytes = System.Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(encodedDataBytes);
        }

    }
}
