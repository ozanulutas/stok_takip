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
    public partial class Form4 : Form
    {
        Form2 form2 = new Form2();
        bool tarihDegistiMi = false;
        int id;
        int satisEskiMiktar;
        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");        

        public Form4()
        {
            InitializeComponent();
            
        }

        void listele(string islem)
        {
            try
            {
                OleDbCommand komut = new OleDbCommand();
                if (islem == "listele" || (comboBox1.Text == "" && textBox1.Text == "" && textBox2.Text == "" && tarihDegistiMi == false && textBox3.Text == "" ))
                {
                    komut.CommandText = "SELECT * FROM tblSatislar";                    
                }
                else
                {
                    komut = new OleDbCommand();
                    string sorgu = "SELECT * FROM tblSatislar WHERE ";
                    if (comboBox1.Text != "")
                    {
                        sorgu += "MADEN = @maden AND ";
                        komut.Parameters.AddWithValue("@maden", comboBox1.Text);
                    }
                    if (textBox1.Text != "")
                    {
                        sorgu += "MİKTAR = @miktar AND ";
                        komut.Parameters.AddWithValue("@miktar", textBox1.Text);
                    }
                    if (textBox2.Text != "")
                    {
                        if(radioButton1.Checked==true)                        
                            sorgu += "FİYAT >= @fiyat AND ";                        
                        else if (radioButton2.Checked== true)                        
                            sorgu += "FİYAT < @fiyat AND ";
                        else
                            sorgu += "FİYAT = @fiyat AND ";
                        komut.Parameters.AddWithValue("@fiyat", textBox2.Text);
                    }
                    if (tarihDegistiMi)
                    {
                        sorgu += "TARİH = @tarih AND ";
                        komut.Parameters.AddWithValue("@tarih", dateTimePicker1.Value.ToShortDateString());
                    }
                    if (textBox3.Text != "")
                    {
                        sorgu += "MÜŞTERİ = @musteri AND ";
                        komut.Parameters.AddWithValue("@musteri", textBox3.Text);
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
                string maden = comboBox1.Text;
                int miktar = int.Parse(textBox1.Text);
                string musteri = textBox3.Text.ToUpper();                
                DateTime tarih = dateTimePicker1.Value;
                int fiyat = 0;

                OleDbCommand komutFiyat = new OleDbCommand();
                komutFiyat.CommandText = "SELECT FİYAT FROM tblMadenler WHERE MADEN = @maden";
                komutFiyat.Parameters.AddWithValue("@maden", maden);
                komutFiyat.Connection = bag;
                bag.Open();
                OleDbDataReader oku = komutFiyat.ExecuteReader();
                while (oku.Read())
                {
                    fiyat = int.Parse(oku[0].ToString());
                }
                oku.Close();
                bag.Close();
                fiyat *= miktar;                

                if (islem == "ekle")
                {
                    if(stokMiktar(maden) >= miktar)
                    {
                        OleDbCommand komut = new OleDbCommand();
                        komut.CommandText = "INSERT INTO tblSatislar(MADEN, MİKTAR, FİYAT, TARİH, MÜŞTERİ) VALUES(@maden, @miktar, @fiyat, @tarih, @musteri)";
                        komut.Parameters.AddWithValue("@maden", maden);
                        komut.Parameters.AddWithValue("@miktar", miktar);
                        komut.Parameters.AddWithValue("@fiyat", fiyat);
                        komut.Parameters.AddWithValue("@tarih", tarih.ToShortDateString());
                        komut.Parameters.AddWithValue("@musteri", musteri);                        
                        komut.Connection = bag;
                        bag.Open();
                        komut.ExecuteNonQuery();
                        bag.Close();

                        stokGuncelle("ekle", maden, miktar);
                    }     
                    else
                        MessageBox.Show(maden+" stoğu yetersiz!");
                }
                if (islem == "guncelle")
                {
                    if(stokMiktar(maden) >= (miktar - satisEskiMiktar))
                    {
                        OleDbCommand komut = new OleDbCommand();
                        komut.CommandText = "UPDATE tblSatislar SET MADEN=@maden, MİKTAR=@miktar, FİYAT=@fiyat, TARİH=@tarih, MÜŞTERİ=@musteri WHERE SATIŞID=" + id;
                        komut.Parameters.AddWithValue("@maden", maden);
                        komut.Parameters.AddWithValue("@miktar", miktar);
                        komut.Parameters.AddWithValue("@fiyat", fiyat);
                        komut.Parameters.AddWithValue("@tarih", tarih.ToShortDateString());
                        komut.Parameters.AddWithValue("@musteri", musteri);
                        komut.Connection = bag;
                        bag.Open();
                        komut.ExecuteNonQuery();
                        bag.Close();

                        stokGuncelle("guncelle", maden, miktar);
                    }
                    else
                        MessageBox.Show(maden + " stoğu yetersiz!");
                }
            }
            catch (Exception hata)
            {
                if (hata.Message == "Giriş dizesi doğru biçimde değildi.") MessageBox.Show("Lütfen tüm alanları doldurun.");
                else MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        public void sil(DataGridView dgv)
        {
            try
            {
                string maden = dgv.CurrentRow.Cells[1].Value.ToString();
                int miktar = int.Parse(dgv.CurrentRow.Cells[2].Value.ToString());

                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "DELETE FROM tblSatislar WHERE SATIŞID = @id";
                komut.Parameters.AddWithValue("@id", dgv.CurrentRow.Cells[0].Value);
                komut.Connection = bag;
                bag.Open();
                komut.ExecuteNonQuery();
                bag.Close();

                DateTime tarih = (DateTime)dgv.CurrentRow.Cells[4].Value;
                if (tarih >= DateTime.Now)
                    stokGuncelle("sil", maden, miktar);
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }

        void stokGuncelle(string satisIslem, string maden, int miktar)
        {
            int stokEskiMiktar=0;
            try
            {                
                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "SELECT MİKTAR FROM tblStok WHERE MADEN=@maden";
                komut.Parameters.AddWithValue("@maden", maden);
                komut.Connection = bag;
                bag.Open();
                OleDbDataReader oku1 = komut.ExecuteReader();
                while (oku1.Read())
                {
                    stokEskiMiktar = int.Parse(oku1[0].ToString());
                }
                oku1.Close();
                bag.Close();

                if (satisIslem == "ekle")
                {
                    komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblStok SET MİKTAR=@miktar WHERE MADEN=@maden";
                    komut.Parameters.AddWithValue("@miktar", stokEskiMiktar - miktar);
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();                 
                }
                if (satisIslem == "guncelle")
                {
                    komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblStok SET MİKTAR=@miktar WHERE MADEN=@maden";
                    if (miktar > satisEskiMiktar)
                        komut.Parameters.AddWithValue("@miktar", stokEskiMiktar - (miktar - satisEskiMiktar));
                    else if (miktar < satisEskiMiktar)
                        komut.Parameters.AddWithValue("@miktar", stokEskiMiktar + (satisEskiMiktar - miktar));
                    else
                        komut.Parameters.AddWithValue("@miktar", stokEskiMiktar);
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();                 
                }
                if (satisIslem == "sil")
                {
                    komut = new OleDbCommand();
                    komut.CommandText = "UPDATE tblStok SET MİKTAR=@miktar WHERE MADEN=@maden";
                    komut.Parameters.AddWithValue("@miktar", stokEskiMiktar + miktar);
                    komut.Parameters.AddWithValue("@maden", maden);
                    komut.Connection = bag;
                    bag.Open();
                    komut.ExecuteNonQuery();
                    bag.Close();                  
                }                
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }            
        }

        int stokMiktar(string maden)
        {
            int stokMiktar = 0;
            try
            {
                OleDbCommand komut = new OleDbCommand();
                komut.CommandText = "SELECT MİKTAR FROM tblStok WHERE MADEN=@maden";
                komut.Parameters.AddWithValue("@maden", maden);
                komut.Connection = bag;
                bag.Open();
                OleDbDataReader oku1 = komut.ExecuteReader();
                while (oku1.Read())
                {
                    stokMiktar = int.Parse(oku1[0].ToString());
                }
                oku1.Close();
                bag.Close();
            }
            catch (Exception hata)
            {               
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
            return stokMiktar;
        }

        void formSifirla()
        {
            comboBox1.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            textBox2.Text = "";
            textBox3.Text = "";            
            tarihDegistiMi = false;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            dateTimePicker1.ValueChanged += new System.EventHandler(dateTimePicker1_ValueChanged); //tarihDegistiMi için
            
            form2.comboBoxAta(comboBox1,"tblMadenler");
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
        //HIZLI ARA
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string aranan = toolStripTextBox1.Text;
                DateTime tarih;
                int sayi;

                OleDbCommand komut = new OleDbCommand();
                if (DateTime.TryParse(aranan, out tarih))
                    komut = new OleDbCommand("SELECT * FROM tblSatislar WHERE TARİH=@aranan", bag);
                else if (int.TryParse(aranan, out sayi))
                    komut = new OleDbCommand("SELECT * FROM tblSatislar WHERE MİKTAR=@aranan OR FİYAT=@aranan", bag);
                else
                    komut = new OleDbCommand("SELECT * FROM tblSatislar WHERE MADEN=@aranan OR MÜŞTERİ=@aranan", bag);
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
                sil(dataGridView1);
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
                sil(dataGridView1);
                listele("listele");
                formSifirla();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            tarihDegistiMi = true;
        }

        private void araListeleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "ARA";
            button2.Text = "LİSTELE";
            label6.Visible = true;
            textBox2.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            button1.Location = new Point(6, 235);
            button2.Location = new Point(6, 264);
            button1.Image = Properties.Resources.Search_noHalo_16x;
            button2.Image = Properties.Resources.ListView_16x;
            listele("listele");
            formSifirla();
        }

        private void ekleVeGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Text = "EKLE";
            button2.Text = "GÜNCELLE";
            label6.Visible = false;
            textBox2.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            button1.Location = new Point(6,177);
            button2.Location = new Point(6, 206);
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
                    satisEskiMiktar= int.Parse(dataGridView1[2, indis].Value.ToString());
                }
                try
                {
                    string sorgu = "SELECT MADEN,MİKTAR,TARİH,MÜŞTERİ FROM tblSatislar where SATIŞID=@id";
                    OleDbCommand komut = new OleDbCommand(sorgu, bag);
                    komut.Parameters.AddWithValue("@id", id);
                    bag.Open();
                    OleDbDataReader oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        comboBox1.Text = oku[0].ToString();
                        textBox1.Text = oku[1].ToString();
                        dateTimePicker1.Value = (DateTime)oku[2];
                        textBox3.Text = oku[3].ToString();     
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
        //RAPROLAMA
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                string sorgu;
                sorgu = "SELECT * FROM tblSatislar";
                OleDbCommand komut = new OleDbCommand(sorgu, bag);
                OleDbDataAdapter adp = new OleDbDataAdapter(komut);
                DataTable tablo = new DataTable();
                adp.Fill(tablo);

                Font font = new Font("Arial", 10, FontStyle.Bold);
                SolidBrush firca = new SolidBrush(Color.Black);
                Pen kalem = new Pen(Color.Black);

                e.Graphics.DrawString("SATIŞ ID", font, firca, 50, 50);
                e.Graphics.DrawString("MADEN", font, firca, 145, 50);
                e.Graphics.DrawString("MİKTAR", font, firca, 260, 50);
                e.Graphics.DrawString("FİYAT", font, firca, 360, 50);
                e.Graphics.DrawString("MÜŞTERİ", font, firca, 500, 50);
                e.Graphics.DrawString("TARİH", font, firca, 650, 50);
                e.Graphics.DrawLine(kalem, 50, 75, 770, 75);

                font = new Font("Arial", 10, FontStyle.Regular);

                for (int i = 0, y = 85; i < tablo.Rows.Count; y += 20, i++)
                {
                    e.Graphics.DrawString(tablo.Rows[i][0].ToString(), font, firca, 50, y);
                    e.Graphics.DrawString(tablo.Rows[i][1].ToString().Replace("SİLİKAT", "S."), font, firca, 145, y);
                    e.Graphics.DrawString(tablo.Rows[i][2].ToString(), font, firca, 260, y);
                    e.Graphics.DrawString(tablo.Rows[i][3].ToString(), font, firca, 360, y);
                    e.Graphics.DrawString(tablo.Rows[i][5].ToString().Substring(0, tablo.Rows[i][5].ToString().Length >= 14 ? 14 : tablo.Rows[i][5].ToString().Length), font, firca, 500, y);
                    e.Graphics.DrawString(tablo.Rows[i][4].ToString().Substring(0, 10), font, firca, 650, y);
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
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
    }
    
}
