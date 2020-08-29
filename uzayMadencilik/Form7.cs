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

namespace uzayMadencilik
{
    public partial class Form7 : Form
    {
        Form2 form2 = new Form2();
        int id;
        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");

        public Form7()
        {
            InitializeComponent();
        }

        void listele(string islem)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();
                if (islem == "listele" || (comboBox1.Text == "" && textBox1.Text == ""))
                {
                    komut.CommandText = "SELECT * FROM tblDestinasyonlar";
                }
                else
                {
                    komut = new OleDbCommand();
                    string sorgu = "SELECT * FROM tblDestinasyonlar WHERE ";
                    if (comboBox1.Text != "")
                    {
                        sorgu += "DESTİNASYON = @destinasyon AND ";
                        komut.Parameters.AddWithValue("@destinasyon", comboBox1.Text);
                    }
                    if (textBox1.Text != "")
                    {
                        sorgu += "TÜR = @tur AND ";
                        komut.Parameters.AddWithValue("@tur", textBox1.Text);
                    }
                    sorgu = sorgu.Substring(0, sorgu.Length - 5);
                    komut.CommandText = sorgu;
                }
                komut.Connection = bag;
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);
                dataGridView1.DataSource = tablo;
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
            }
        }

        void ekle_guncelle(string islem)
        {
            try
            {
                string desitnasyon = comboBox1.Text.ToUpper();
                string tur = textBox1.Text.ToUpper();

                if (islem == "ekle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "INSERT INTO tblDestinasyonlar(DESTİNASYON, TÜR) VALUES(@desitnasyon, @tur)";
                    komut.Parameters.AddWithValue("@desitnasyon", desitnasyon);
                    komut.Parameters.AddWithValue("@tur", tur);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                    form2.comboBoxAta(comboBox1, "tblDestinasyonlar");
                }

                if (islem == "guncelle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblDestinasyonlar SET DESTİNASYON=@desitnasyon, TÜR=@tur WHERE DESTİNASYONID=" + id;
                    komut.Parameters.AddWithValue("@desitnasyon", desitnasyon);
                    komut.Parameters.AddWithValue("@tur", tur);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                    form2.comboBoxAta(comboBox1, "tblDestinasyonlar");
                }
            }
            catch (Exception hata)
            {
                if (hata.Message == "Giriş dizesi doğru biçimde değildi.") MessageBox.Show("Lütfen tüm alanları doldurun.");
                else MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        void formSifirla()
        {
            comboBox1.Text = "";
            textBox1.Text = "";
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            form2.comboBoxAta(comboBox1, "tblDestinasyonlar");
            listele("liste");
        }
        //ARAMA VE EKLEME
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "ARA")
            {
                listele("ara");
                formSifirla();
            }
            if (button1.Text == "EKLE")
            {
                ekle_guncelle("ekle");
                listele("listele");
                formSifirla(); ;
            }            
        }
        //HIZLI ARAMA
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string aranan = toolStripTextBox1.Text;                

                OleDbCommand komut = new OleDbCommand();
                komut = new OleDbCommand("SELECT * FROM tblDestinasyonlar WHERE DESTİNASYON=@aranan OR TÜR=@aranan", bag);
                komut.Parameters.AddWithValue("@aranan", aranan);                
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);
                dataGridView1.DataSource = tablo;
                formSifirla(); 
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
            }
        }
        //LİSTELEME VE GÜNCELLEME
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "LİSTELE")
            {
                listele("listele");
                formSifirla();
            }
            if (button2.Text == "GÜNCELLE")
            {
                ekle_guncelle("guncelle");
                listele("listele");
                formSifirla();
            }
        }
        //LİSTELEME
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            listele("listele");
            formSifirla();
        }
        //SİLME İŞLEMİ
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                form2.sil("tblDestinasyonlar", "DESTİNASYONID", dataGridView1);
                formSifirla();
            }
            else e.Cancel = true;
        }
        //SİLME İŞLEMİ
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                form2.sil("tblDestinasyonlar", "DESTİNASYONID", dataGridView1);
                listele("listele");
                formSifirla();
            }
        }

        private void aRAVeLİSTELEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "ARA";
            button2.Text = "LİSTELE";
            button1.Image = Properties.Resources.Search_noHalo_16x;
            button2.Image = Properties.Resources.ListView_16x;
            listele("listele");
            formSifirla();
        }

        private void eKLEVeGÜNCELLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "EKLE";
            button2.Text = "GÜNCELLE";
            button1.Image = Properties.Resources.Add_16x;
            button2.Image = Properties.Resources.GoToNextModified_16x;
            listele("listele");
            formSifirla();
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (button2.Text == "GÜNCELLE")
            {
                if (dataGridView1.CurrentCell.RowIndex < dataGridView1.RowCount - 1)
                {
                    int indis = dataGridView1.CurrentCell.RowIndex;
                    id = int.Parse(dataGridView1[0, indis].Value.ToString());
                }
                try
                {
                    string sorgu = "SELECT DESTİNASYON,TÜR FROM tblDestinasyonlar where DESTİNASYONID=@id";
                    OleDbCommand komut = new OleDbCommand(sorgu, bag);
                    komut.Parameters.AddWithValue("@id", id);
                    bag.Open();
                    OleDbDataReader oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        comboBox1.Text = oku[0].ToString();
                        textBox1.Text = oku[1].ToString();
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
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                string sorgu;
                sorgu = "SELECT * FROM tblDestinasyonlar";
                OleDbCommand komut = new OleDbCommand(sorgu, bag);
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);

                Font font = new Font("Arial", 10, FontStyle.Bold);
                SolidBrush firca = new SolidBrush(Color.Black);
                Pen kalem = new Pen(Color.Black);

                e.Graphics.DrawString("DEST.ID", font, firca, 60, 50);
                e.Graphics.DrawString("DESTİNASYON", font, firca, 155, 50);
                e.Graphics.DrawString("TÜR", font, firca, 280, 50);
                e.Graphics.DrawLine(kalem, 50, 75, 770, 75);

                font = new Font("Arial", 10, FontStyle.Regular);

                for (int i = 0, y = 85; i < tablo.Rows.Count; y += 20, i++)
                {
                    e.Graphics.DrawString(tablo.Rows[i][0].ToString(), font, firca, 60, y);
                    e.Graphics.DrawString(tablo.Rows[i][1].ToString(), font, firca, 155, y);
                    e.Graphics.DrawString(tablo.Rows[i][2].ToString(), font, firca, 280, y);
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        private void rAPROLAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void programdanÇıkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void kullanıcılarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.ShowDialog();
        }
    }
}
