using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERP_Projesi_V2._0.Ekranlar.Modüller.musteri
{
    public partial class musteri_1 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Müşteri özellikleri değişkenleri başlangıcı
        public int id;
        public String adi;
        public decimal borcu;
        //Müşteri özellikleri değişkenleri sonu

        public musteri_1()
        {
            InitializeComponent();
            listele();
        }

        //Yeni Müşteri Ekle butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "INSERT INTO musteri(adi, borcu) VALUES (@adi, @borcu)";

                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    degiskenleriAta();
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = adi;
                    komut.Parameters.Add("@borcu", SqlDbType.Decimal).Value = borcu;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            Console.Out.WriteLine("Yeni müşteribaşarılı bir şekilde eklendi");
                            temizle();
                        }
                        else
                        {
                            MessageBox.Show("Yeni müşteri eklenemedi.");
                        }
                    }
                    catch (Exception hata)
                    {
                        Console.Out.WriteLine("HATA: " + hata.ToString());
                    }
                    finally
                    {
                        baglanti.Close();
                    }
                }
            }
            listele();
        }

        //Listle butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(textBox1.Text))
                listele();
            else
            {
                adi = textBox1.Text;
                listele("adi", adi);
            }
        }
        //Ekranda ki textBox'a null kontrolü yapılır.
        //Eğer null değer değilse, sayfanın başında belirtilen "Ürün Özellikleri" değişkenine atama yapılır.
        //Boş olmaması gereken kısımlar boş ise hata mesajları gösterilir.
        public void degiskenleriAta()
        {
            //Tüm hata mesajlarının toplanacağı liste oluşturulur.
            List<string> hataMesajlari = new List<string>();
            //Boş bırakılamayacak değişkenler.
            bool durum = MetinKontrolAta(textBox1.Text, out adi);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Adı Soyadı kısmı boş olamaz"); }
            borcu = 0m;
            //Eğer hataMesajı listesi doluysa, tüm hataları tek bir mesaj kutusunda gösterir.
            if (hataMesajlari.Count > 0)
            {
                string mesaj = string.Join(Environment.NewLine, hataMesajlari);
                MessageBox.Show(mesaj, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //textBox'ı temizler.
        public void temizle()
        {
            textBox1.Text = "";
        }

        //Belirlenen sütunda, belirlenen değeri içeren tüm satırlar listelenir.
        //Eğer sütun yada değer girilmezse, otomatik olarak null değer alırlar ve tüm tablo listelenir.
            public void listele(String sutun = null, object deger = null)
            {
                using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
                {
                    try
                    {
                        baglanti.Open();
                        String sqlKomutu = "Select * FROM musteri ";
                        if (sutun != null && deger != null)
                        {
                            sqlKomutu += $"WHERE {sutun} LIKE @deger";
                        }
                        SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                        if (sutun != null && deger != null)
                        {
                            komut.Parameters.AddWithValue("@deger", $"%{deger}%");
                        }
                        SqlDataAdapter da = new SqlDataAdapter(komut);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        temizle();
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show("HATA: " + hata.ToString());
                    }
                    finally
                    {
                        baglanti.Close();
                    }
                }
            }

        //Öncelikle bir metin var mı diye kontrol edilir.
        //Eğer bir metin varsa, belirtilen String değişkene o metni atama yapılır.
        //Yapılan kontrole göre bool bir değer döndürülür.
        public bool MetinKontrolAta(String metin, out String degisken)
        {
            if (!String.IsNullOrWhiteSpace(metin))
            {
                degisken = metin;
                return true;
            }
            else
            {
                degisken = null;
                return false;
            }
        }

        //Seçilen satırda ki bilgileri alır ve musteri-2 ekranına gönderir.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            List<String> hataMesajları = new List<string>();
            if(index >= 0){
                bool durum = int.TryParse(dataGridView1.Rows[index].Cells[0].Value.ToString(), out id);
                if (!durum) { hataMesajları.Add("Seçilen liste elemanından ID değeri alınamadı"); }
                durum = MetinKontrolAta(dataGridView1.Rows[index].Cells[1].Value.ToString(), out adi);
                if (!durum) { hataMesajları.Add("Seçilen liste elemanından Ad Soyad değeri alınamadı"); }
                durum = decimal.TryParse(dataGridView1.Rows[index].Cells[2].Value.ToString(), out borcu);
                if (!durum) { hataMesajları.Add("Seçilen liste elemanından Borç değeri alınamadı"); }
                musteri_2 form = new musteri_2(id, adi, borcu);
                form.Show();
            }
        }

    }
}
