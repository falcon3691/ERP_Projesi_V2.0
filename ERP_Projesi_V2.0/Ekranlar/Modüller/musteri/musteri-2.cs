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
    public partial class musteri_2 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Müşteri özellikleri değişkenleri başlangıcı
        public int id;
        public String adi;
        public decimal borcu;
        //Müşteri özellikleri değişkenleri sonu

        //musteri-2 formu ile alakalı değişkenler başlangıcı.
        public bool checkBox1Durumu = true;
        //musteri-2 formu ile alakalı değişkenler sonu.

        //Bu ekran açıldığında musteri-1 ekranından gelen, dataGridView bilgilerini textBox'lara dolduruyor.
        public musteri_2(int id, String adi, decimal borcu)
        {
            InitializeComponent();
            textBox1.Text = id.ToString();
            textBox2.Text = adi;
            textBox3.Text = borcu.ToString();
        }

        //Bilgileri Güncelle butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "UPDATE musteri " +
                                 "SET adi = @adi " +
                                 "WHERE id = @id";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = textBox2.Text.ToString();
                    komut.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(textBox1.Text);

                    try {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if(sonuc == 1)
                        {
                            Console.Out.WriteLine("Müşteri bilgileri başarılı bir şekilde güncellendi");
                        }
                        else
                        {
                            MessageBox.Show("Müşteri bilgileri güncellenemedi");
                        }
                    }
                    catch (Exception hata) {
                        Console.Out.WriteLine("HATA: " + hata.ToString());
                    }
                    finally{
                        baglanti.Close();
                    }
                }
            }
        }
        
        //Normalde ikinci dateTimePicker görünmezdir. checkBox seçilince ikinci dateTimePicker görünür olur.
        //Seçim kaldırılınca tekrar görünmez hale gelir.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1Durumu) {
                dateTimePicker2.Visible = true;
                checkBox1Durumu = false;
            }
            else
            {
                dateTimePicker2.Visible = false;
                checkBox1Durumu = true;
            }
        }
        
        //Müşteriyi Sİl butonu
        private void button2_Click(object sender, EventArgs e)
        {
            using(SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "DELETE FROM musteri WHERE id = @id";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(textBox1.Text);

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            MessageBox.Show("Müşteri başarılı bir şekilde silindi");
                            this.Close();
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
        }
    }
}
