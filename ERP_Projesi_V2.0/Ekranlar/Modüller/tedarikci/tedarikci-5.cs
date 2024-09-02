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
    public partial class tedarikci_5 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Ürün özellik değişkeni başlangıcı
        public String urun;
        public String id;
        public String urunler;
        //Ürün özellik değişkeni sonu
        public tedarikci_5(String urun, String id)
        {
            InitializeComponent();
            this.urun = urun;
            this.id = id;
            textBox1.Text = urun;
        }

        //Ürünü Düzelt butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                //Alınan id değerine denk gelen satırda ki ürün bilgilerini alır ve
                //her ürünü satır satır olacak şekilde düzenleyip dataGridView2'ye kaynak olarak atar.
                baglanti.Open();
                String sqlKomutu = "SELECT urunler FROM tedarikci WHERE id = @id";
                SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);
                urunler = dt.Rows[0]["urunler"].ToString();
                baglanti.Close();
                if (urunler.Contains(urun + ','))
                    urunIslemi(urunler.Replace(urun + ',', textBox1.Text + ','), "güncellen");
                else if (urunler.Contains(',' + urun))
                    urunIslemi(urunler.Replace(',' + urun, ',' + textBox1.Text), "güncellen");
                else if (urunler.Contains(urun))
                    urunIslemi(urunler.Replace(urun, textBox1.Text), "güncellen");
            }
        }
        //Ürünü Sil butonu
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                //Alınan id değerine denk gelen satırda ki ürün bilgilerini alır ve
                //her ürünü satır satır olacak şekilde düzenleyip dataGridView2'ye kaynak olarak atar.
                baglanti.Open();
                String sqlKomutu = "SELECT urunler FROM tedarikci WHERE id = @id";
                SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);
                urunler = dt.Rows[0]["urunler"].ToString();
                baglanti.Close();
                if (urunler.Contains(urun + ','))
                    urunIslemi(urunler.Replace(urun + ',', ""), "silin");
                else if (urunler.Contains(',' + urun))
                    urunIslemi(urunler.Replace(',' + urun, ""), "silin");
                else if (urunler.Contains(urun))
                    urunIslemi(urunler.Replace(urun, ""), "silin");
            }
        }
        public void urunIslemi(String urunler, String mesaj)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                //Alınan id değerine ve urun değerini denk gelen satırda ki ürün bilgilerini değiştiriri
                String sqlKomutu = "UPDATE tedarikci " +
                                   "SET urunler = @urun " +
                                   "WHERE id = @id AND urunler = @urunler";
                using(SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    Console.Out.WriteLine(id + " / " + urunler +" / " + this.urunler);
                    komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
                    komut.Parameters.Add("@urun", SqlDbType.NVarChar).Value = urunler;
                    komut.Parameters.Add("@urunler", SqlDbType.NVarChar).Value = this.urunler;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            MessageBox.Show("Ürün başarılı bir şekilde " + mesaj + "di.");
                            if(mesaj == "silin")
                                this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ürün "+ mesaj + "emedi.");
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
