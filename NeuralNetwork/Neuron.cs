using System;
using static System.Math;
using NeuralNetworkConstructor;

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

            return sum;
        }
        private double GetHiddenNeuronActivation(double[] inputs, double[] weights)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; ++i)
            {
                sum += inputs[i] * weights[i];
            }

            return LeakyReLU(sum);
        }

        private double LeakyReLU(double value)
        {
            return value >= 0 ? value: NetworkParameters.alpha * value;
        }

        public double CalculateOutput()
        {
            //Тип нейрона либо скрытый, либо выходной. У входного слоя нет нейронов. Там просто массив пикселей
            _output = layerType == Network.LayerType.OUTPUT ? GetOutputNeuronActivation(_inputs, _weights) : GetHiddenNeuronActivation(_inputs, _weights);

            return _output;
        }
    }
}
