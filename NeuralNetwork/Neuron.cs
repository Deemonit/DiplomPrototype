using System;
using static System.Math;

namespace NeuralNetwork
{
    public class Neuron
    {
        public Neuron(double[] inputs, double[] weights, Network.LayerType layerType)
        {
            Weights = weights;
            Inputs = inputs;
            this.layerType = layerType;
        }
        private Network.LayerType layerType;

        private double[] _weights;
        private double[] _inputs;

        private double _output;

        public double[] Weights { get => _weights; set => _weights = value; }

        public double Output
        {
            get { return _output; }
            set
            {
                //Тип нейрона либо скрытый, либо выходной. У входного слоя нет нейронов. Там просто массив пикселей
                _output = layerType = Network.LayerType.OUTPUT ? GetOutputNeuronActivation(_inputs, _weights) : GetHiddenNeuronActivation(_inputs, _weights);
            }
        }
        public double[] Inputs { get => _inputs; set => _inputs = value; }

        //inputs - массив значений выходов нейронов предыдущего слоя; weights - массив значений весов, которые связывают текущий нейрон с предыдущим слоем
        private double GetOutputNeuronActivation(double[] inputs, double[] weights)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; ++i)
            {
                sum += inputs[i] * weights[i];
            }

            //if (sum > 10)
            //{
            //    sum /= Math.Pow(10, Math.Floor(Math.Log10(sum)));
            //}
            //else if (sum < -10)
            //{
            //    sum /= Math.Pow(10, Math.Floor(Math.Log10(-sum)));
            //}
            //sum = 1 / (1 + Math.Exp(-sum));


            return sum;
        }
        private double GetHiddenNeuronActivation(double[] inputs, double[] weights)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; ++i)
            {
                sum += inputs[i] * weights[i];
            }



            //if (double.IsNaN(sum) || double.IsInfinity(sum))
            //{
            //    sum = 1;
            //}

            //if (sum > 10)
            //{
            //    sum /= Math.Pow(10, Math.Floor(Math.Log10(sum)));
            //}
            //else if (sum < -10)
            //{
            //    sum /= Math.Pow(10, Math.Floor(Math.Log10(-sum)));
            //}
            //sum = Max(sum, sum * NeuralNetworkConstructor.NetworkParameters.alpha);

            return LeakyReLU(sum);
        }

        private double LeakyReLU(double value)
        {
            return value >= 0 ? value: NetworkParameters.alpha * value;
        }

        public static double[] Softmax(double[] logits)
    {
        if (logits == null)
            throw new ArgumentNullException(nameof(logits));

        if (logits.Length == 0)
            throw new ArgumentException(
                "Массив логитов не должен быть пустым.",
                nameof(logits));

        var probabilities = new double[logits.Length];

        // Вычитаем максимум для численной устойчивости.
        double maxLogit = logits[0];

        for (int i = 1; i < logits.Length; i++)
        {
            if (logits[i] > maxLogit)
                maxLogit = logits[i];
        }

        double sum = 0.0;

        for (int i = 0; i < logits.Length; i++)
        {
            probabilities[i] = Math.Exp(logits[i] - maxLogit);
            sum += probabilities[i];
        }

        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] /= sum;
        }

        return probabilities;
    }
    }
}
