using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YapaySinirAglariii
{
        public class Layer
        {
            public Neuron[] Nöronlar { get; private set; }

            public Layer(int nöronSayisi, int girdiSayisi)
            {
                Nöronlar = new Neuron[nöronSayisi];
                for (int i = 0; i < nöronSayisi; i++)
                {
                    Nöronlar[i] = new Neuron(girdiSayisi);
                }
            }

            public double[] Hesapla(double[] girdiler)
            {
                double[] ciktilar = new double[Nöronlar.Length];
                for (int i = 0; i < Nöronlar.Length; i++)
                {
                    Nöronlar[i].Cikti = Nöronlar[i].Hesapla(girdiler);
                    ciktilar[i] = Nöronlar[i].Cikti;
                }
                return ciktilar;
            }

            public void AgirliklariGuncelle(double[] hatalar, double[] girdiler, double ogrenmeOrani)
            {
                for (int i = 0; i < Nöronlar.Length; i++)
                {
                    for (int j = 0; j < Nöronlar[i].Agirliklar.Length; j++)
                    {
                        Nöronlar[i].Agirliklar[j] += ogrenmeOrani * hatalar[i] * girdiler[j];
                    }
                    Nöronlar[i].Bias += ogrenmeOrani * hatalar[i];
                }
            }   
        }
}
