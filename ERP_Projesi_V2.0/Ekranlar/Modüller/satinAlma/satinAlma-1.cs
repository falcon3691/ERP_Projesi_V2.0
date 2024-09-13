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
    public partial class satinAlma_1 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public string baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //satinAlma-1 ekranının değişkenleri başlangıcı
        public string urunAdi = null;
        public string tedarikciKodu = null;
        //satinAlma-1 ekranının değişkenleri sonu

        public satinAlma_1()
        {
            InitializeComponent();
            listele("urunler");
            listele("tedarikci");
        }

        //Ürün Ara butonu
        private void button4_Click(object sender, EventArgs e)
        {
            string deger = textBox1.Text;
            listele("urunler", deger);
        }

        //Tedarikçi Ara butonu
        private void button3_Click(object sender, EventArgs e)
        {
            string deger = textBox2.Text;
            listele("tedarikci", deger);
        }

        public void listele(string sutun = null, string deger = null)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                string sqlKomutu = null;
                string sqlKomutu1 = null;
                if (sutun == "urunler")
                {
                    sqlKomutu = "SELECT DISTINCT VALUE AS urun FROM tedarikci CROSS APPLY STRING_SPLIT(urunler, ',')";
                    sqlKomutu1 = "SELECT id, adi FROM tedarikci WHERE urunler LIKE @deger";

                }
                else if(sutun == "tedarikci")
                {
                    sqlKomutu = "SELECT id, adi FROM tedarikci";
                    sqlKomutu1 = "SELECT DISTINCT VALUE AS urun FROM tedarikci CROSS APPLY STRING_SPLIT(urunler, ',') WHERE adi LIKE @deger";
                }
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    try
                    {
                        baglanti.Open();
                        SqlDataAdapter da = new SqlDataAdapter(komut);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        //tedarikci veri tablosunda olan tüm ürünleri satır satır listeler.
                        //Eğer ürün arama yapılırsa, ürünler arasında benzer olan ürünleri ve
                        //sadece o ürünlere sahip tedarikçileri listeler.
                        if(sutun == "urunler")
                        {
                            if (deger == "")
                            {
                                dataGridView1.DataSource = dt;
                                listele("tedarikci");
                            }
                            else if (deger != null)
                            {
                                DataRow[] satırlar = dt.Select($"urun LIKE '%{deger}%'");
                                if (satırlar.Length > 0)
                                {
                                    SqlCommand komut1 = new SqlCommand(sqlKomutu1, baglanti);
                                    komut1.Parameters.AddWithValue("@deger", '%' + deger + '%');
                                    SqlDataAdapter da1 = new SqlDataAdapter(komut1);
                                    DataTable dt1 = new DataTable();
                                    da1.Fill(dt1);
                                    dataGridView2.DataSource = dt1;
                                    DataTable dt2 = dt.Clone();
                                    foreach (DataRow row in satırlar)
                                        dt2.ImportRow(row);
                                    dataGridView1.DataSource = dt2;

                                }
                                else
                                {
                                    MessageBox.Show("Aranan ürün bulunamadı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    dataGridView1.DataSource = dt;
                                    textBox1.Text = "";
                                }
                            }
                            else
                            {
                                dataGridView1.DataSource = dt;
                                textBox1.Text = "";
                            }
                        }
                        //tedarikci veri tablosunda olan tüm tedarikcilerin id ve adi verilerini listeler.
                        //Eğer tedarikçi adı araması yapılırsa, tedarikçi adı benzer olanlar ve
                        //sadece o tedarikçilerin sahip olduğu ürünler listelenir.
                        else if (sutun == "tedarikci")
                        {
                            if (deger == "")
                            {
                                dataGridView2.DataSource = dt;
                                listele("urunler");
                            }
                            else if (deger != null)
                            {
                                DataRow[] satırlar = dt.Select($"adi LIKE '%{deger}%'");
                                if (satırlar.Length > 0)
                                {
                                    SqlCommand komut1 = new SqlCommand(sqlKomutu1, baglanti);
                                    komut1.Parameters.AddWithValue("@deger", '%' + deger + '%');
                                    SqlDataAdapter da1 = new SqlDataAdapter(komut1);
                                    DataTable dt1 = new DataTable();
                                    da1.Fill(dt1);
                                    dataGridView1.DataSource = dt1;
                                    DataTable dt2 = dt.Clone();
                                    foreach (DataRow row in satırlar)
                                        dt2.ImportRow(row);
                                    dataGridView2.DataSource = dt2;
                                }
                                else
                                {
                                    MessageBox.Show("Aranan tedarikçi bulunamadı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    dataGridView2.DataSource = dt;
                                    textBox1.Text = "";
                                }
                            }
                            else
                            {
                                dataGridView2.DataSource = dt;
                                textBox1.Text = "";
                            }
                        }
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

        //Ürünler listesinde seçilen ürünün adı "urunAdi" değişkenine atılır.
        //Eğer "tedarikciKodu" kodu null değilse, satinAlma-2 ekranı açılır.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                urunAdi = dataGridView1.Rows[index].Cells[0].Value.ToString();
                if (tedarikciKodu != null)
                {
                    satinAlma_2 form = new satinAlma_2(urunAdi, tedarikciKodu);
                    form.Show();
                    urunAdi = null;
                    tedarikciKodu = null;
                }
            }
        }


        //Tedarikçi listesinde seçilen tedarikçinin id'si "tedarikciKodu" değişkenine atılır.
        //Eğer "urunAdi" değişkeni null değilse, satinAlma-2 ekranı açılır.
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                tedarikciKodu = dataGridView2.Rows[index].Cells[0].Value.ToString();
                if (urunAdi != null)
                {
                    satinAlma_2 form = new satinAlma_2(urunAdi, tedarikciKodu);
                    form.Show();
                    urunAdi = null;
                    tedarikciKodu = null;
                }
            }
        }
    }
}
