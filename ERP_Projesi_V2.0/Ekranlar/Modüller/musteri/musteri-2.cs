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
            listele();
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

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            Console.Out.WriteLine("Müşteri bilgileri başarılı bir şekilde güncellendi");
                        }
                        else
                        {
                            MessageBox.Show("Müşteri bilgileri güncellenemedi");
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

        //Müşteriyi Sİl butonu
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
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

        //Borç Ödeme butonu
        private void button4_Click(object sender, EventArgs e)
        {
            musteri_3 form = new musteri_3(int.Parse(textBox3.Text.ToString()), textBox2.Text.ToString());
            form.Show();
        }

        //İşlemleri Listele butonu
        private void button3_Click(object sender, EventArgs e)
        {
            listele();
        }

        public void listele()
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "SELECT * FROM muhasebe WHERE kisiKodu = @kisiKodu";
                if (checkBox1.Checked)
                    sqlKomutu += " AND islemTarihi >= @ilkTarih AND islemTarihi <= @sonTarih";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@kisiKodu", SqlDbType.NVarChar).Value = textBox2.Text;
                    if (checkBox1.Checked)
                    {
                        komut.Parameters.Add("ilkTarih", SqlDbType.DateTime).Value = dateTimePicker1.Value.Date;
                        komut.Parameters.Add("sonTarih", SqlDbType.DateTime).Value = dateTimePicker2.Value.Date;
                    }
                    try
                    {
                        baglanti.Open();
                        SqlDataAdapter da = new SqlDataAdapter(komut);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Toplam Borç ve Ödeme Değerlerini Hesaplama
                        decimal toplamBorc = 0;
                        decimal toplamOdeme = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            string islemTuru = row["islemTuru"].ToString();
                            decimal tutar = Convert.ToDecimal(row["toplamFiyat"]); // "tutar" sütunundaki veriyi alıyoruz.

                            if (islemTuru == "BORÇ")
                            {
                                toplamBorc += tutar; // Borçları topluyoruz.
                            }
                            else if (islemTuru == "ÖDEME")
                            {
                                toplamOdeme += tutar; // Ödemeleri topluyoruz.
                            }
                        }

                        // Toplam Borç - Toplam Ödeme Hesaplama
                        decimal sonuc = toplamBorc - toplamOdeme;
                        if (sonuc < 0)
                            sonuc = 0;
                        // Sonucu musteri_2 ekranında bir label'a yazdırma (örneğin: labelToplamBorc)
                        textBox3.Text = sonuc.ToString(); 

                        //Kişinin borç bilgisini güncelleme işlemleri.
                        string sqlKomutu1 = "UPDATE musteri " +
                                            "SET borcu = @borcu " +
                                            "WHERE adi = @adi";
                        using(SqlCommand komut1 = new SqlCommand(sqlKomutu1, baglanti))
                        {
                            komut1.Parameters.Add("@borcu", SqlDbType.Decimal).Value = sonuc;
                            komut1.Parameters.Add("@adi", SqlDbType.NVarChar).Value = textBox2.Text;
                            try
                            {
                                string sonuc1 = (komut1.ExecuteNonQuery() == 1) ? "Müşteri bilgileri başarılı bir şekilde güncellendi." :
                                                                                 "Müşteri bilgileri güncellenemedi.";
                                Console.Out.WriteLine(sonuc1);
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

        //Normalde ikinci dateTimePicker görünmezdir. checkBox seçilince ikinci dateTimePicker görünür olur.
        //Seçim kaldırılınca tekrar görünmez hale gelir.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1Durumu)
            {
                dateTimePicker2.Visible = true;
                checkBox1Durumu = false;
            }
            else
            {
                dateTimePicker2.Visible = false;
                checkBox1Durumu = true;
            }
        }
        
    }
}
