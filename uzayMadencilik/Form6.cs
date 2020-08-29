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
    public partial class Form6 : Form
    {
        Form2 form2 = new Form2();
        int id;
        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");

        public Form6()
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
                    komut.CommandText = "SELECT * FROM tblMadenler";
                }
                else
                {
                    komut = new OleDbCommand();
                    string sorgu = "SELECT * FROM tblMadenler WHERE ";
                    if (comboBox1.Text != "")
                    {
                        sorgu += "MADEN = @maden AND ";
                        komut.Parameters.AddWithValue("@maden", comboBox1.Text);
                    }
                    if (textBox1.Text != "")
                    {
                        if (radioButton1.Checked == true)
                            sorgu += "FİYAT >= @fiyat AND ";
                        else if (radioButton2.Checked == true)
                            sorgu += "FİYAT < @fiyat AND ";
                        else
                            sorgu += "FİYAT = @fiyat AND ";
                        komut.Parameters.AddWithValue("@fiyat", textBox1.Text);
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
                string maden = comboBox1.Text.ToUpper();
                int fiyat = int.Parse(textBox1.Text);

                if (islem == "ekle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "INSERT INTO tblMadenler(MADEN, FİYAT) VALUES(@maden, @fiyat)";
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Parameters.AddWithValue("@fiyat", fiyat);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                    form2.comboBoxAta(comboBox1, "tblMadenler");
                }

                if (islem == "guncelle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblMadenler SET MADEN=@maden, FİYAT=@fiyat WHERE MADENID=" + id;
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Parameters.AddWithValue("@fiyat", fiyat);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                    form2.comboBoxAta(comboBox1, "tblMadenler");
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

        private void Form6_Load(object sender, EventArgs e)
        {
            form2.comboBoxAta(comboBox1, "tblMadenler");
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
                formSifirla();
            }
            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }
        //HIZLI ARAMA
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string aranan = toolStripTextBox1.Text;
                int sayi;

                OleDbCommand komut = new OleDbCommand();
                if (int.TryParse(aranan, out sayi))
                    komut = new OleDbCommand("SELECT * FROM tblMadenler WHERE FİYAT=@aranan", bag);
                else
                    komut = new OleDbCommand("SELECT * FROM tblMadenler WHERE MADEN=@aranan", bag);
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
            radioButton1.Checked = false;
            radioButton2.Checked = false;
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
                form2.sil("tblMadenler", "MADENID", dataGridView1);
                formSifirla();
            }
            else e.Cancel = true;
        }
        //SİLME
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                form2.sil("tblMadenler", "MADENID", dataGridView1);
                listele("listele");
                formSifirla();
            }
        }

        private void araVeListeleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "ARA";
            button2.Text = "LİSTELE";
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            button1.Location = new Point(6, 125);
            button2.Location = new Point(6, 154);
            button1.Image = Properties.Resources.Search_noHalo_16x;
            button2.Image = Properties.Resources.ListView_16x;
            listele("listele");
            formSifirla();
        }

        private void ekleVeGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "EKLE";
            button2.Text = "GÜNCELLE";
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            button1.Location = new Point(6, 102);
            button2.Location = new Point(6, 132);
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
                    string sorgu = "SELECT MADEN,FİYAT FROM tblMadenler where MADENID=@id";
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
                sorgu = "SELECT * FROM tblMadenler";
                OleDbCommand komut = new OleDbCommand(sorgu, bag);
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);

                Font font = new Font("Arial", 10, FontStyle.Bold);
                SolidBrush firca = new SolidBrush(Color.Black);
                Pen kalem = new Pen(Color.Black);

                e.Graphics.DrawString("MADEN ID", font, firca, 60, 50);
                e.Graphics.DrawString("MADEN", font, firca, 155, 50);
                e.Graphics.DrawString("FİYAT", font, firca, 280, 50);
                e.Graphics.DrawLine(kalem, 50, 75, 770, 75);

                font = new Font("Arial", 10, FontStyle.Regular);

                for (int i = 0, y = 85; i < tablo.Rows.Count; y += 20, i++)
                {
                    e.Graphics.DrawString(tablo.Rows[i][0].ToString(), font, firca, 60, y);
                    e.Graphics.DrawString(tablo.Rows[i][1].ToString().Replace("SİLİKAT", "S."), font, firca, 155, y);
                    e.Graphics.DrawString(tablo.Rows[i][2].ToString(), font, firca, 280, y);
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        private void raprolaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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
    }
}
