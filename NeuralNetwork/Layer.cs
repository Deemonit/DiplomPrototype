using System;
using System.IO;
using System.Xml;
using NeuralNetworkConstructor;

namespace NeuralNetwork
{
    abstract class Layer
    {
        Random random;

        private double[,] _weights;

        public double[,] Weights { get => _weights; set { _weights = value; } }

        protected double learningRate = NetworkParameters.learningRate;//скорость обучения
        protected double lambda = NetworkParameters.regilarization;//регуляризация (L2)
        protected double beta = NetworkParameters.gradientMoment;//скорость обновления весов

        protected int curNeurons;
        protected int prevNeurons;
        protected Layer(int curNeurons, int prevNeurons, string layerName,Network.LayerType layerType)
        {
            random = new Random();
            this.curNeurons = curNeurons;
            this.prevNeurons = prevNeurons;

            _neurons = new Neuron[curNeurons];
            Neurons = Neurons;

            _weights = WeightInitialize(Network.MemoryMode.GET, layerName);
            //Инициировать нейроны с массивами весов
            for (int i = 0; i < curNeurons; i++)
            {
                //Массив весов для каждого нейрона
                double[] oneNeuronWeights = new double[prevNeurons];
                //Наполнение этого массива из массива всех весов слоя
                for (int j = 0; j < prevNeurons; j++)
                {
                    oneNeuronWeights[j] = _weights[i, j];
                }
                Neurons[i] = new Neuron(null, oneNeuronWeights, layerType);
            }
        }

        Neuron[] _neurons;
        public Neuron[] Neurons { get => _neurons; set => _neurons = value; }
        public double[] Data
        {
            set
            {
                for (int i = 0; i < Neurons.Length; ++i)
                {
                    Neurons[i].Inputs = value;
                }
            }
        }

        public double[,] WeightInitialize(Network.MemoryMode mode, string layerName)
        {

            double[,] _weights = new double[curNeurons, prevNeurons];
            XmlDocument weights_doc = new XmlDocument();
            XmlElement weights_root;
            if (File.Exists($"{Path.Combine(NetworkParameters.weightsPath, layerName)}.xml"))
            {
                weights_doc.Load($"{Path.Combine(NetworkParameters.weightsPath, layerName)}.xml");
                weights_root = weights_doc.DocumentElement;
            }
            else
            {
                weights_root = weights_doc.CreateElement("Weights");
                weights_doc.AppendChild(weights_root);
            }

            double limit = Math.Sqrt(2.0 / prevNeurons);
            int weightsElementCount = weights_root.ChildNodes.Count;
            if (weights_root.ChildNodes.Count < curNeurons * prevNeurons)
            {
                for (int i = 0; i < (curNeurons * prevNeurons) - weightsElementCount; i++)
                {
                    XmlElement weight = weights_doc.CreateElement("weight");
                    double weightValue = GeneratedWeightValue(random) * limit;
                    weight.InnerText = weightValue.ToString();//(random.Next(-99999, 99999) * 0.0001).ToString();
                    weights_root.AppendChild(weight);
                }
                weights_doc.Save($"{Path.Combine(NetworkParameters.weightsPath, layerName)}.xml");
            }

            switch (mode)
            {
                case Network.MemoryMode.GET:

                    for (int i = 0; i < _weights.GetLength(0); i++)
                    {
                        for (int j = 0; j < _weights.GetLength(1); j++)
                        {
                            _weights[i, j] = double.Parse(weights_root.ChildNodes.Item(_weights.GetLength(1) * i + j).InnerText.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    break;
                case Network.MemoryMode.SET:
                    //Переписать значение веса для каждого нейрона в слое (layerType)
                    for (int i = 0; i < _neurons.Length; i++)
                    {
                        for (int j = 0; j < _weights.GetLength(1); j++)
                        {
                            weights_root.ChildNodes.Item(_weights.GetLength(1) * i + j).InnerText = _neurons[i].Weights[j].ToString();
                        }
                    }
                    weights_doc.Save($"{Path.Combine(NetworkParameters.weightsPath, layerName)}.xml");
                    break;
            }
            return _weights;
        }
        //для прямых проходов
        public abstract void StraightPass(Network net, Layer nextLayer);
        //для обучения - обратное прохождение
        public abstract double[] MiniBatchBackwardPass(double[] errors);

        public double GetHiddenDerivative(double output)
        {
            return (output>0?1:output* NetworkParameters.alpha);
        }
        public double GetOutputDerivative(double output)
        {
            return output * (1 - output);
        }
        public double GetGradient(double error, double derivative)
        {
            return error * derivative;
        }
        private double GeneratedWeightValue()
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();

            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }
    }
}
