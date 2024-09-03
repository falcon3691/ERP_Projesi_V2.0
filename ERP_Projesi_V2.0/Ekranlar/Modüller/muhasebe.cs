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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ERP_Projesi_V2._0.Ekranlar.Modüller
{
    public partial class muhasebe : Form
    {
        //SQL veri tabanı bağlantı kodu.
        public String baglantiKodu = "Data Source=DESKTOP-HSH38D0\\SQLEXPRESS;Initial Catalog=KKP_V2;Integrated Security=True";

        //Muhasebe değişkenleri başlangıcı.
        public DateTime tarih;
        public int toplamFiyat;
        public String girdiCikti;
        public String islemTuru;
        //Muhasebe değişkenleri sonu.

        public muhasebe()
        {
            InitializeComponent();
            listele();
            AdjustDataGridViewSize();
        }

        private void AdjustDataGridViewSize()
        {
            int totalWidth = 20;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                totalWidth += column.Width;
            }

            dataGridView1.Width = totalWidth;
        }
        //İşlemleri Listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            listele();
        }

        //Borçları Listele butonu
        private void button3_Click(object sender, EventArgs e)
        {
            listele("islemTuru", "BORÇ");
        }

        //Alacakları Listele butonu
        private void button2_Click(object sender, EventArgs e)
        {
            listele("islemTuru", "ALACAK");
        }
        
        //Girdileri Listele butonu
        private void button5_Click(object sender, EventArgs e)
        {
            listele("girdiCikti", "GİRDİ");
        }

        //Çıktıları Listele butonu
        private void button6_Click(object sender, EventArgs e)
        {
            listele("girdiCikti", "ÇIKTI");
        }

        //Listeyi Raporla butonu
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Varsayılan dosya yolu ve adı ayarlama
            string masaustuYolu = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string varsayilanDosyaAdi = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

            saveFileDialog.InitialDirectory = masaustuYolu;
            saveFileDialog.FileName = varsayilanDosyaAdi;
            saveFileDialog.Filter = "PDF Dosyaları (*.pdf)|*.pdf|Tüm Dosyalar (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dosyaYolu = saveFileDialog.FileName;

                try
                {
                    //ExportDataGridViewToPdf(dataGridView1, dosyaYolu);
                    CreatePdf(dosyaYolu, dataGridView1);
                    MessageBox.Show("Dosya başarıyla kaydedildi: " + dosyaYolu);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        //checkBox1'in değerine göre, dateTimePicker2'nin Visible değerini değiştirir.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
        }

        //degerEkle fonksiyonu sadece veri tabanına deneme amaçlı veri eklemek için kullanılıyor.
        //Diğer modüllerden veri ekleme işlemleri tamamlanınca silinmesi gerekiyor.
        public void degerEkle(DateTime tarih, int toplamFiyat, String girdiCikti, String islemTuru)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "INSERT INTO muhasebe(islemTarihi, toplamFiyat, girdiCikti, islemTuru) " +
                                                 "VALUES(@islemTarihi, @toplamFiyat, @girdiCikti, @islemTuru)";
                using (SqlCommand komut = new SqlCommand(sqlKomutu, baglanti))
                {
                    komut.Parameters.Add("@islemTarihi", SqlDbType.DateTime).Value = tarih;
                    komut.Parameters.Add("@toplamFiyat", SqlDbType.Int).Value = toplamFiyat;
                    komut.Parameters.Add("@girdiCikti", SqlDbType.NVarChar).Value = girdiCikti;
                    komut.Parameters.Add("@islemTuru", SqlDbType.NVarChar).Value = islemTuru;
                    try
                    {
                        baglanti.Open();
                        int sonuc = komut.ExecuteNonQuery();
                        if (sonuc == 1)
                        {
                            Console.Out.WriteLine("İşlem eklendi.");
                            listele();
                        }
                        else
                            Console.Out.WriteLine("İşlem eklenemedi.");
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show(hata.ToString());
                    }
                    finally
                    {
                        baglanti.Close();
                    }
                }
            }
        }

        public void listele(String sutun = null, String deger = null)
        {
            using (SqlConnection baglanti = new SqlConnection(baglantiKodu))
            {
                String sqlKomutu = "SELECT * FROM muhasebe";
                if (sutun != null && deger != null)
                {
                    sqlKomutu += $" WHERE {sutun} = @deger";
                    if(checkBox1.Checked)
                        sqlKomutu += " AND islemTarihi >= @deger1 AND islemTarihi <= @deger2";
                }
                else if(checkBox1.Checked)
                    sqlKomutu += " WHERE islemTarihi >= @deger1 AND islemTarihi <= @deger2";

                try
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand(sqlKomutu, baglanti);
                    if (sutun != null && deger != null)
                        komut.Parameters.Add("@deger", SqlDbType.NVarChar).Value = deger;
                    if (checkBox1.Checked)
                    {
                        komut.Parameters.Add("@deger1", SqlDbType.DateTime).Value = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                        komut.Parameters.Add("@deger2", SqlDbType.DateTime).Value = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    baglanti.Close();
                }
                catch (Exception hata)
                {
                    MessageBox.Show(hata.ToString());
                }
                finally
                {
                    baglanti.Close();
                }

            }
        }

        //dataGridView'ın içeriğini bir PDF dosyasına yazar, kullanıcıdan dosya yolu ve dosya adını belirtebileceği bir ekran açılır.
        //Eğer dosya yolu veya dosya adı değiştirilmezse, varsayılan olarak dosya yolu masaüstü ve dosya adı "gününTarihi_Saat" olacak
        //şekilde ayarlanır. 
        //Oluşturulan dosya kayıt edilir.
        public void CreatePdf(string filePath, DataGridView dataGridView)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (Document document = new Document(PageSize.A4))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    writer.PageEvent = new PdfPageEvents();

                    document.Open();

                    // Tarih bilgisini ekle
                    string todayDate = DateTime.Now.ToString("dd.MM.yyyy"); 
                    BaseFont bfArialUniCode = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bfArialUniCode, 12, iTextSharp.text.Font.BOLD);
                    Paragraph dateParagraph = new Paragraph(todayDate + "\n\n", font);
                    dateParagraph.Alignment = Element.ALIGN_LEFT;
                    document.Add(dateParagraph);

                    // Sütun isimlerini ekle
                    PdfPTable table = new PdfPTable(dataGridView.ColumnCount);
                    table.WidthPercentage = 100;

                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, font));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                    }

                    // Çizgi ekle
                    table.HeaderRows = 1;
                    document.Add(table);

                    // Satırları ekle
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Value?.ToString() ?? string.Empty, font));
                                table.AddCell(pdfCell);
                            }
                        }
                    }

                    // Son çizgi ekle
                    document.Add(table);
                    document.Add(new Paragraph("\n")); // Boşluk bırak

                    document.Close();
                }
            }
        }

    }
    //Oluşturulan PDF dosyasında her sayfanın sonuna sayfa sayısını yazdırmak için kullanılır.
    public class PdfPageEvents : PdfPageEventHelper
    {
        private int pageNumber = 1;

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // Sayfa numarasını yazmak için PdfContentByte kullanımı
            PdfContentByte canvas = writer.DirectContent;
            Phrase phrase = new Phrase($"Sayfa: {pageNumber}", FontFactory.GetFont("Helvetica", 10, iTextSharp.text.Font.BOLD));
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, phrase, document.PageSize.Width - 40, 30, 0);

            // Sayfa numarasını arttır
            pageNumber++;
        }
    }
}
