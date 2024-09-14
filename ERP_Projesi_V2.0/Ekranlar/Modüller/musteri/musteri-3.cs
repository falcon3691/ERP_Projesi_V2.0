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
    public partial class musteri_3 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public string baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //musteri_3 ekranı değişkenleri başlangıcı.
        public string kisiKodu;
        //musteri_3 ekranı değişkenleri sonu.

        public musteri_3(int toplamBorcu, string kisiKodu)
        {
            InitializeComponent();
            this.kisiKodu = kisiKodu;
            textBox1.Text = toplamBorcu.ToString();
            textBox2.Text = toplamBorcu.ToString();
        }
        //Borç Öde butonu
        private void button1_Click(object sender, EventArgs e)
        {
            decimal kalan = decimal.Parse(textBox1.Text) - decimal.Parse(textBox3.Text);
            if(kalan < 0)
            {
                MessageBox.Show(Math.Abs(kalan).ToString() + " TL para üstü verilecek.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                kalan = 0m;
                textBox2.Text = kalan.ToString();
            }
            else
            {
                textBox2.Text = kalan.ToString();
            }
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti, islemTuru, kisiKodu) " +
                                               "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti, @islemTuru, @kisiKodu)";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = DateTime.Today;
                    komut.Parameters.Add("@toplamFiyat", SqlDbType.Decimal).Value = decimal.Parse(textBox3.Text);
                    komut.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = "GİRDİ";
                    komut.Parameters.Add("@islemTuru", SqlDbType.NVarChar).Value = "ÖDEME";
                    komut.Parameters.Add("@kisiKodu", SqlDbType.NVarChar).Value = kisiKodu;
                    try
                    {
                        baglanti.Open();
                        string sonuc = (komut.ExecuteNonQuery() == 1) ? "Borç ödeme işlemi başarılı bir şekilde gerçekleşti." :
                                                                        "Borç ödeme işlemi gerçekleşemedi.";
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
