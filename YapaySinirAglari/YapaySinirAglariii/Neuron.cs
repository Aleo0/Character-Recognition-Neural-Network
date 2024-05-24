using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YapaySinirAglariii
{
    public class Neuron
    {
        public double[] Agirliklar { get; set; }
        public double Bias { get; set; }
        public double Cikti { get; set; }

        public Neuron(int girdiSayisi)
        {
            Agirliklar = new double[girdiSayisi];
            Random rastgele = new Random();
            for (int i = 0; i < girdiSayisi; i++)
            {
                Agirliklar[i] = rastgele.NextDouble() * 2 - 1;
            }
            Bias = rastgele.NextDouble() * 2 - 1;
        }

        public double Hesapla(double[] girdiler)
        {
            double net = Bias;
            for (int i = 0; i < girdiler.Length; i++)
            {
                net += Agirliklar[i] * girdiler[i];
            }
            return Sigmoid(net);
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }

}
