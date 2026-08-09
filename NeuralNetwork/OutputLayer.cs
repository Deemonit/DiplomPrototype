using System;

namespace NeuralNetwork
{
    class OutputLayer : Layer
    {
        //private readonly double[,] gradientSum;
        Random random;
        public OutputLayer(int curNeurons, int prevNeurons, string type, Network.LayerType layerType) : base(curNeurons, prevNeurons, type, layerType)
        {
            random = new Random();
        }
        public override void StraightPass(Network net, Layer nextLayer)
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i].CalculateOutput();
                net.RESULTS[i] = Neurons[i].Output;
            }

            double[] probabilities = Softmax(net.RESULTS);

            for (int i = 0; i < probabilities.Length; i++)
            {
                net.RESULTS[i] = probabilities[i];
            }
        }

        //для обучения - обратное прохождение
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
        //            gradient = GetGradient(errors[i], GetOutputDerivative(Neurons[i].Output));
        //            gradient += lambda * Neurons[i].Weights[j];
        //            if (double.IsNaN(gradient) || double.IsInfinity(gradient))
        //            {
        //                gradient = 1;
        //            }

        //            update_speed[j] = beta * update_speed[j] - learningRate * gradient; // вычисляем новую скорость
        //            error_sum[j] += Neurons[i].Weights[j] * update_speed[j];
        //            Neurons[i].Weights[j] -= update_speed[j] * Neurons[i].Inputs[j];
        //        }
        //    }
        //    return error_sum;
        //}

        //для обучения - обратное прохождение для одного изображения
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

        //Функция для классификаторов с 1-им правильным ответом
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
