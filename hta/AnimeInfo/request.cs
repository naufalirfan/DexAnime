using System.Net;
using System.IO;
using System.Text;
 namespace Anime_Info {
    //Bagian ini yang mengurus urusan koneksi & grab data
    //Bagian ini adalah yang memakan waktu eksekusi paling lama gara2 butuh internet
    //Strukturnya mirip semua menggunakan objek HttpWebRequest FtpWebRequest dan WebClient
    class request {
        public static string baca(string Url) {
            try {
                HttpWebRequest baca = (HttpWebRequest)WebRequest.Create(Url);   //generate object
                baca.Method = "GET";                //set method nya GET alias menerima data dari server
                WebResponse respon = baca.GetResponse();    //ambil respon
                StreamReader data = new StreamReader(respon.GetResponseStream(), Encoding.UTF8);                
                string hasil = data.ReadToEnd();    //ubah respon ke string            
                data.Close(); respon.Close();       //tutup
                return hasil;
            } catch {
                return "GAGAL"; //Ga bisa buka url (mungkin ga ada internet, salah url, atau yg lain pokoknya gagal :v
            }
        }
         private static bool ftpUploadFile(string file, string lokasi, string nama) {
            try {
                    //pertama generate object request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://dexanime.com/" + lokasi + "/" + nama);                    
                request.Method = WebRequestMethods.Ftp.UploadFile;  //set property
                request.Credentials = new NetworkCredential("username", "password");             
                byte[] data = Encoding.UTF8.GetBytes(file);         //ambil (byte) data dari file                                                                    
                request.ContentLength = data.Length;                //set property lagi                              
                Stream server = request.GetRequestStream();         //write data dengan stream
                server.Write(data, 0, data.Length);
                server.Close();
                return true;
            } catch {
                return false; //kalau gagal return false
            }
         }
         public static bool upload(string file, string lokasi, string nama) {
            bool hasil = false;
            try {           //Pertama coba bikin directory nya, apakah bisa atau tidak terserah
                            //soalnya kalau directory sudah ada akan terjadi error, tapi ignore saja
                FtpWebRequest requestDir = (FtpWebRequest)WebRequest.Create("ftp://dexanime.com/" + lokasi);
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential("username", "password");
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                requestDir.GetResponse();
            } catch { /* Dibiarkan saja */ } finally {
                hasil = ftpUploadFile(file,lokasi,nama); //Entah membuat direktori berhasil atau tidak tetap upload file
            }
            return hasil; 
            
        }
        public static bool download(string path, string url) {
            try {               //Untuk mendownload file dari url kedalam local path (path juga termasuk nama file)
                byte[] data;
                using (WebClient client = new WebClient()) {
                    data = client.DownloadData(url);    //proses download
                }
                File.WriteAllBytes(path, data);         //proses penulisan ke hardisk
                return true;
            } catch {
                return false;                           //jika gagal false
            }
            
        }
     }
}
