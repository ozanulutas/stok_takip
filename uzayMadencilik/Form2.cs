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
    public partial class Form2 : Form
    {              
        bool tarihDegistiMi = false;
        int id;
        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");
               
        public Form2()
        {
            InitializeComponent();                      
        }

        public void comboBoxAta(ComboBox cb, string tabloAdi)
        {
            try
            {
                cb.Items.Clear();
                string sorgu = "SELECT * FROM " + tabloAdi;
                OleDbCommand komut = new OleDbCommand(sorgu, bag);                
                bag.Open();
                OleDbDataReader oku = komut.ExecuteReader();
                while (oku.Read()) 
                {
                    string kayit = oku[1].ToString();
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
        
        void listele(string islem)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();
                if (islem == "listele" || (comboBox1.Text == "" && comboBox2.Text == "" && comboBox3.Text == "" && textBox1.Text == "" && tarihDegistiMi==false))
                {
                    komut.CommandText = "SELECT * FROM tblSeferler";
                }
                else
                {
                    komut = new OleDbCommand();
                    string sorgu = "SELECT * FROM tblSeferler WHERE ";
                    if (comboBox1.Text != "")
                    {
                        sorgu += "DESTİNASYON = @dest AND ";
                        komut.Parameters.AddWithValue("@dest", comboBox1.Text);
                    }
                    if (comboBox2.Text != "")
                    {
                        sorgu += "MADEN = @maden AND ";
                        komut.Parameters.AddWithValue("@maden", comboBox2.Text);
                    }
                    if (comboBox3.Text != "")
                    {
                        sorgu += "TEKNİK = @teknik AND ";
                        komut.Parameters.AddWithValue("@teknik", comboBox3.Text);
                    }
                    if (textBox1.Text != "")
                    {
                        sorgu += "PERSONEL = @personel AND ";
                        komut.Parameters.AddWithValue("@personel", textBox1.Text);
                    }
                    if (tarihDegistiMi)
                    {                                               
                        sorgu += "TARİH = @tarih AND ";
                        komut.Parameters.AddWithValue("@tarih", dateTimePicker1.Value.ToShortDateString());
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
                string dest = comboBox1.Text.ToUpper();
                string maden = comboBox2.Text.ToUpper();
                string teknik = comboBox3.Text.ToUpper();
                int personel = int.Parse(textBox1.Text);
                DateTime tarih = dateTimePicker1.Value;

                if(islem=="ekle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "INSERT INTO tblSeferler(DESTİNASYON, MADEN, TEKNİK, PERSONEL, TARİH) VALUES(@dest, @maden, @teknik, @personel, @tarih)";
                    komut.Parameters.AddWithValue("@dest", dest);
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Parameters.AddWithValue("@teknik", teknik);
                    komut.Parameters.AddWithValue("@personel", personel);
                    komut.Parameters.AddWithValue("@tarih", tarih.ToShortDateString());
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                }
                    
                if(islem=="guncelle")
                {
                    OleDbCommand komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblSeferler SET DESTİNASYON=@dest, MADEN=@maden, TEKNİK=@teknik, PERSONEL=@personel, TARİH=@tarih WHERE SEFERID=" + id;
                    komut.Parameters.AddWithValue("@dest", dest);
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Parameters.AddWithValue("@teknik", teknik);
                    komut.Parameters.AddWithValue("@personel", personel);
                    komut.Parameters.AddWithValue("@tarih", tarih.ToShortDateString());
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();
                }
            }
            catch (Exception hata)
            {
                if (hata.Message=="Giriş dizesi doğru biçimde değildi.") MessageBox.Show("Lütfen tüm alanları doldurun.");
                else MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();    
            }
        }

        public void sil(string tablo, string sutun, DataGridView dgv)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "DELETE FROM "+ tablo + " WHERE "+ sutun + "=@id";
                komut.Parameters.AddWithValue("@id", dgv.CurrentRow.Cells[0].Value);
                komut.Connection = bag;
                bag.Open();
                komut.ExecuteNonQuery();
                bag.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        void formSifirla()
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            tarihDegistiMi = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBoxAta(comboBox1,"tblDestinasyonlar");
            comboBoxAta(comboBox2, "tblMadenler");
            listele("liste");
            dateTimePicker1.ValueChanged += new System.EventHandler(dateTimePicker1_ValueChanged); //tarihDegistiMi için            
        }
        //ARAMA VE EKLEME
        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "ARA")
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
        }
        //HIZLI ARA
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string aranan = toolStripTextBox1.Text;
                DateTime tarih;

                OleDbCommand komut = new OleDbCommand();
                if (DateTime.TryParse(aranan, out tarih))
                    komut = new OleDbCommand("SELECT * FROM tblSeferler WHERE TARİH=@aranan", bag);
                else
                    komut = new OleDbCommand("SELECT * FROM tblSeferler WHERE DESTİNASYON=@aranan OR MADEN=@aranan OR TEKNİK=@aranan OR PERSONEL=@aranan", bag);
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
        //LİSTELEME
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            listele("listele");
            formSifirla();
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
        //SİLME İŞLEMİ
        public void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {              
            DialogResult cevap = MessageBox.Show("Kayıt silinsin mi?", "UYARI", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                sil("tblSeferler", "SEFERID", dataGridView1);
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
                sil("tblSeferler", "SEFERID", dataGridView1);
                listele("listele");
                formSifirla();
            }            
        }
        
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(button2.Text == "GÜNCELLE")
            {
                if (dataGridView1.CurrentCell.RowIndex < dataGridView1.RowCount - 1)
                {
                    int indis = dataGridView1.CurrentCell.RowIndex;
                    id = int.Parse(dataGridView1[0, indis].Value.ToString());
                }
                try
                {
                    string sorgu = "SELECT * FROM tblSeferler where SEFERID=@id";
                    OleDbCommand komut = new OleDbCommand(sorgu, bag);
                    komut.Parameters.AddWithValue("@id", id);
                    bag.Open();
                    OleDbDataReader oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        comboBox1.Text = oku[1].ToString();
                        comboBox2.Text = oku[2].ToString();
                        comboBox3.Text = oku[3].ToString();
                        textBox1.Text = oku[4].ToString();
                        dateTimePicker1.Value = (DateTime)oku[5];
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

        private void seferlerToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            button1.Text = "ARA";
            button2.Text = "LİSTELE";
            button1.Image = Properties.Resources.Search_noHalo_16x;
            button2.Image = Properties.Resources.ListView_16x;
            listele("listele");
            formSifirla();
            
        }

        private void ekleVeGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            button1.Text = "EKLE";
            button2.Text = "GÜNCELLE";
            button1.Image = Properties.Resources.Add_16x;
            button2.Image = Properties.Resources.GoToNextModified_16x;
            listele("listele");
            formSifirla();
        }
        
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            tarihDegistiMi = true;
        }

        private void raporlaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }
        //RAPROLAMA
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                string sorgu;
                sorgu = "SELECT * FROM tblSeferler";
                OleDbCommand komut = new OleDbCommand(sorgu, bag);
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);

                Font font = new Font("Arial", 10, FontStyle.Bold);
                SolidBrush firca = new SolidBrush(Color.Black);
                Pen kalem = new Pen(Color.Black);

                e.Graphics.DrawString("SEFER ID", font, firca, 50, 50);
                e.Graphics.DrawString("DESTİNASYON", font, firca, 150, 50);
                e.Graphics.DrawString("MADEN", font, firca, 290, 50);
                e.Graphics.DrawString("ÇIKARMA \nTEKNİĞİ", font, firca, 430, 50);
                e.Graphics.DrawString("PERSONEL \nSAYISI", font, firca, 550, 50);
                e.Graphics.DrawString("TARİH", font, firca, 650, 50);
                e.Graphics.DrawLine(kalem, 50, 90, 770, 90);

                font = new Font("Arial", 10, FontStyle.Regular);

                for (int i = 0, y = 95; i < tablo.Rows.Count; y += 20, i++)
                {
                    e.Graphics.DrawString(tablo.Rows[i][0].ToString(), font, firca, 50, y);
                    e.Graphics.DrawString(tablo.Rows[i][1].ToString(), font, firca, 150, y);
                    e.Graphics.DrawString(tablo.Rows[i][2].ToString().Replace("SİLİKAT", "S."), font, firca, 290, y);
                    e.Graphics.DrawString(tablo.Rows[i][3].ToString().Replace("PROSES", "P."), font, firca, 430, y);
                    e.Graphics.DrawString(tablo.Rows[i][4].ToString(), font, firca, 550, y);
                    e.Graphics.DrawString(tablo.Rows[i][5].ToString().Substring(0, 10), font, firca, 650, y);
                }
            }
            catch(Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
            
        }       

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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

        private void programdanÇıkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
    

}
