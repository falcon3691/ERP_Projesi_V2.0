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

namespace ERP_Projesi_V2._0.Ekranlar.Modüller
{
    public partial class satis : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public string baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Ekran değişkenleri başlangıcı.
        public int id;
        public int miktari;
        public decimal toplamFiyat = 0;
        public decimal fiyat = 0;
        public string adi;
        DataTable dt2 = new DataTable();
        public bool ilkUrun = true;
        //Ekran değişkenleri sonu

        public satis()
        {
            InitializeComponent();
            listele();
            comboBoxDoldur();
            //Fiş listesinin sütunlarının ayarlanması.
            dt2.Columns.Add("Ürün Adı", typeof(string));
            dt2.Columns.Add("Adet", typeof(int));
            dt2.Columns.Add("Fiyatı", typeof(decimal));
            dt2.Columns.Add("Toplam Fiyatı", typeof(decimal));
        }
        //Satış Yap butonu
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                //Satışın yapıldığı tarihi, işlem tutarı ve işlemin "GİRDİ" olduğu muhasebe veri tablosuna kayıt edilir.
                string sqlKomutu1 = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti) " +
                                                "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti)";
                using (SqlCommand komut1 = new SqlCommand(sqlKomutu1, baglanti))
                {
                    komut1.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = DateTime.Now;
                    komut1.Parameters.Add("@toplamFiyat", SqlDbType.Int).Value = toplamFiyat;
                    komut1.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = "GİRDİ";
                    try
                    {
                        baglanti.Open();
                        string sonuc = (komut1.ExecuteNonQuery() == 1) ? "Muhasebe işlemi başarıyla kaydedildi." : "Muhasebe işlemi kaydedilemedi.";
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

                int i = 0;
                //Satış yapıldıktan sonra kalan ürün miktarlarını veri tabanına kayıt eder.
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[2].Value != null)
                    {
                        string sqlKomutu = "UPDATE malzeme " +
                                           "SET miktari = @miktari " +
                                           "WHERE id = @id";
                        using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                        {
                            komut.Parameters.Add("@id", SqlDbType.Int).Value = row.Cells[0].Value;
                            komut.Parameters.Add("@miktari", SqlDbType.Int).Value = row.Cells[2].Value;
                            try
                            {
                                baglanti.Open();
                                string sonuc = (komut.ExecuteNonQuery() == 1) ? "Ürün miktar bilgisi değiştirildi" : " Ürün miktar bilgisi değiştirilemedi.";
                                Console.Out.WriteLine(i.ToString() + ". " + sonuc);
                                i++;
                                temizle();
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
            }
        }

        //Fişi Temizle butonu
        private void button2_Click(object sender, EventArgs e)
        {
            //Fiş'te bulunan ürünlerin "Adet" miktarlarını, Ürünler'de bulunan ürünün "miktar" değerine ekler.
            foreach (DataGridViewRow row1 in dataGridView2.Rows)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value == row1.Cells[0].Value)
                    {
                        if (row.Cells[2].Value != null && row1.Cells[1].Value != null)
                            row.Cells[2].Value = int.Parse(row.Cells[2].Value.ToString()) + int.Parse(row1.Cells[1].Value.ToString());
                    }
                }
            }
            temizle();
        }

        //Veresiye butonu
        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                //Satışın yapıldığı tarih ve saati, işlem tutarı, işlemin "GİRDİ" ve "BORÇ" olduğu ve
                //comboBox1'de seçili kişinin adı muhasebe veri tablosuna kayıt edilir.
                string sqlKomutu1 = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti, islemTuru, kisiKodu) " +
                                                "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti, @islemTuru, @kisiKodu)";
               
                using (SqlCommand komut1 = new SqlCommand(sqlKomutu1, baglanti))
                {
                    komut1.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = DateTime.Now.Date;
                    komut1.Parameters.Add("@toplamFiyat", SqlDbType.Int).Value = toplamFiyat;
                    komut1.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = "GİRDİ";
                    komut1.Parameters.Add("@islemTuru", SqlDbType.NVarChar).Value = "BORÇ";
                    komut1.Parameters.Add("@kisiKodu", SqlDbType.NVarChar).Value = comboBox1.SelectedValue;
                    try
                    {
                        baglanti.Open();
                        string sonuc = (komut1.ExecuteNonQuery() == 1) ? "Muhasebe işlemi başarıyla kaydedildi." : "Muhasebe işlemi kaydedilemedi.";
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
                
                //Kişi adına göre arama yapılır ve kişinin borç bilgisi alınır.
                //Ardından borç bilgisine toplam fiyat tutarı eklenir ve kişinin borcu güncellenir.
                string sqlKomutu2 = "SELECT borcu FROM musteri WHERE adi = @adi";
                using (SqlCommand komut2 = new SqlCommand(sqlKomutu2, baglanti))
                {
                    komut2.Parameters.Add("@adi", SqlDbType.NVarChar).Value = comboBox1.SelectedValue;
                    try
                    {
                        baglanti.Open();
                        SqlDataAdapter da = new SqlDataAdapter(komut2);
                        DataTable dt2 = new DataTable();
                        da.Fill(dt2);
                        dt2.Rows[0][0] = int.Parse(dt2.Rows[0][0].ToString()) + toplamFiyat;
                        string sqlKomutu3 = "UPDATE musteri " +
                                            "SET borcu = @borcu " +
                                            "WHERE adi = @adi";
                        SqlCommand komut3 = new SqlCommand(sqlKomutu3, baglanti);
                        komut3.Parameters.Add("@borcu", SqlDbType.Int).Value = dt2.Rows[0][0];
                        komut3.Parameters.Add("@adi", SqlDbType.NVarChar).Value = comboBox1.SelectedValue;
                        string sonuc = (komut3.ExecuteNonQuery() == 1) ? "Borç bilgisi güncellendi." :
                                                                            "Borç bilgisi güncellenemedi.";
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

                int i = 0;
                //Satış yapıldıktan sonra kalan ürün miktarlarını veri tabanına kayıt eder.
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[2].Value != null)
                    {
                        string sqlKomutu = "UPDATE malzeme " +
                                           "SET miktari = @miktari " +
                                           "WHERE id = @id";
                        using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                        {
                            komut.Parameters.Add("@id", SqlDbType.Int).Value = row.Cells[0].Value;
                            komut.Parameters.Add("@miktari", SqlDbType.Int).Value = row.Cells[2].Value;
                            try
                            {
                                baglanti.Open();
                                string sonuc = (komut.ExecuteNonQuery() == 1) ? "Ürün miktar bilgisi değiştirildi" :
                                                                                " Ürün miktar bilgisi değiştirilemedi.";
                                Console.Out.WriteLine(i.ToString() + ". " + sonuc);
                                i++;
                                temizle();
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
            }
        }

        //Listeyi Yenile butonu
        private void button4_Click(object sender, EventArgs e)
        {
            comboBoxDoldur();
        }

        //Yeni Müşteri Ekle butonu
        private void button5_Click(object sender, EventArgs e)
        {
            musteri.musteri_1 form = new musteri.musteri_1();
            form.Show();
        }

        public void temizle()
        {
            button1.Enabled = false;
            button3.Enabled = false;
            dataGridView1.Refresh();
            dt2.Clear();
            toplamFiyat = 0;
            label5.Text = toplamFiyat.ToString();
        }

        //dataGridView1'e malzeme veri tablosunda bulunan ürün bilgileri eklenir.
        //Veriler eklenince "alisFiyati" sütununda ki değerler 1.10 katsayısı ile çarpılır ve
        //"Fiyatı" isimli sonradan oluşturulup eklenen sütununa eklenir.
        public void listele(string sutun = null, object deger = null)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "SELECT id, adi, miktari, alisFiyati FROM malzeme ";
                if (sutun != null && deger != null)
                {
                    sqlKomutu += $"WHERE {sutun} = @deger ";
                }
                try
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // Yeni bir sütun ekleme (Fiyat sütunu)
                    dt.Columns.Add("Fiyat", typeof(decimal));

                    // "alisFiyati" değerini %10 arttırarak yeni sütuna ekleme
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal alisFiyati = Convert.ToDecimal(row["alisFiyati"]);
                        decimal fiyat = alisFiyati * 1.10M; // %10 arttırma
                        row["Fiyat"] = Math.Round(fiyat, 2); // Fiyatı 2 basamağa yuvarlama
                    }
                    dataGridView1.DataSource = dt;
                    // İstemediğiniz sütunları gizlemek (eğer gerekirse)
                    dataGridView1.Columns["alisFiyati"].Visible = false;
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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

        //dataGridView2'ye, Ürün listesinden seçilen ürün bilgilerini ekler.
        public void listele2(string adi, decimal fiyat)
        {
            DataRow row = dt2.NewRow();
            row["Ürün Adı"] = adi;
            row["Adet"] = 1;
            row["Fiyatı"] = fiyat;
            row["Toplam Fiyatı"] = fiyat;
            dt2.Rows.Add(row);
            dataGridView2.DataSource = dt2;
        }

        //comboBox'a musteri veri tablosunda ki "adi" sütunun verileri eklenir.
        //Veriler eklendikten sonra 0.indekse "Yeni Müşteri Ekle" seçeneği eklenir.
        public void comboBoxDoldur()
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = "SELECT adi FROM musteri";
                try
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "adi";
                    comboBox1.ValueMember = "adi";
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

        //Ürün listesinde ki ürünlerden biri seçilirse, ürün bilgileri fişe yazılır.
        //Eğer daha önce eklenen bir ürün tekrar seçilirse, yeni satıra eklenmez Toplam Fiyat artar.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button1.Enabled = true;
            button3.Enabled = true;
            int index = e.RowIndex;
            if (index >= 0)
            {
                if (int.Parse(dataGridView1.Rows[index].Cells[2].Value.ToString()) > 0)
                {
                    //Ürünler listesinde seçilen ürünün miktarını 1 azaltır.
                    dataGridView1.Rows[index].Cells[2].Value = int.Parse(dataGridView1.Rows[index].Cells[2].Value.ToString()) - 1;
                    adi = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    if (ilkUrun)
                    {
                        fiyat = Math.Round(decimal.Parse(dataGridView1.Rows[index].Cells["Fiyat"].Value.ToString()), 2);
                        listele2(adi, fiyat);
                        dataGridView2.Refresh();
                        ilkUrun = false;
                        toplamFiyat += fiyat;
                        label5.Text = toplamFiyat.ToString();
                    }
                    else
                    {
                        DataGridViewRow row1 = null;
                        bool urunVarMi = false;
                        //Fiş listesinde ki satırlar arasında tek tek dolaşır ve
                        //Ürünler listesinde seçilen ürünün "adi" değerine göre işlem yapar.
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            //Eğer "adi" değeri varsa "Adet" sayısını 1 arttırır ve "Toplam Fiyat" değerine "Fiyat" değerini ekler.
                            if (row.Cells["Ürün Adı"].Value != null && row.Cells["Ürün Adı"].Value.ToString() == adi)
                            {
                                urunVarMi = true;
                                row1 = row;
                            }
                        }
                        //Eğer "adi" değeri varsa "Adet" sayısını 1 arttırır ve "Toplam Fiyat" değerine "Fiyat" değerini ekler.
                        if (urunVarMi)
                        {
                            fiyat = decimal.Parse(row1.Cells[2].Value.ToString());
                            row1.Cells["Adet"].Value = int.Parse(row1.Cells["Adet"].Value.ToString()) + 1;
                            row1.Cells[3].Value = decimal.Parse(row1.Cells[3].Value.ToString()) + fiyat;
                            dataGridView2.Refresh();
                            toplamFiyat += fiyat;
                            label5.Text = toplamFiyat.ToString();
                        }
                        //Eğer "adi" değeri yoksa yeni değerler ile yeni satır ekler.
                        else
                        {
                            fiyat = Math.Round(decimal.Parse(dataGridView1.Rows[index].Cells["Fiyat"].Value.ToString()), 2);
                            listele2(adi, fiyat);
                            dataGridView2.Refresh();
                            toplamFiyat += fiyat;
                            label5.Text = toplamFiyat.ToString();
                        }

                    }
                }
            }
        }
        //Fiş listesinde tıklanan ürün, listeden çıkarılır.
        //Çıkarılan ürünün, Ürün listesinde ki miktarı eski haline gelir.
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value == dataGridView2.Rows[index].Cells[0].Value)
                    {
                        if (int.Parse(dataGridView2.Rows[index].Cells[1].Value.ToString()) == 1)
                        {
                            row.Cells[2].Value = int.Parse(row.Cells[2].Value.ToString()) + 1;
                            toplamFiyat -= decimal.Parse(dataGridView2.Rows[index].Cells[2].Value.ToString());
                            label5.Text = toplamFiyat.ToString();
                            dataGridView2.Rows.RemoveAt(index);
                            break;

                        }
                        else if (int.Parse(dataGridView2.Rows[index].Cells[1].Value.ToString()) >= 2)
                        {
                            row.Cells[2].Value = int.Parse(row.Cells[2].Value.ToString()) + 1;
                            dataGridView2.Rows[index].Cells[1].Value = int.Parse(dataGridView2.Rows[index].Cells[1].Value.ToString()) - 1;
                            dataGridView2.Rows[index].Cells[3].Value = decimal.Parse(dataGridView2.Rows[index].Cells[3].Value.ToString()) -
                                                                        decimal.Parse(dataGridView2.Rows[index].Cells[2].Value.ToString());
                            toplamFiyat -= decimal.Parse(dataGridView2.Rows[index].Cells[2].Value.ToString());
                            label5.Text = toplamFiyat.ToString();
                            break;
                        }
                    }
                }
            }
        }
    }
}
