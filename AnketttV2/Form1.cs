using DAL;
using Entity.Models;
using Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnketttV2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        AnketContext db = new AnketContext();
        private void button2_Click(object sender, EventArgs e)
        {
            Soru soru = new Soru();
            soru.SoruCumlesi = textBox2.Text;

            db.Sorular.Add(soru);
            db.SaveChanges();
            SorulariYenile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SorulariYenile();
            CevaplariYenile();
        }
        public void CevaplariYenile()
        {
            dataGridView2.DataSource = null;
            //dataGridView2.DataSource = db.Cevaplar.ToList();
            dataGridView2.DataSource = db.Cevaplar.Select(x => new CevapViewModels()
            {
                AdSoyad = x.CevabiVerenKisi.AdSoyad,
                Soru = x.Sorusu.SoruCumlesi,
                Cevap = x.Yanit.ToString(),
            }).ToList();
        }
        public void SorulariYenile()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = db.Sorular.ToList();
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Clear();
            foreach (Soru soru in db.Sorular)
            {
                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = soru.SoruCumlesi;
                flowLayoutPanel1.Controls.Add(lbl);
                flowLayoutPanel1.SetFlowBreak(lbl, true);

                RadioButton r1 = new RadioButton();
                r1.Name = "Soru_" + soru.SoruID;
                r1.Text = "Evet";
                // flowLayoutPanel1.Controls.Add(r1);

                RadioButton r2 = new RadioButton();
                r2.Name = "Soru_" + soru.SoruID;
                r2.Text = "Hayır";
                //  flowLayoutPanel1.Controls.Add(r2);

                FlowLayoutPanel p = new FlowLayoutPanel();
                p.Width = 300;
                p.Height = 60;
                p.Controls.Add(r1);
                p.Controls.Add(r2);
                flowLayoutPanel1.Controls.Add(p);
                flowLayoutPanel1.SetFlowBreak(p, true);

                //ComboBox c1 = new ComboBox();
                //c1.Name="Soru_"+soru.SoruID;
                //c1.Items.Add("Evet");
                //c1.Items.Add("Hayır");
                //flowLayoutPanel1.Controls.Add(c1);
                //flowLayoutPanel1.SetFlowBreak(c1, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///*COMBOBOX*/
            //foreach (Control item in flowLayoutPanel1.Controls)
            //{
            //    if(item is ComboBox)
            //    {
            //        //Soru_15 ---> 15
            //        string soruID = item.Name.Replace("Soru_", "");
            //        int SID = Convert.ToInt32(soruID);
            //        Cevap c = new Cevap();
            //        c.SoruID = SID;
            //        int y= (((ComboBox)item).SelectedIndex + 1)% 2;
            //        c.Yanit = (Yanit)y;
            //    }
            //}

            /*RADIOBUTTON*/
            foreach (Control pnl in flowLayoutPanel1.Controls)
            {
                if (pnl is FlowLayoutPanel)
                {
                    foreach (RadioButton item in ((FlowLayoutPanel)pnl).Controls)
                    {
                        RadioButton r = item;
                        if (r.Checked)
                        {
                            //Soru_15 ---> 15
                            string soruID = item.Name.Replace("Soru_", "");
                            int SID = Convert.ToInt32(soruID);
                            Cevap c = new Cevap();
                            c.SoruID = SID;
                            c.Yanit = r.Text == "Evet" ? Yanit.Evet : Yanit.Hayir;//seçili butonumuz evetse evet,hayırsa hayır diye ayarlayabiliriz


                            Kisi k = db.Kisiler.Where(x => x.AdSoyad == textBox1.Text).FirstOrDefault();
                            if (k != null)
                                c.KisiID = k.KisiID;
                            else
                            {
                                k = new Kisi();
                                k.AdSoyad = textBox1.Text;
                                db.Kisiler.Add(k);
                                db.SaveChanges();
                                c.KisiID = k.KisiID;
                            }
                            db.Cevaplar.Add(c);
                            db.SaveChanges();
                        }
                    }
                }
            }
            MessageBox.Show("Eklendi");
            CevaplariYenile();
        }

        private void button3_Click(object sender, EventArgs e)
        {  //soru sil
            if (dataGridView1.SelectedRows.Count == 0)//seçili bir şey yok ise    
                MessageBox.Show("Soru seçiniz");
            else
            {
                foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                {
                    int SoruID = (int)item.Cells[0].Value;//silinecek satırın IDsine ulaş
                    Soru silinecek = db.Sorular.Find(SoruID);
                    db.Sorular.Remove(silinecek);
                }
                db.SaveChanges();//önce bir silsin sonra bir kerede kaydetsin o yüzden dışarı aldık
                SorulariYenile();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {   //soru düzenle
            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("Soru seçiniz");
            else
            {
                SoruDuzenle form = new SoruDuzenle();
                int secilenID = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                Soru duzenlenecek = db.Sorular.Find(secilenID);
                form.GelenSoru = duzenlenecek;
                form.Show();
            }           
        }
        private void button5_Click(object sender, EventArgs e)
        {  //cevap sil
            if (dataGridView2.SelectedRows.Count == 0)
                MessageBox.Show("Silinecek cevabı seçiniz");
            else
            {
                List<Cevap> silinecekler = new List<Cevap>();
                foreach (DataGridViewRow item in dataGridView2.SelectedRows)
                {
                    var silinecek = db.Cevaplar.ToList()[item.Index];
                    silinecekler.Add(silinecek);
                }
                db.Cevaplar.RemoveRange(silinecekler);
                db.SaveChanges();
                CevaplariYenile();             
            }
        }
    }
}
