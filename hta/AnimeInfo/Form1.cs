using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Anime_Info {
    public partial class Form1 : Form {
        string[] hasilPencarian = new string[10];   //Global var 
        string tmp;       
        public Form1() {
            InitializeComponent();              //Initialize
            BoxPencarian.Select();              //Select search bar
            database.load();                    //Load database
            favorit.load();                     //Load data favorit

            if (File.Exists(@"tmp")) {
                string isi = File.ReadAllText(@"tmp");  //Jika chace (tmp.db) ada maka load anime di sana
                string link = olah.pisah(isi, "[lama]", "[end]");
                refresh(link);  //refresh halaman agar tampil
            }
        }

        private void TombolCari_Click(object sender, EventArgs e) { //Event saat klik cari
            if (BoxPencarian.Text.Length < 3) {
                BoxStatus.Text = "Please input at least 3 chars for searching";
            } else {
                BoxStatus.Text = "Looking Database...";
                string keyword = string.Join("", new string[] { lokasi.malSearch, BoxPencarian.Text });
                string data = request.baca(keyword);        //ambil data pencarian
                if (data == "GAGAL") {
                    BoxJudul.Text = "No internet connection";
                    BoxStatus.Text = "Cannot reach database";
                } else {
                    hasilPencarian = olah.search(data);     //diolah jadi array hasil pencarian
                    string[] teks = new string[10];
                    for (int i = 0; i < 10; i++) {
                        string[] mentahan = hasilPencarian[i].Split('/'); //array elemen url hasil
                        teks[i] = Regex.Replace(mentahan[5], "_", " ");   //Replace, contoh : Death_Note (TV) => Death Note (TV)
                        hasilPencarian[i] = hasilPencarian[i].Split(new string[] { " " }, StringSplitOptions.None)[0];
                    }                                                     //menghilangkan tipe anime, contohnya : (TV)
                    BoxStatus.Text = "Search result for '" + BoxPencarian.Text + "'";
                    BoxHasil1.Text = teks[0];   //Search result tampilkan
                    BoxHasil2.Text = teks[1];
                    BoxHasil3.Text = teks[2];
                    BoxHasil4.Text = teks[3];
                    BoxHasil5.Text = teks[4];
                    BoxHasil6.Text = teks[5];
                    BoxHasil7.Text = teks[6];
                    BoxHasil8.Text = teks[7];
                    BoxHasil9.Text = teks[8];
                    BoxHasil10.Text = teks[9];
                }
            }
        }
        
        private void refreshRelated(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //Event ketika browser related anime selesai di load
            if (BrowserRelated.DocumentText != animeNow.related) {
                //Ketika isinya bukan related anime (artinya user nge-klik link)
                string alamat = BrowserRelated.Url.AbsoluteUri;
                if (alamat.Contains("/anime/")) {
                    alamat = lokasi.malUrl + Regex.Replace(alamat, "about:", String.Empty);
                    refresh(alamat);
                } else {
                    BrowserRelated.DocumentText = animeNow.related;
                }
            }
        }
        private void refreshFavorit(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //Event ketika browser favorit selesai di load
            string fav = favorit.baca();
            if (BrowserFavorit.DocumentText != fav) {
                //Ketika isinya bukan favorit (artinya user nge-klik link)

                string alamat = BrowserFavorit.Url.AbsoluteUri;
                if (alamat.Contains("hapus/")) {            //Ketika user klik link utk hapus
                    string key = alamat.Split('/')[3];
                    favorit.hapus(key);                     //Key berisi id anime
                    refresh(animeNow.url);
                } else if (alamat.Contains("/anime/")) {    //Ketika user klik link anime fav
                    alamat = lokasi.malUrl + Regex.Replace(alamat, "about:", String.Empty);
                    refresh(alamat);
                }
            }
        }

        private void refreshLagu(object sender, WebBrowserDocumentCompletedEventArgs e) {
            if (BrowserLagu.DocumentText != animeNow.DLsong) {
                //Ketika isinya bukan DLsong (artinya user nge-klik link)
                string alamat = BrowserLagu.Url.AbsoluteUri;
                string id = Regex.Replace(alamat, "about:", String.Empty);
                System.Diagnostics.Process.Start(lokasi.thehylia + id);
                BrowserLagu.DocumentText = animeNow.DLsong;
            }
        }
        private void tombolFavorit_Click(object sender, EventArgs e) {
            favorit.add(animeNow.url);
            refresh(animeNow.url);
            BoxPencarian.Select();
        }
        private void button1_Click(object sender, EventArgs e) {    //Saat download di klik
            string linkDL = lokasi.dexAnime + Regex.Replace(animeNow.judul, " ", "_");
            System.Diagnostics.Process.Start(linkDL);
        }

        private void tombolVideo_Click(object sender, EventArgs e) {
            if (animeNow.judul != null) {
                boxVideoStatus.Text = "Loading...";
                string video = ambil.video(BoxKeyword.Text);
                if (video != "NULL") {
                    browserVideo.Navigate(video);
                } else {
                    boxVideoStatus.Text = "No internet connection!";
                }
            } else {
                boxVideoStatus.Text = "Choose the anime first in search or favorites list";
            }
        }

        private void uploadbtn_Click(object sender, EventArgs e) {
            statusSync.Text = "Uploading...";
            statusSync.Update();

            if (boxUsername.Text.Length > 3 && boxPassword.Text.Length > 5) {
                string key = boxUsername.Text + boxPassword.Text;
                if (favorit.upload(key) && database.upload(key)) {
                    statusSync.Text = "Upload success!"+Environment.NewLine+"File is synced in server and secure";
                } else {
                    statusSync.Text = "Upload fail, please contact Naufal";
                }
            } else {
                statusSync.Text = "Username minimum 4 chars and Password minimum 6 chars!";
            }
        }
        private void downloadbtn_Click(object sender, EventArgs e) {            
            statusSync.Text = "Downloading...";
            statusSync.Update();
            if (boxUsername.Text.Length > 3 && boxPassword.Text.Length > 5) {
                string key = boxUsername.Text + boxPassword.Text;
                bool berhasil = request.download("favorit.db", lokasi.Cloud + key + "/favorit.db");
                bool semua = request.download("private.db", lokasi.Cloud + key + "/private.db");
                if (berhasil && semua) {
                    favorit.load();
                    database.load();
                    refresh(animeNow.url);
                    statusSync.Text = "Download success!"+Environment.NewLine+"Favorit and custom data is restored, im recomended you to rebuild DB after this";
                } else {
                    statusSync.Text = "Download failed!, make sure username and password is correct";
                }
            } else {
                statusSync.Text = "Username minimum 4 chars and Password minimum 6 chars!";
            }
        }
        private void backButton_Click(object sender, EventArgs e) {
            boxUsername.Text = "";
            boxPassword.Text = "";
            panelTersembunyi.Visible = false;
            statusSync.Text = tmp;
        }

        private void button1_Click_1(object sender, EventArgs e) {
            tmp = statusSync.Text;
            panelTersembunyi.Visible = true;
        }

        private void refresh(string url) {     //Method refresh interface utama
            BoxJudul.Text = "Loading...";
            BoxJudul.Update();
            database.read(url);
            string path = @"img//" + animeNow.url.Split('/')[4] + ".jpg";	//contoh : @"img/2344.jpg"
            if (File.Exists(path)) {
                BoxGambar.Image = Image.FromFile(path);
            }
            if (favorit.isAnimeFav(animeNow.url)) {
                tombolFavorit.Enabled = false;
                tombolFavorit.Text = "Already in Favorites";
                tombolFavorit.BackColor = SystemColors.Window;
            } else {
                tombolFavorit.Enabled = true;
                tombolFavorit.Text = "Add to Favorites";
                tombolFavorit.BackColor = SystemColors.ActiveCaption;
            }
            if (olah.bisaDiDownload(animeNow.judul)) {
                TombolDL.Enabled = true;
                TombolDL.BackColor = SystemColors.ActiveCaption;
                statusDL.Text = "Download Available !";
            } else {
                TombolDL.Enabled = false;
                TombolDL.BackColor = SystemColors.Window;
                statusDL.Text = "Download Unavailable";
            }
            BoxJudulAlt.Text = animeNow.judulAlt;
            BoxSinopsis.Text = animeNow.sinopsis;
            BoxMember.Text = animeNow.member;
            BoxScore.Text = animeNow.score;
            BoxGenres.Text = animeNow.genre;
            BoxTayang.Text = animeNow.tayang;
            BoxDurasi.Text = animeNow.durasi;
            BoxStudio.Text = animeNow.studios;
            BoxLaguOP.Text = animeNow.laguOP;
            BoxLaguED.Text = animeNow.laguED;
            boxCatatan.Text = animeNow.catatan;
            boxScorePribadi.Value = animeNow.scorePribadi;
            BoxEps.Text = animeNow.eps;
            BrowserRelated.DocumentText = animeNow.related;
            BoxReview.Text = animeNow.review;
            BrowserFavorit.DocumentText = favorit.baca();
            BrowserLagu.DocumentText = animeNow.DLsong;
            BoxJudul.Text = animeNow.judul;
        }

        private void gantiTab(object sender, EventArgs e) { //Method saat ganti tab, ini buat select box isian
            if (tabUtama.SelectedTab == bagianVideo) {
                AcceptButton = tombolVideo;
                BoxKeyword.Select();
            } else {
                AcceptButton = TombolCari;
                BoxPencarian.Select();
            }
        }

        private void boxCatatan_TextChanged(object sender, EventArgs e) { //Ketika user ganti teks notes pribadi
            if (boxCatatan.Text != animeNow.catatan) {
                tombolPrivate.Enabled = true;   
                tombolPrivate.BackColor = Color.LightGreen;
            } else {
                tombolPrivate.Enabled = false;
                tombolPrivate.BackColor = SystemColors.Window;
            }
        }

        private void boxScorePribadi_ValueChanged(object sender, EventArgs e) { //Ketika user ganti score pribadi
            if (boxScorePribadi.Value != animeNow.scorePribadi) {
                tombolPrivate.Enabled = true;
                tombolPrivate.BackColor = Color.LightGreen;
            } else {
                tombolPrivate.Enabled = false;
                tombolPrivate.BackColor = SystemColors.Window;
            }
        }

        private void tombolPrivate_Click(object sender, EventArgs e) {  //Ketika user klik save info pribadi
            animeNow.scorePribadi = boxScorePribadi.Value;
            animeNow.catatan = boxCatatan.Text;
            database.addPrivate();
            tombolPrivate.Enabled = false;
            tombolPrivate.BackColor = SystemColors.Window;
            BoxPencarian.Select();
        }

        private void webLoadSelesai(object sender, EventArgs e) {   //Ketika video selesai diload
            boxVideoStatus.Text = "Video " + BoxKeyword.Text;
        }

        private void buttonDB_Click(object sender, EventArgs e) { //ketika user klik tombol refresh DB
            rebuildDB();
        }

        private void rebuildDB() {      //Method untuk rebuild database
            string tmp = BoxJudul.Text;         //
            string tmp2 = BoxJudulAlt.Text;     // Ingat anime sebelum di refresh=

            BoxJudul.Text = "Updating database....";
            BoxJudulAlt.Text = "This process update all your anime data. dont click anything!";
            BoxJudul.Update();                  // Teks tanda sedang update database
            BoxJudulAlt.Update();               //
            Cursor.Hide();

            string[] list = favorit.bacaPerBaris();     // Baca daftar anime favorit untuk di grab nanti
            File.Delete(@"data.db");            // Hapus database lama
            int panjang = list.Length;

            progressBar1.Visible = true;        //
            statusDB.Visible = true;            // Mempersiapkan loading bar
            progressBar1.Maximum = panjang + 1; // dan juga status bar nya
            progressBar1.Step = 1;              //
            float persen;

            for (int i = 0; i < panjang; i++) {
                persen = (float)i / panjang * 100;
                statusDB.Text = "Process data " + (i + 1) + "/" + panjang + " from server (" + persen.ToString("0.0") + "%)";
                statusDB.Update();              //
                progressBar1.Value = i + 2;     // Update tampilan loading tiap step

                string animenya = olah.pisah(list[i], "<a href=\"", "\" style=\"");
                if (animenya != "-") {
                    string url = lokasi.malUrl + animenya;
                    database.tulisSingkat(url); // Proses utama
                }
            }
            progressBar1.Visible = false;   // Sembunyikan loading bar
            statusDB.Visible = false;       // juga status nya

            BoxJudul.Text = tmp;        //
            BoxJudulAlt.Text = tmp2;    // Kembalikan seperti semula
            Cursor.Show();
            database.load();            // Update isi database yg baru
        }

        //Fungsi yg gak efektif, masih blm tau cara ubahnya, ini buat tombol hasil pencarian
        private void BoxHasil1_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[0]);
            }
        }
        private void BoxHasil2_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[1]);
            }
        }
        private void BoxHasil3_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[2]);
            }
        }
        private void BoxHasil4_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[3]);
            }
        }
        private void BoxHasil5_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[4]);
            }
        }
        private void BoxHasil6_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[5]);
            }
        }
        private void BoxHasil7_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[6]);
            }
        }
        private void BoxHasil8_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[7]);
            }
        }
        private void BoxHasil9_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[8]);
            }
        }
        private void BoxHasil10_Click(object sender, EventArgs e) {
            if (hasilPencarian[0] != null) {
                refresh(hasilPencarian[9]);
            }
        }
    }
}
