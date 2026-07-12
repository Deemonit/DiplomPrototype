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
                _output = GetOutputNeuronActivation(_inputs, _weights);
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
            if (double.IsNaN(sum) || double.IsInfinity(sum))
            {
                sum = 1;
            }

            switch (layerType)
            {
                case Network.LayerType.OUTPUT:
                    {
                        if (sum > 10)
                        {
                            sum /= Math.Pow(10, Math.Floor(Math.Log10(sum)));
                        }
                        else if (sum < -10)
                        {
                            sum /= Math.Pow(10, Math.Floor(Math.Log10(-sum)));
                        }
                        sum = 1 / (1 + Math.Exp(-sum));
                        break;
                    }
                case Network.LayerType.HIDDEN:
                    {
                        if (sum > 10)
                        {
                            sum /= Math.Pow(10, Math.Floor(Math.Log10(sum)));
                        }
                        else if (sum < -10)
                        {
                            sum /= Math.Pow(10, Math.Floor(Math.Log10(-sum)));
                        }
                        sum = Max(sum, sum * NeuralNetworkConstructor.NetworkParameters.alpha);
                        break;
                    }
            }

            return sum;
        }
        private double GetHiddenNeuronActivation(double[] inputs, double[] weights)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; ++i)
            {
                sum += inputs[i] * weights[i];
            }
            if (double.IsNaN(sum) || double.IsInfinity(sum))
            {
                sum = 1;
            }

            if (sum > 10)
            {
                sum /= Math.Pow(10, Math.Floor(Math.Log10(sum)));
            }
            else if (sum < -10)
            {
                sum /= Math.Pow(10, Math.Floor(Math.Log10(-sum)));
            }
            sum = Max(sum, sum * NeuralNetworkConstructor.NetworkParameters.alpha);

            return sum;
        }
    }
}
