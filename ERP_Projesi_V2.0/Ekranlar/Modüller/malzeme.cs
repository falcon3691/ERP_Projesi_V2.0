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

namespace ERP_Projesi_V2._0
{
    public partial class malzeme : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";
        //Ürün Özellikleri değişkenleri başlangıcı.
        public int id;
        public int miktari;
        public decimal alisFiyati;
        public String adi;
        public String birimi;
        public String kategorisi;
        public String tedarikciKodu;
        public String aciklama;
        public DateTime satinAlmaTarihi;
        public DateTime sonKullanmaTarihi;
        //Ürün Özellikleri değişkenleri sonu.
        public malzeme()
        {
            InitializeComponent();
            //Kullanılacak birim çeşitleri belirlenir ve liste comboBox1'in kaynağı olarak belirlenir.
            List<String> birimler = new List<string> {"Kilogram(Kg)", "Litre(L)", "Adet", "Paket", "Kutu", "Şişe"};
            comboBox1.DataSource = birimler;
            //Kullanılacak kategoriler belirlenir ve liste comboBox2'in kaynağı olarak belirlenir.
            List<String> kategoriler = new List<string> {"Gıda Maddeleri", "Süt ve SÜt Ürünleri", "Kahvaltılıklar", "İçecekler", "Atıştırmalıklar", "Temizlik Ürünleri", "Kişisel Bakım Ürünleri", "Konserve ve Hazır Yiyecekler"};
            comboBox2.DataSource = kategoriler;
            //malzeme veri tablosu listelenir
            listele();

        }

        //Ekle butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {

                String sqlKomutu = "INSERT INTO malzeme(adi, miktari, birimi, kategorisi, alisFiyati, satinAlmaTarihi, sonKullanmaTarihi, tedarikciKodu, aciklama) " +
                                          "VALUES (@adi, @miktari, @birimi, @kategorisi, @alisFiyati, @satinAlmaTarihi, @sonKullanmaTarihi, @tedarikciKodu, @aciklama)";

                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    degiskenleriAta();
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = adi;
                    komut.Parameters.Add("@miktari", SqlDbType.Int).Value = miktari;
                    komut.Parameters.Add("@birimi", SqlDbType.NVarChar).Value = birimi;
                    komut.Parameters.Add("@kategorisi", SqlDbType.NVarChar).Value = kategorisi;
                    komut.Parameters.Add("@alisFiyati", SqlDbType.Decimal).Value = alisFiyati;
                    komut.Parameters.Add("@satinAlmaTarihi", SqlDbType.DateTime).Value = satinAlmaTarihi;
                    komut.Parameters.Add("@sonKullanmaTarihi", SqlDbType.DateTime).Value = sonKullanmaTarihi;
                    komut.Parameters.Add("@tedarikciKodu", SqlDbType.NVarChar).Value = tedarikciKodu;
                    komut.Parameters.Add("@aciklama", SqlDbType.NVarChar).Value = aciklama;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            Console.Out.WriteLine("Ürün başarılı bir şekilde eklendi");
                            temizle();
                        }
                        else
                        {
                            MessageBox.Show("Ürün eklenemedi.");
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
        //Güncelle butonu
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "UPDATE malzeme " +
                                   "SET adi = @adi, " +
                                       "miktari = @miktari, " +
                                       "birimi = @birimi, " +
                                       "kategorisi = @kategorisi, " +
                                       "alisFiyati = @alisFiyati, " +
                                       "satinAlmaTarihi = @satinAlmaTarihi, " +
                                       "sonKullanmaTarihi = @sonKullanmaTarihi, " +
                                       "tedarikciKodu = @tedarikciKodu, " +
                                       "aciklama = @aciklama " +
                                   "WHERE id=@id";

                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    degiskenleriAta();
                    bool durum = int.TryParse(textBox1.Text, out id); if (!durum) { MessageBox.Show("Ürün ID  boş bırakılamaz."); }
                    komut.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = adi;
                    komut.Parameters.Add("@miktari", SqlDbType.Int).Value = miktari;
                    komut.Parameters.Add("@birimi", SqlDbType.NVarChar).Value = birimi;
                    komut.Parameters.Add("@kategorisi", SqlDbType.NVarChar).Value = kategorisi;
                    komut.Parameters.Add("@alisFiyati", SqlDbType.Decimal).Value = alisFiyati;
                    komut.Parameters.Add("@satinAlmaTarihi", SqlDbType.DateTime).Value = satinAlmaTarihi;
                    komut.Parameters.Add("@sonKullanmaTarihi", SqlDbType.DateTime).Value = sonKullanmaTarihi;
                    komut.Parameters.Add("@tedarikciKodu", SqlDbType.NVarChar).Value = tedarikciKodu;
                    komut.Parameters.Add("@aciklama", SqlDbType.NVarChar).Value = aciklama;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            Console.Out.WriteLine("Ürün başarılı bir şekilde güncellendi");
                            temizle();
                        }
                        else
                        {
                            MessageBox.Show("Ürün güncellenemedi.");
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
        //Sil butonu
        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {

                String sqlKomutu = "DELETE FROM malzeme WHERE id=@id";

                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    if (!String.IsNullOrWhiteSpace(textBox1.Text) && int.TryParse(textBox1.Text, out id))
                    {
                        komut.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        try
                        {
                            baglanti.Open();
                            int sonuc = komut.ExecuteNonQuery();
                            if (sonuc == 1)
                            {
                                Console.Out.WriteLine("Ürün başarılı bir şekilde silindi");
                                temizle();
                            }
                            else
                            {
                                MessageBox.Show("Ürün silinemedi.");
                            }
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
                    else
                    {
                        MessageBox.Show("Ürünü silmek için ID'sini girmelisiniz.");
                    }
                }
            }
            listele();
        }
        //Hepsini Listele butonu
        private void button5_Click(object sender, EventArgs e)
        {
            listele();
        }
        //Ürünü Ara butonu
        private void button4_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text) && int.TryParse(textBox1.Text, out id))
            {
                listele("id", id);
            }
            else if (!String.IsNullOrWhiteSpace(textBox7.Text))
            {
                adi = textBox7.Text.ToString();
                listele("adi", adi);
            }
            else
            {
                MessageBox.Show("Ürünün ID'si veya Adı'nı boş bırakamazsınız.");
            }
        }
        //dataGridView üzerindeki seçilen elemanın değerlerini,
        //ekranda bulunan textBox, comboBox ve dateTimePicker'lara değer olarak atılır.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;

            if(index >= 0)
            {
                DataGridViewRow satır = dataGridView1.Rows[index];
                textBox1.Text = satır.Cells[0].Value.ToString();
                textBox7.Text = satır.Cells[1].Value.ToString();
                textBox6.Text = satır.Cells[2].Value.ToString();
                comboBox1.SelectedItem = satır.Cells[3].Value.ToString();
                comboBox2.SelectedItem = satır.Cells[4].Value.ToString();
                textBox3.Text = satır.Cells[5].Value.ToString();
                dateTimePicker1.Value = DateTime.Parse(satır.Cells[6].Value.ToString());
                dateTimePicker2.Value = DateTime.Parse(satır.Cells[7].Value.ToString());
                textBox9.Text = satır.Cells[8].Value.ToString();
                textBox10.Text = satır.Cells[9].Value.ToString();
            }
        }

        //Tüm textBox'ları temizler, comboBox'ları 0.index'e ayarlar ve dateTimePicker'ları bugünün tarihine ayarlar.
        public void temizle() {
            textBox1.Text = "";
            textBox3.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }
        //Belirlenen sütunda, belirlenen değeri içeren tüm satırlar listelenir.
        //Eğer sütun yada değer girilmezse, otomatik olarak null değer alırlar ve tüm tablo listelenir.
        public void listele(String sutun = null, object deger = null)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu)){
                try
                {
                    baglanti.Open();
                    String sqlKomutu = "Select * FROM malzeme";
                    if(sutun != null && deger != null)
                    {
                        sqlKomutu += $"WHERE {sutun} LIKE @deger";
                    }   
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    if (sutun != null && deger != null)
                    {
                        komut.Parameters.AddWithValue("@deger" , $"%{deger}%");
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

        //Ekranda ki textBox, comboBox ve dateTimePicker'lara null kontrolü yapılır.
        //Eğer null değer değillerse, sayfanın başında belirtilen "Ürün Özellikleri" değişkenlerine atama yapılır.
        //Boş olmaması gereken kısımlar boş ise hata mesajları gösterilir.
        public void degiskenleriAta()
        {
            //Tüm hata mesajlarının toplanacağı liste oluşturulur.
            List<string> hataMesajlari = new List<string>();
            //Boş bırakılamayacak değişkenler.
            bool durum = int.TryParse(textBox6.Text, out miktari);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Ürün Miktarı boş bırakılamaz"); }
            durum = decimal.TryParse(textBox3.Text, out alisFiyati);
            Console.Out.WriteLine("Değişken atada: " + alisFiyati);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Alış Fiyatı boş bırakılamaz"); }
            durum = MetinKontrolAta(textBox7.Text, out adi);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Ürün Adı boş bırakılamaz"); }
            durum = MetinKontrolAta(textBox9.Text, out tedarikciKodu);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Tedarikçi Kodu boş bırakılamaz"); }
            durum = DateTimeKontrolAta(dateTimePicker1, out satinAlmaTarihi);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajlari.Add("Satın Alma Tarihi boş bırakılamaz"); }
            //Boş bırakılabilen değişkenler.
            durum = ComboBoxKontrolAta(comboBox1, out birimi);
            durum = ComboBoxKontrolAta(comboBox2, out kategorisi);
            durum = MetinKontrolAta(textBox10.Text, out aciklama);
            durum = DateTimeKontrolAta(dateTimePicker2, out sonKullanmaTarihi); 
            //Eğer hataMesajı listesi doluysa, tüm hataları tek bir mesaj kutusunda gösterir.
            if (hataMesajlari.Count > 0)
            {
                string mesaj = string.Join(Environment.NewLine, hataMesajlari);
                MessageBox.Show(mesaj, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        //Öncelikle dateTimePicker'ın değeri null mu diye kontrol edilir.
        //Eğer null değilse, belirtilen DateTime değişkene o değeri atama yapılır.
        //Yapılan kontrole göre bool bir değer döndürülür.
        public bool DateTimeKontrolAta(DateTimePicker dateTimePicker, out DateTime degisken)
        {
            if (dateTimePicker.Value != null)
            {
                degisken = dateTimePicker.Value;
                return true;
            }
            else {
                degisken = default(DateTime);    
                return false; 
            }
        }
        //Öncelikle comboBox'ın değeri null mu diye kontrol edilir.
        //Eğer null değilse, belirtilen String değişkene o değeri atama yapılır.
        //Yapılan kontrole göre bool bir değer döndürülür.
        public bool ComboBoxKontrolAta(ComboBox comboBox, out String degisken)
        {
            if (comboBox.SelectedItem != null)
            {
                degisken = comboBox.SelectedItem.ToString();
                return true;
            }
            else
            {
                degisken = null;
                return false;
            }
        }
    }
}
