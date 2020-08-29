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
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }        

        OleDbConnection bag = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = uzaymadencilik.accdb");
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string ka = textBox1.Text;
                string sifre = textBox2.Text;
                string sorgu = "SELECT kulId FROM tblKullanicilar where ka=@ka and sifre=@sifre";
                OleDbCommand komut = new OleDbCommand(sorgu, bag);
                komut.Parameters.AddWithValue("@ka",ka);
                komut.Parameters.AddWithValue("@sifre", sifre);               
                bag.Open();
                OleDbDataReader oku = komut.ExecuteReader();
                if(oku.Read())
                {
                    int kulId = int.Parse(oku[0].ToString());                    
                    oku.Close();
                    bag.Close();
                    Form3 form3 = new Form3(kulId,ka);
                    this.Hide();
                    form3.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
                    oku.Close();
                    bag.Close();
                }
              
                
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message);
                if (bag.State == ConnectionState.Open) bag.Close();
            }
        }
    }
}
