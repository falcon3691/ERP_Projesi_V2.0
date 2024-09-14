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

namespace ERP_Projesi_V2._0.Ekranlar.Modüller.satinAlma
{
    public partial class satinAlma_2 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public string baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //satinAlma-2 ekranı değişkenleri başlangıcı.
        public int toplamFiyat;
        public DateTime tarih;
        public string girdiCikti;
        public string islemTuru;
        public string kisiKodu;
        //satinAlma-2 ekranı değişkenleri sonu.

        public satinAlma_2(string urunAdi, string tedarikciKodu)
        {
            InitializeComponent();
            textBox1.Text = urunAdi;
            textBox4.Text = tedarikciKodu;
            dateTimePicker1.Value = DateTime.Today;
        }
        //Satın Al butonu
        private void button1_Click(object sender, EventArgs e)
        {
            toplamFiyat = int.Parse(textBox2.Text) * int.Parse(textBox3.Text);
            tarih = dateTimePicker1.Value;
            girdiCikti = "ÇIKTI";
            islemTuru = "NULL";
            kisiKodu = textBox4.Text;
            urunEkle();
            muhasebeKaydi();
            this.Close();
        }
        //Veresiye butonu
        private void button2_Click(object sender, EventArgs e)
        {
            toplamFiyat = int.Parse(textBox2.Text) * int.Parse(textBox3.Text);
            tarih = dateTimePicker1.Value;
            girdiCikti = "GİRDİ";
            islemTuru = "ALACAK";
            kisiKodu = textBox4.Text;
            urunEkle();
            muhasebeKaydi();
            this.Close();
        }

        public void muhasebeKaydi()
        {
            using(SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti, islemTuru, kisiKodu) " +
                                               "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti, @islemTuru, @kisiKodu)";
                using(SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = tarih;
                    komut.Parameters.Add("@toplamFiyat", SqlDbType.Int).Value = toplamFiyat;
                    komut.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = girdiCikti;
                    komut.Parameters.Add("@islemTuru", SqlDbType.NVarChar).Value = islemTuru;
                    komut.Parameters.Add("@kisiKodu", SqlDbType.NVarChar).Value = kisiKodu;

                    try
                    {
                        baglanti.Open();
                        string sonuc = (komut.ExecuteNonQuery() == 1) ? "Muhasebe kaydı oluşturuldu." : "Muhasebe kaydı oluşturulamadı.";
                        Console.Out.WriteLine(sonuc);
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show(hata.ToString(), "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        baglanti.Close();
                    }
                }
            }
        }
        public void urunEkle()
        {
            using(SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "INSERT INTO malzeme(adi, miktari, alisFiyati, satinAlmaTarihi, tedarikciKodu, aciklama) " +
                                               "VALUES(@adi, @miktari, @alisFiyati, @satinAlmaTarihi, @tedarikciKodu, @aciklama)";
                using(SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = textBox1.Text;
                    komut.Parameters.Add("@miktari", SqlDbType.Int).Value = int.Parse(textBox2.Text);
                    komut.Parameters.Add("@alisFiyati", SqlDbType.Int).Value = int.Parse(textBox3.Text);
                    komut.Parameters.Add("@satinAlmaTarihi", SqlDbType.DateTime).Value = tarih;
                    komut.Parameters.Add("@tedarikciKodu", SqlDbType.NVarChar).Value = kisiKodu;
                    komut.Parameters.Add("@aciklama", SqlDbType.NVarChar).Value = textBox5.Text;

                    try
                    {
                        baglanti.Open();
                        string sonuc = (komut.ExecuteNonQuery() == 1) ? "Ürün ekleme başarılı olundu." : "Ürün ekleme başarısız olundu.";
                        Console.Out.WriteLine(sonuc);
                    }
                    catch(Exception hata)
                    {
                        MessageBox.Show(hata.ToString(), "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        baglanti.Close();
                    }
                }
            }
        }
    }
}
