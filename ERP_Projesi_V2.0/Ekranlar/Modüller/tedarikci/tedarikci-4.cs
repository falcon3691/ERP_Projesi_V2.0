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

namespace ERP_Projesi_V2._0.Ekranlar.Modüller.tedarikci
{
    public partial class tedarikci_4 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //tedarikci_4 ekranı değişkenleri başlangıcı.
        public string kisiKodu;
        //tedarikci_4 ekranı değişkenleri sonu.
        public tedarikci_4(decimal alacakMiktari, string kisiKodu)
        {
            InitializeComponent();
            textBox1.Text = alacakMiktari.ToString();
            textBox2.Text = alacakMiktari.ToString();
            this.kisiKodu = kisiKodu;
        }

        //Ödeme Yap butonu
        private void button1_Click(object sender, EventArgs e)
        {
            decimal sonuc = decimal.Parse(textBox1.Text) - decimal.Parse(textBox3.Text);
            if(sonuc < 0)
            {
                MessageBox.Show(Math.Abs(sonuc).ToString() + " tutarında para üstü alınacak.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = "0";
            }
            else
            {
                textBox2.Text = sonuc.ToString();
            }
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti, islemTuru, kisiKodu) " +
                                               "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti, @islemTuru, @kisiKodu)";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = DateTime.Today.Date;
                    komut.Parameters.Add("@toplamFiyat", SqlDbType.Decimal).Value = decimal.Parse(textBox3.Text);
                    komut.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = "ÇIKTI";
                    komut.Parameters.Add("@islemTuru", SqlDbType.NVarChar).Value = "ÖDEME";
                    komut.Parameters.Add("@kisiKodu", SqlDbType.NVarChar).Value = kisiKodu;
                    try
                    {
                        baglanti.Open();
                        string sonuc1 = (komut.ExecuteNonQuery() == 1) ? "Ödeme işlemi başarılı bir şekilde gerçekleşti." :
                                                                        "Ödeme işlemi gerçekleşemedi.";
                        Console.Out.WriteLine(sonuc1);
                        this.Close();
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
