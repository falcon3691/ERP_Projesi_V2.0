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
    public partial class tedarikci_1 : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Tedarikçi özellikleri değişkenleri başlangıcı
        public String id;
        public String adi;
        public String urunler;
        //Tedarikçi özellikleri değişkenleri sonu

        public tedarikci_1(){
            InitializeComponent();
            listele();
        }

        //Yeni Tedarikçi Ekle butonu
        private void button1_Click(object sender, EventArgs e){
            degiskenleriAta();
            using(SqlConnection baglanti = new SqlConnection(baglantiKodu)){
                String sqlKomutu = "INSERT INTO tedarikci(id, adi, urunler)" +
                                   "VALUES(@id, @adi, @urunler)";
                using(SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
                    komut.Parameters.Add("@adi", SqlDbType.NVarChar).Value = adi;
                    komut.Parameters.Add("@urunler", SqlDbType.NVarChar).Value = urunler;

                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if(sonuc == 1)
                        {
                            Console.Out.WriteLine("Yeni tedarikçi başarılı bir şekilde eklendi.");
                        }
                        else
                        {
                            MessageBox.Show("Yeni tedarikçi eklenemedi.");
                        }
                    }
                    catch (Exception hata){
                        Console.Out.WriteLine("HATA: " + hata.ToString());
                    }
                    finally{
                        baglanti.Close();
                    }
                }
            }
            listele();
        }

        //Tedarikçileri Listele butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
                listele("id", textBox1.Text);
            else if (!String.IsNullOrWhiteSpace(textBox2.Text))
                listele("adi", textBox2.Text);
            else
                listele();
                
        }

        //Ekranda ki textBox'lara null kontrolü yapılır.
        //Eğer null değer değilse, sayfanın başında belirtilen "Tedarikçi Özellikleri" değişkenlerine atama yapılır.
        //Boş olmaması gereken kısımlar boş ise hata mesajları gösterilir.
        public void degiskenleriAta(){
            //Tüm hata mesajlarının toplanacağı liste oluşturulur.
            List<String> hataMesajları = new List<string>();

            //Boş bırakılamayacak değişkenler.
            bool durum = MetinKontrolAta(textBox1.Text, out id);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajları.Add("Tedarikçi Kodu boş bırakılamaz."); }
            durum = MetinKontrolAta(textBox2.Text, out adi);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajları.Add("Tedarikçi Adı boş bırakılamaz."); }
            durum = MetinKontrolAta(textBox3.Text, out urunler);
            //Eğer hata çıkarsa hataMesajları listesine, hata mesajı eklenir.
            if (!durum) { hataMesajları.Add("Tedarikçi Ürünleri boş bırakılamaz."); }

            //Eğer hataMesajı listesi doluysa, tüm hataları tek bir mesaj kutusunda gösterir.
            if (hataMesajları.Count > 0)
            {
                String mesaj = String.Join(Environment.NewLine, hataMesajları);
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

        //Belirlenen sütunda, belirlenen değeri içeren tüm satırlar listelenir.
        //Eğer sütun yada değer girilmezse, otomatik olarak null değer alırlar ve tüm tablo listelenir.
        public void listele(String sutun = null, object deger = null){
            using(SqlConnection baglanti = new SqlConnection(baglantiKodu)){

                try{
                    baglanti.Open();
                    String sqlKomutu = "SELECT * FROM tedarikci ";
                    if (sutun != null && deger != null)
                    {
                        sqlKomutu += $"WHERE {sutun} LIKE @deger";
                    }
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    if (sutun != null && deger != null)
                    {
                        komut.Parameters.AddWithValue("@deger", $"%{deger}%");
                    }
                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    temizle();
                }
                catch (Exception hata){
                    MessageBox.Show("HATA: " + hata.ToString());
                }
                finally{
                    baglanti.Close();
                }
            }
        }

        //textBox'ları temizler.
        public void temizle(){
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        //Seçilen satırda ki id değerini tedarikci_2 ekranına atar ve tedarikci_2 ekranını açar.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if(index >= 0)
            {
                List<String> hataMesajlari = new List<String>();
                bool durum = MetinKontrolAta(dataGridView1.Rows[index].Cells[0].Value.ToString(), out id);
                if (!durum) { hataMesajlari.Add("Tedarikçi Kodu bilgisi alınamadı"); }
                else
                {
                    tedarikci_2 form = new tedarikci_2(id);
                    form.Show();
                }

                if(hataMesajlari.Count > 0)
                {
                    String mesaj = String.Join(Environment.NewLine, hataMesajlari);
                    MessageBox.Show(mesaj, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
