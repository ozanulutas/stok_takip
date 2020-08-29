using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Windows.Forms.DataVisualization.Charting;

namespace uzayMadencilik
{
    public partial class Form3 : Form
    {
        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");
        DateTime tarih = DateTime.Now;
        TimeSpan gunEkle = new TimeSpan(5, 0, 0, 0);
        int kulId;
        string ka;

        public Form3(int kulId, string ka)
        {
            InitializeComponent();
            this.kulId = kulId;
            this.ka = ka;
        
        }

        void grafikCiz(string islem, ComboBox cb, Chart chart)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();

                if (islem == "formLoad" || cb.Text == "" || cb.Text == "TÜM SATIŞLAR")
                {
                    komut.CommandText = "SELECT MONTH(TARİH), SUM(MİKTAR) FROM tblSatislar GROUP BY MONTH(TARİH) ORDER BY MONTH(TARİH)";
                }
                else
                {
                    komut.CommandText = "SELECT MONTH(TARİH), SUM(MİKTAR) FROM tblSatislar WHERE MADEN = @maden GROUP BY MONTH(TARİH) ORDER BY MONTH(TARİH)";
                    komut.Parameters.AddWithValue("@maden", cb.Text);
                }
                komut.Connection = bag;
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);

                foreach (var series in chart.Series)
                    series.Points.Clear();

                int i = 0;
                foreach (DataRow sat in tablo.Rows)
                {
                    double miktar = double.Parse(sat[1].ToString());
                    chart.Series["Satışlar"].Points.Add(miktar);

                    chart.Series["Satışlar"].Points[i++].AxisLabel = sat[0].ToString();
                }
                chart.ChartAreas[0].AxisX.Title = "Aylar";
                chart.ChartAreas[0].AxisY.Title = "Satışlar(Ton)";
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        void yaklasanEtkinlik(string tbl, DataGridView dgv)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "SELECT * FROM " + tbl + " WHERE TARİH >= @tarih AND TARİH <= @tarih2";
                komut.Parameters.AddWithValue("@tarih", tarih.ToShortDateString());
                komut.Parameters.AddWithValue("@tarih2", tarih.Add(gunEkle).ToShortDateString());
                komut.Connection = bag;

                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);
                dgv.DataSource = tablo;
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
            }
        }

        void comboBoxAta(ComboBox cb)
        {
            try
            {
                cb.Items.Clear();
                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "SELECT MADEN FROM tblSatislar GROUP BY MADEN";
                komut.Connection = bag;
                bag.Open();
                OleDbDataReader oku = komut.ExecuteReader();
                cb.Items.Add("TÜM SATIŞLAR");
                while (oku.Read())
                {
                    string kayit = oku[0].ToString();
                    cb.Items.Add(kayit);
                }
                oku.Close();
                bag.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        void uyar(NotifyIcon ni)
        {
            ni.Text = "metin";
            ni.BalloonTipTitle = "Hoşgeldin "+ka;
            if (dataGridView1.RowCount - 1 > 0 || dataGridView2.RowCount - 1 > 0)
                ni.BalloonTipText = "BUGÜN YAKLAŞAN ETKİNLİKLERİN VAR!";
            else
                ni.BalloonTipText = "LÜTFEN BİR İŞLEM SEÇ";
            ni.Icon = SystemIcons.Application;
            ni.BalloonTipIcon = ToolTipIcon.Info;
            ni.ShowBalloonTip(1000);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
           
            comboBox1.Text = "TÜM SATIŞLAR";

            yaklasanEtkinlik("tblSeferler", dataGridView1);
            yaklasanEtkinlik("tblSatislar", dataGridView2);
            comboBoxAta(comboBox1);
            grafikCiz("formLoad", comboBox1, chart1);

            uyar(notifyIcon1);
            yenileToolStripMenuItem.Image = Properties.Resources.Refresh_16x;
            programdanÇıkToolStripMenuItem.Image = Properties.Resources.Exit_16x;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            grafikCiz("indexChanged",comboBox1, chart1);            
        }

        private void seferlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void satışlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.ShowDialog();
        }

        private void stokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }

        private void madenlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.ShowDialog();
        }

        private void destinasyonlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.ShowDialog();
        }

        private void kullanıcılarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.ShowDialog();
        }

        private void yenileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object sndr = new object();
            EventArgs ea = new EventArgs();
            Form3_Load(sndr, ea);
        }

        private void programdanÇıkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Form2 form2 = new Form2();
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                form2.sil("tblSeferler", "SEFERID", dataGridView1);                
            }
            else e.Cancel = true;            
        }

        private void dataGridView2_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Form4 form4 = new Form4();
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                form4.sil(dataGridView2);
            }
            else e.Cancel = true;
        }
    }
}
