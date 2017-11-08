using DAL;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnketttV2
{
    public partial class SoruDuzenle : Form
    {
        public Soru GelenSoru { get; set; }
        public SoruDuzenle()
        {
            InitializeComponent();
        }

        private void SoruDuzenle_Load(object sender, EventArgs e)
        {
            textBox1.Text = GelenSoru.SoruCumlesi;
        }

        private void button1_Click(object sender, EventArgs e)
        {   //soru düzenle kaydet
            //EF bir kayıtta değişiklik yapabilmesi CONTEXT üzerinden geliyorsa mümkün
            AnketContext db = new AnketContext();
            //düzenlenecek kaydın veritabanındaki halini bulduk getirdik
            var duzenlenecek = db.Sorular.Find(GelenSoru.SoruID);
            duzenlenecek.SoruCumlesi = textBox1.Text;
            //kayıtta değişiklik var:
            db.Entry(duzenlenecek).State = EntityState.Modified;
            db.SaveChanges();

            Form1 f =(Form1) Application.OpenForms["Form1"];
            f.SorulariYenile();
            f.CevaplariYenile();
            this.Close();
        }
    }
}
