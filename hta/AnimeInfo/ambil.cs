using System;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace Anime_Info {
    /*-------------- Bagian ini mengolah data mentah dari internet -----------
      ------------------- ya gitu aja penjelasan umum nya :v -----------------*/
    class ambil {
        private static string data;

        public static bool load(string anime) {
            data = request.baca(anime);
            if (data == "GAGAL") {
                animeNow.judul = "No internet connection!";
                return false;
            } else {
                return true;
            }
        }

        public static string review() {
            if (data.Contains("<div style=\"float: left; display: none;")) {
                string[] tmp = olah.potong(data, "<div style=\"float: left; display: none; ");
                string hasil = olah.pisah(tmp[1], "</div>", "<div id=");
                hasil = WebUtility.HtmlDecode(Regex.Replace(hasil, "<.*?>", string.Empty)).Substring(30);
                return hasil;
            } else {
                return "Saat ini belum ada review";
            }
        }

        public static string judulAlt() {
            string hasil = olah.pisah(data, "English:</span>", "</div><div");
            if (data.Contains("Synonyms:</span>")) {
                string tambahan = olah.pisah(data, "dark_text\">Synonyms:</span>", "</div>");
                hasil = string.Join("|", new string[] { hasil, tambahan });
            }
            hasil = WebUtility.HtmlDecode(hasil);
            return hasil;
        }

        public static string video(string keyword) {
            //Ngambil video dari youtube dengan keyword tertentu
            string hasil = "NULL";
            string isi = request.baca(lokasi.youtubeSearch + animeNow.judul + " " + keyword);
            if (isi.Contains("data-video-ids=\"")) {
                hasil = lokasi.youtubeEmbed + olah.pisah(isi, "data-video-ids=\"", "\"><span");
            }
            return hasil;
        }

        public static string downloadLagu() {
            string isi = request.baca(lokasi.thehyliaSearch + animeNow.judul);
            if (isi.Contains("matching albums for")) {
                isi = olah.potong(isi, "matching albums for")[1];
                isi = olah.pisah(isi, "<p align=\"left\">", "</p>");
                isi = Regex.Replace(isi, lokasi.thehylia, string.Empty);
            } else {
                isi = "Network Failure";
            }
            return isi;
        }

        /*-----------------Yang dibawah kommen ini isinya sama semua----------------
        -----------intinya cuma ngambil data tertentu dari data mentah------------*/
        public static string related() {
            return olah.pisah(data, "Related Anime</h2>", "<br><h2><div");
        }
        public static string judul() {
            return olah.pisah(data, "\"h1\"><span itemprop=\"name\">", "</span></h1>");
        }
        public static string sinopsis() {
            string mentah = olah.pisah(data, "<span itemprop=\"description\">", "</span>");
            return Regex.Replace(WebUtility.HtmlDecode(mentah), "<.*?>", string.Empty);
        }
        public static string member() {
            string mentah = olah.pisah(data, "Members:</span>", "</div>");
            return Regex.Replace(mentah, @"\s+", "");
        }
        public static string score() {
            return olah.pisah(data, "ratingValue\">", "</span><sup>");
        }
        public static string tayang() {
            return olah.pisah(data, "Aired:</span>", "</div>").Substring(3);
        }
        public static string durasi() {
            return olah.pisah(data, "Duration:</span>", "</div>").Substring(3);
        }
        public static string eps() {
            return olah.pisah(data, "Episodes:</span>", "</div>").Substring(3);
        }
        public static string studios() {
            return olah.tanpaHtml(data, "Studios:</span>", 10);
        }
        public static string genre() {
            return olah.tanpaHtml(data, "Genres:</span>", 3);
        }
        public static string gambar() {
            return olah.pisah(data, "property=\"og:image\" content=\"", "\"><meta name=");
        }

        //------------------------------------Bagian Lagu--------------------------------------------
        public static string laguOP() {
            return ambilLagu(data, "opnening");
        }
        public static string laguED() {                   //Karena ambilLaguOP & ambilLaguED mirip strukturnya
            return ambilLagu(data, "ending");    //Jadi ku gabung bikin method ambilLagu
        }
        private static string ambilLagu(string teks, string jenis) {
            string hasil2 = "";
            string mentah = olah.pisah(teks, "js-theme-songs " + jenis + "\">", "</div>");
            if (mentah != "-") {
                string[] hasil = olah.potong(mentah, "theme-song\">");
                for (int i = 1; i < hasil.Length; i++) {
                    string sebuahLagu = WebUtility.HtmlDecode(Regex.Replace(hasil[i], "<.*?>", string.Empty));
                    hasil2 += Regex.Replace(sebuahLagu, "<span class=\"", string.Empty)+ Environment.NewLine;
                }
                return hasil2;
            } else {
                return "Maaf, lagu tidak ditemukan";
            }
        }
    }
}
