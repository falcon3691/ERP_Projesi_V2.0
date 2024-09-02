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
    public partial class tedarikci_2 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Tedarikçi özellikleri değişkenleri başlangıcı
        public String id;
        public String adi;
        public String urun;
        public int borcu;
        public int alacagi;
        //Tedarikçi özellikleri değişkenleri sonu

        public tedarikci_2(String id)
        {
            InitializeComponent();
            this.id = id;
            sqlDegiskenAta(id);
            textBox1.Text = id;
            textBox2.Text = adi;
            textBox3.Text = borcu.ToString();
            textBox4.Text = alacagi.ToString();
            dataGridViewDoldur(2, id);
        }

        //Tedarikçi Sil butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "DELETE FROM tedarikci WHERE id = @id";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            MessageBox.Show("Tedarikçi başarılı bir şekilde silindi.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Tedarikçi silinemedi.");
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

        //Tedarikçi Güncelle butonu
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "UPDATE tedarikci" +
                                  " SET id = @yeniId, " +
                                       "adi = @adi" +
                                  " WHERE id = @eskiId";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@yeniId", SqlDbType.NVarChar).Value = textBox1.Text;
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = textBox2.Text;
                    komut.Parameters.Add("@eskiId", SqlDbType.NVarChar).Value = id;
                    id = textBox1.Text;
                    adi = textBox2.Text;
                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            MessageBox.Show("Tedarikçi başarılı bir şekilde güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Tedarikçi güncellenemedi.");
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

        //Ürünleri Lsitele butonu
        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                try
                {
                    //Alınan id değerine denk gelen satırda ki ürün bilgilerini alır ve
                    //her ürünü satır satır olacak şekilde düzenleyip dataGridView2'ye kaynak olarak atar.
                    baglanti.Open();
                    String sqlKomutu = "SELECT VALUE AS urun " +
                                       "FROM tedarikci CROSS APPLY STRING_SPLIT(urunler, ',') " +
                                       "WHERE id = @id";
                    if (!String.IsNullOrWhiteSpace(textBox5.Text))
                    {
                        sqlKomutu += " AND VALUE LIKE @urun";
                    }

                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
                    if (!String.IsNullOrWhiteSpace(textBox5.Text))
                    {
                        komut.Parameters.AddWithValue("@urun", $"%{textBox5.Text}%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView2.DataSource = dt;
                    baglanti.Close();
                }

                catch (Exception hata)
                {
                    Console.Out.WriteLine("HATA: " + hata.ToString());
                }
                finally
                {

                }
            }
        }

        //id değerine göre SQL veri tabanında arama yapılır.
        //Eğer aranan değer varsa, sayfanın başında belirtilen "Tedarikçi Özellikleri" değişkenlerine atama yapılır.
        public void sqlDegiskenAta(String id)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                try
                {
                    baglanti.Open();
                    String sqlKomutu = "SELECT * FROM tedarikci WHERE id = @id";
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    baglanti.Close();

                    bool durum = MetinKontrolAta(dt.Rows[0]["adi"].ToString(), out adi);
                    durum = int.TryParse(dt.Rows[0]["borcMiktari"].ToString(), out borcu);
                    durum = int.TryParse(dt.Rows[0]["alacakMiktari"].ToString(), out alacagi);

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

        //id değerine göre SQL veri tabanında arama yapılır.
        //Eğer aranan değer varsa, istenen dataGridView tablosu doldurulur.
        public void dataGridViewDoldur(int deger, String id)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                try
                {
                    switch (deger)
                    {
                        //dataGridView1
                        case 1:
                            break;
                        //dataGridView2
                        case 2:
                            //Alınan id değerine denk gelen satırda ki ürün bilgilerini alır ve
                            //her ürünü satır satır olacak şekilde düzenleyip dataGridView2'ye kaynak olarak atar.
                            baglanti.Open();
                            String sqlKomutu = "SELECT VALUE AS urun FROM tedarikci CROSS APPLY STRING_SPLIT(urunler, ',') WHERE id = @id";
                            SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                            komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

                            SqlDataAdapter da = new SqlDataAdapter(komut);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView2.DataSource = dt;
                            baglanti.Close();
                            break;
                    }
                }

                catch (Exception hata)
                {
                    Console.Out.WriteLine("HATA: " + hata.ToString());
                }
                finally
                {

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

        //Seçilen satırdaki ürün ismini tedarikci_5 ekranına atar ve tedarik_5 ekranını açar.
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;

            if(index >= 0)
            {
                List<String> hataMesajlari = new List<string>();
                bool durum = MetinKontrolAta(dataGridView2.Rows[index].Cells[0].Value.ToString(), out urun);
                if (!durum) { hataMesajlari.Add("Ürün adı alınamadı."); }
                tedarikci_5 form = new tedarikci_5(urun, textBox1.Text);
                form.Show();
                
                if(hataMesajlari.Count > 0)
                {
                    String mesaj = String.Join(Environment.NewLine, hataMesajlari);
                    MessageBox.Show(mesaj, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //dateTimePicker2'yi Visible değerini değiştirir.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                dateTimePicker2.Visible = true;
            else if(checkBox1.Checked == false)
                dateTimePicker2.Visible = false;
        }
    }
}
