using System;

namespace NeuralNetwork
{
    class HiddenLayer : Layer
    {
        // Сумма градиентов весов скрытого слоя по всем изображениям текущего mini-batch.
        //
        // Первый индекс — скрытый нейрон.
        // Второй индекс — нейрон или значение предыдущего слоя.
        private readonly double[,] gradientSum;

        Random random;
        public HiddenLayer(int curNeurons, int prevNeurons, string type, Network.LayerType layerType) : base(curNeurons, prevNeurons, type, layerType) 
        { 
            random = new Random();

            // Для каждого веса скрытого слоя создаём отдельный накопитель градиента.
            gradientSum = new double[curNeurons, prevNeurons];
        }
        public override void StraightPass(Network net, Layer nextLayer)
        {
            double[] hiddenOut = new double[Neurons.Length];
            for (int i = 0; i < hiddenOut.Length; i++)
            {
                Neurons[i].CalculateOutput();
                hiddenOut[i] = Neurons[i].Output;
            }
            nextLayer.Data = hiddenOut;
        }

        public override double[] MiniBatchBackwardPass(double[] outputDelta)
        {
            double[] previousLayerErrors = new double[prevNeurons];
            //double[] update_speed = new double[prevNeurons];

            for (int outputIndex = 0; outputIndex < curNeurons; outputIndex++)
            {
                double delta = outputDelta[outputIndex];

                for (int hiddenIndex = 0; hiddenIndex < prevNeurons; hiddenIndex++)
                {
                    double hiddenOutput = Neurons[outputIndex].Inputs[hiddenIndex];
                    double currentWeight = Neurons[outputIndex].Weights[hiddenIndex];
                    // Градиент конкретного веса:
                    // дельта выходного нейрона × значение соответствующего скрытого нейрона (связанного нейрона на предыдущем слое).
                    double weightGradient = delta * hiddenOutput;

                    gradientSum[outputIndex, hiddenIndex] += weightGradient;

                    // Считаем ошибку для скрытого нейрона
                    // Она понадобится при обучении предыдущего слоя
                    previousLayerErrors[hiddenIndex] += currentWeight * delta;
                }
            }
            return previousLayerErrors;
        }

        //public override double[] MiniBatchBackwardPass(double[] errors)
        //{
        //    double[] error_sum = new double[prevNeurons];
        //    double[] update_speed = new double[prevNeurons];
        //    double gradient = 0;

        //    for (int j = 0; j < prevNeurons; j++)
        //    {
        //        error_sum[j] = 0;
        //        update_speed[j] = beta;
        //    }

        //    for (int i = 0; i < curNeurons; i++)
        //    {
        //        for (int j = 0; j < prevNeurons; j++)
        //        {
        //            gradient = GetGradient(errors[i], GetHiddenDerivative(Neurons[i].Output));
        //            gradient += lambda * Neurons[i].Weights[j];
        //            if (double.IsNaN(gradient) || double.IsInfinity(gradient))
        //            {
        //                gradient = 1;
        //            }

        //            update_speed[j] = beta * update_speed[j] - learningRate * gradient; // вычисляем новую скорость
        //            error_sum[j] += Neurons[i].Weights[j] * update_speed[j];
        //            Neurons[i].Weights[j] -= update_speed[j] * Neurons[i].Inputs[j];

        //            if (double.IsNaN(Neurons[i].Weights[j]) || double.IsInfinity(Neurons[i].Weights[j]))
        //            {
        //                Neurons[i].Weights[j] = random.NextDouble();
        //            }
        //        }
        //    }
        //    return error_sum;
        //}
    }
}
