using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP_Projesi_V2._0.Ekranlar
{
    public partial class anaSayfa : Form
    {
        public anaSayfa()
        {
            InitializeComponent();
        }

        private void mALZEMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            malzeme form = new malzeme();
            form.Show();
            form.MdiParent = this;
        }

        private void sATIŞToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modüller.satis form = new Modüller.satis();
            form.MdiParent = this;
            form.Show();
        }

        private void sATINALMAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modüller.satinAlma.satinAlma_1 form = new Modüller.satinAlma.satinAlma_1();
            form.MdiParent = this;
            form.Show();
        }

        private void mUHASEBEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modüller.muhasebe form = new Modüller.muhasebe();
            form.MdiParent = this;
            form.Show();
        }

        private void mÜŞTERİToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modüller.musteri.musteri_1 form = new Modüller.musteri.musteri_1();
            form.MdiParent = this;
            form.Show();
        }

        private void tEDARİKÇİToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modüller.tedarikci.tedarikci_1 form = new Modüller.tedarikci.tedarikci_1();
            form.MdiParent = this;
            form.Show();
        }
    }
}
