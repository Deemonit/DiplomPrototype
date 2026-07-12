using System;

namespace NeuralNetwork
{
    class OutputLayer : Layer
    {
        Random random;
        public OutputLayer(int curNeurons, int prevNeurons, string type, Network.LayerType layerType) : base(curNeurons, prevNeurons, type, layerType) { random = new Random(); }
        public override void StraightPass(Network net, Layer nextLayer)
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i].Output = Neurons[i].Output;
                net.RESULTS[i] = Neurons[i].Output;
            }
        }

        //для обучения - обратное прохождение
        public override double[] MiniBatchBackwardPass(double[] errors)
        {
            double[] error_sum = new double[prevNeurons];
            double[] update_speed = new double[prevNeurons];
            double gradient = 0;

            for (int j = 0; j < prevNeurons; j++)
            {
                error_sum[j] = 0;
                update_speed[j] = beta;
            }

            for (int i = 0; i < curNeurons; i++)
            {
                for (int j = 0; j < prevNeurons; j++)
                {
                    gradient = GetGradient(errors[i], GetOutputDerivative(Neurons[i].Output));
                    gradient += lambda * Neurons[i].Weights[j];
                    if (double.IsNaN(gradient) || double.IsInfinity(gradient))
                    {
                        gradient = 1;
                    }

                    update_speed[j] = beta * update_speed[j] - learningRate * gradient; // вычисляем новую скорость
                    error_sum[j] += Neurons[i].Weights[j] * update_speed[j];
                    Neurons[i].Weights[j] -= update_speed[j] * Neurons[i].Inputs[j];

                    if (double.IsNaN(Neurons[i].Weights[j]) || double.IsInfinity(Neurons[i].Weights[j]))
                    {
                        Neurons[i].Weights[j] = random.NextDouble();
                    }
                }
            }
            return error_sum;
        }
    }
}
