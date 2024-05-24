using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YapaySinirAglariii
{
    public partial class Form1 : Form
    {
        private int[,] testGirisMatrisi = new int[7, 5];
        private HarfTaniyici harfTaniyici;

        private List<PictureBox> pictureBoxes = new List<PictureBox>(); // PictureBox'ları tutacak List


        public Form1()
        {
            InitializePictureBoxes();
            InitializeComponent();
           
            harfTaniyici = new HarfTaniyici();
           
        }

        private void InitializePictureBoxes()
        {
            const int kareBoyutu = 40;
            const int aralik = 5;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.BackColor = Color.White;
                    pictureBox.Tag = new Point(i, j);
                    pictureBox.Size = new Size(kareBoyutu, kareBoyutu);
                    pictureBox.Location = new Point(j * (kareBoyutu + aralik) + aralik, i * (kareBoyutu + aralik) + aralik);
                    pictureBox.Click += PictureBox_Click;
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    Controls.Add(pictureBox);
                    pictureBoxes.Add(pictureBox);
                }
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            Point konum = (Point)pictureBox.Tag;
            int satir = konum.X;
            int sutun = konum.Y;

            if (testGirisMatrisi[satir, sutun] == 0)
            {
                pictureBox.BackColor = Color.Black;
                testGirisMatrisi[satir, sutun] = 1;
            }
            else
            {
                pictureBox.BackColor = Color.White;
                testGirisMatrisi[satir, sutun] = 0;
            }
        }

        private void btnEgit_Click(object sender, EventArgs e)
        {
            ClearTestGirisMatrisi();
            lblSonuc.Text = "--";
            

            harfTaniyici.epsilon = (double)nudEpsilon.Value;

            harfTaniyici.Egitim();
            lblHataDegeri.Text = $"Hata: {harfTaniyici.HataDegeri}";

            // Ağırlıkları kaydet
            harfTaniyici.AgirliklariKaydet("agirliklar.txt");

            btnTahminEt.Enabled = true;
            btnAgirliklariYukle.Enabled = true;

            MessageBox.Show("Eğitim tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTahminEt_Click(object sender, EventArgs e)
        {
            char tahmin = harfTaniyici.TahminEt(testGirisMatrisi);
            

            double[] cikisKatmani = harfTaniyici.CikisKatmaniniGetir(testGirisMatrisi); // Yeni fonksiyon
            string olasiliklar = harfTaniyici.SinifOlasiliklariniYazdir(cikisKatmani);
            lblSonuc.Text = olasiliklar;


        }

        private void ClearTestGirisMatrisi()
        {
            int index = 0;
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                pictureBox.BackColor = Color.White;
                int i = index / 5;  // Satır hesaplama
                int j = index % 5;  // Sütun hesaplama
                testGirisMatrisi[i, j] = 0;
                index++;
            }
        }

        private void btnAgirliklariYukle_Click(object sender, EventArgs e)
        {
            if (harfTaniyici.AgirliklariYukle("agirliklar.txt"))
            {
                MessageBox.Show("Ağırlıklar yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnTahminEt.Enabled = true;
            }
            else
            {
                MessageBox.Show("Ağırlıklar yüklenemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nudEpsilon_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
