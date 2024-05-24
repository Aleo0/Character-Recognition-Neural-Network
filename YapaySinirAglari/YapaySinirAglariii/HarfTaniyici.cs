using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YapaySinirAglariii
{
    public class HarfTaniyici
    {
        private int girisNöronSayisi = 35;
        private int gizliNöronSayisi = 10;
        private int cikisNöronSayisi = 5;

        public double HataDegeri { get; private set; }  
        private double ogrenmeOrani = 0.4;
        public double epsilon = 0.01;

        private int[,,] egitimVerileri = new int[5, 7, 5] {
            { {0,0,1,0,0}, {0,1,0,1,0}, {1,0,0,0,1}, {1,0,0,0,1}, {1,1,1,1,1}, {1,0,0,0,1}, {1,0,0,0,1} }, // A
            { {1,1,1,1,0}, {1,0,0,0,1}, {1,0,0,0,1}, {1,1,1,1,0}, {1,0,0,0,1}, {1,0,0,0,1}, {1,1,1,1,0} }, // B
            { {0,0,1,1,1}, {0,1,0,0,0}, {1,0,0,0,0}, {1,0,0,0,0}, {1,0,0,0,0}, {0,1,0,0,0}, {0,0,1,1,1} }, // C
            { {1,1,1,0,0}, {1,0,0,1,0}, {1,0,0,0,1}, {1,0,0,0,1}, {1,0,0,0,1}, {1,0,0,1,0}, {1,1,1,0,0} }, // D
            { {1,1,1,1,1}, {1,0,0,0,0}, {1,0,0,0,0}, {1,1,1,1,1}, {1,0,0,0,0}, {1,0,0,0,0}, {1,1,1,1,1} }  // E
        };
        private int[,] istenenCikti = new int[5, 5] {
            {0,0,0,0,1}, // A
            {0,0,0,1,0}, // B
            {0,0,1,0,0}, // C
            {0,1,0,0,0}, // D
            {1,0,0,0,0}  // E
        };

        private Layer girisGizliKatman;
        private Layer gizliCikisKatman;

        public HarfTaniyici()
        {
            girisGizliKatman = new Layer(gizliNöronSayisi, girisNöronSayisi);
            gizliCikisKatman = new Layer(cikisNöronSayisi, gizliNöronSayisi);
        }

        public void Egitim()
        {
            double hata = double.MaxValue;
            int epoch = 0;
            while (hata > epsilon && epoch < 10000)
            {
                double toplamHata = 0;
                for (int ornek = 0; ornek < egitimVerileri.GetLength(0); ornek++)
                {
                    double[] girisKatmani = GirisiDuzlestir(egitimVerileri, ornek);
                    double[] gizliKatmanCikti = girisGizliKatman.Hesapla(girisKatmani);
                    double[] cikisKatmani = gizliCikisKatman.Hesapla(gizliKatmanCikti);

                    double[] cikisHatasi = new double[cikisNöronSayisi];
                    for (int i = 0; i < cikisNöronSayisi; i++)
                    {
                        cikisHatasi[i] = istenenCikti[ornek, i] - cikisKatmani[i];
                        toplamHata += cikisHatasi[i] * cikisHatasi[i];
                    }

                    double[] gizliCikisKatmanHatasi = HatayiHesapla(cikisHatasi, gizliCikisKatman);
                    double[] girisGizliKatmanHatasi = HatayiHesapla(gizliCikisKatmanHatasi, girisGizliKatman);

                    gizliCikisKatman.AgirliklariGuncelle(gizliCikisKatmanHatasi, gizliKatmanCikti, ogrenmeOrani);
                    girisGizliKatman.AgirliklariGuncelle(girisGizliKatmanHatasi, girisKatmani, ogrenmeOrani);
                }
                hata = toplamHata / (egitimVerileri.GetLength(0) * cikisNöronSayisi);
                HataDegeri = hata;

                epoch++;
                Console.WriteLine($"Epoch {epoch}, Hata: {hata}");
            }
        }

        private double[] GirisiDuzlestir(int[,,] veriler, int ornek)
        {
            double[] duz = new double[girisNöronSayisi];
            int indeks = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    duz[indeks++] = veriler[ornek, i, j];
                }
            }
            return duz;
        }

        private double[] GirisiDuzlestir(int[,] giris)
        {
            double[] duz = new double[girisNöronSayisi];
            int indeks = 0;
            for (int i = 0; i < giris.GetLength(0); i++)
            {
                for (int j = 0; j < giris.GetLength(1); j++)
                {
                    duz[indeks++] = giris[i, j];
                }
            }
            return duz;
        }

        private double[] HatayiHesapla(double[] cikisHatasi, Layer katman)
        {
            double[] gizliHata = new double[katman.Nöronlar.Length];
            for (int i = 0; i < katman.Nöronlar.Length; i++)
            {
                for (int j = 0; j < cikisHatasi.Length; j++)
                {
                    gizliHata[i] += cikisHatasi[j] * katman.Nöronlar[j].Agirliklar[i];
                }
                gizliHata[i] *= katman.Nöronlar[i].Cikti * (1 - katman.Nöronlar[i].Cikti);
            }
            return gizliHata;
        }

        public char TahminEt(int[,] giris)
        {
            
            double[] girisKatmani = GirisiDuzlestir(giris);
            double[] gizliKatman = girisGizliKatman.Hesapla(girisKatmani);
            double[] cikisKatmani = gizliCikisKatman.Hesapla(gizliKatman);

            int maksIndeks = 0;
            for (int i = 1; i < cikisKatmani.Length; i++)
            {
                if (cikisKatmani[i] > cikisKatmani[maksIndeks])
                {
                    maksIndeks = i;
                }
            }

            return (char)('A' + maksIndeks);
        }

        public string SinifOlasiliklariniYazdir(double[] cikisKatmani)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < cikisNöronSayisi; i++)
            {
                char harf = (char)('A' + i);
                sb.Append($"{harf}: {cikisKatmani[i]:F10}\n"); // Olasılığı yazdır
            }

            return sb.ToString();
        }

        public double[] CikisKatmaniniGetir(int[,] giris)
        {
            double[] girisKatmani = GirisiDuzlestir(giris);
            double[] gizliKatman = girisGizliKatman.Hesapla(girisKatmani);
            return gizliCikisKatman.Hesapla(gizliKatman);
        }


        public void AgirliklariKaydet(string dosyaAdi)
        {
            using (StreamWriter writer = new StreamWriter(dosyaAdi))
            {
                // Giriş-Gizli katman ağırlıkları
                for (int i = 0; i < girisGizliKatman.Nöronlar.Length; i++)
                {
                    for (int j = 0; j < girisGizliKatman.Nöronlar[i].Agirliklar.Length; j++)
                    {
                        writer.WriteLine(girisGizliKatman.Nöronlar[i].Agirliklar[j]);
                    }
                    writer.WriteLine(girisGizliKatman.Nöronlar[i].Bias);
                }

                // Gizli-Çıkış katman ağırlıkları
                for (int i = 0; i < gizliCikisKatman.Nöronlar.Length; i++)
                {
                    for (int j = 0; j < gizliCikisKatman.Nöronlar[i].Agirliklar.Length; j++)
                    {
                        writer.WriteLine(gizliCikisKatman.Nöronlar[i].Agirliklar[j]);
                    }
                    writer.WriteLine(gizliCikisKatman.Nöronlar[i].Bias);
                }
            }
        }

        public bool AgirliklariYukle(string dosyaAdi)
        {
            if (!File.Exists(dosyaAdi))
                return false;

            using (StreamReader reader = new StreamReader(dosyaAdi))
            {
                // Giriş-Gizli katman ağırlıkları
                for (int i = 0; i < girisGizliKatman.Nöronlar.Length; i++)
                {
                    for (int j = 0; j < girisGizliKatman.Nöronlar[i].Agirliklar.Length; j++)
                    {
                        if (!double.TryParse(reader.ReadLine(), out girisGizliKatman.Nöronlar[i].Agirliklar[j]))
                            return false;
                    }
                    double biasValue;
                    if (!double.TryParse(reader.ReadLine(), out biasValue))
                        return false;
                    girisGizliKatman.Nöronlar[i].Bias = biasValue; // Bias değerini atama
                }

                // Gizli-Çıkış katman ağırlıkları
                for (int i = 0; i < gizliCikisKatman.Nöronlar.Length; i++)
                {
                    for (int j = 0; j < gizliCikisKatman.Nöronlar[i].Agirliklar.Length; j++)
                    {
                        if (!double.TryParse(reader.ReadLine(), out gizliCikisKatman.Nöronlar[i].Agirliklar[j]))
                            return false;
                    }
                    double biasValue;
                    if (!double.TryParse(reader.ReadLine(), out biasValue))
                        return false;
                    gizliCikisKatman.Nöronlar[i].Bias = biasValue;
                }
            }
            return true;
        }
    }

}

