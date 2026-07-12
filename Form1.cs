using NeuralNetwork;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NeuralNetworkConstructor;

namespace DiplomPrototype
{
    public partial class Form1 : Form
    {
        private Network network;
        private Random random;

        private int _inpNeurons;

        public Form1()
        {
            InitializeComponent();
            SetDefaultValues();
        }
        private void SetDefaultValues()
        {
            _inpNeurons = drawSymbolsClass1.DATA_MATRIX.GetLength(0) * drawSymbolsClass1.DATA_MATRIX.GetLength(1);

            random = new Random();

            network = new Network(_inpNeurons, NetworkParameters.NEURONS_COUNT, NetworkParameters.ANSWER_SET.Length, drawSymbolsClass1);
        }

        private void Button_MakeRandomSymbolPrediction_Click(object sender, EventArgs e)
        {
            if (File.Exists(NetworkParameters.testSymbolsPath))
            {
                MakePrediction();
            }
            else
            {
                MessageBox.Show($"Файл {NetworkParameters.testSymbolsPath} в корневой папке проекта: {Path.Combine(Environment.CurrentDirectory, "DataSets")} не найден");
            }
        }

        private void UpdateResultInfo()
        {
            if (network.trueIndex >= 0)
            {
                Label_RightAnswer.Text = NetworkParameters.ANSWER_SET[network.trueIndex];
            }
            if (network.predictIndex >= 0)
            {
                Laber_NetworkAnswer.Text = NetworkParameters.ANSWER_SET[network.predictIndex];
            }
            Label_precitionCountToSession.Text = network.GetPredictionCountToSession().ToString();
            label_epochCount.Text = network.GetLossArray().Length.ToString();
        }

        private void MakePrediction()
        {
            network.PredictRandomTestSymbol();
            UpdateResultInfo();
            UpdateAccuracyGraphic();
        }

        private void Button_ResetWeights_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Прогресс обучения будет потерян. Задать весам случайное значение?", "Внимание!", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                network.ResetNeuralNetworkWeights();
                network = new Network(_inpNeurons, NetworkParameters.NEURONS_COUNT, NetworkParameters.ANSWER_SET.Length, drawSymbolsClass1);
                MessageBox.Show("Веса сброшени, и загружены в нейросеть с новыми значениями");
            }
        }


        private void drawSymbolsClass1_DoubleClick(object sender, EventArgs e)
        {
            drawSymbolsClass1.IsDrawMode = !drawSymbolsClass1.IsDrawMode;
        }

        private void Button_MatrixClear_Click(object sender, EventArgs e)
        {
            drawSymbolsClass1.ErasePicture();
            Laber_NetworkAnswer.Text = "";
            Label_RightAnswer.Text = "";
        }

        private void drawSymbolsClass1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawSymbolsClass1.IsDrawMode)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            drawSymbolsClass1.SetPixelColor(e.Y / drawSymbolsClass1.scaledWeight, e.X / drawSymbolsClass1.scaledHeight, 255);
                            network.StraightForward(drawSymbolsClass1.DATA_MATRIX);
                            UpdateResultInfo();
                            break;
                        }
                    case MouseButtons.Right:
                        {
                            drawSymbolsClass1.SetPixelColor(e.Y / drawSymbolsClass1.scaledWeight, e.X / drawSymbolsClass1.scaledHeight, 0);
                            network.StraightForward(drawSymbolsClass1.DATA_MATRIX);
                            UpdateResultInfo();
                            break;
                        }
                }
            }
        }

        private void Button_DownloadPucture_Click(object sender, EventArgs e)
        {
            network.StraightForward(drawSymbolsClass1.DATA_MATRIX);
            Laber_NetworkAnswer.Text = (NetworkParameters.ANSWER_SET[Array.IndexOf(network.RESULTS, network.RESULTS.Max())]).ToString();
        }

        private void Button_StartLearningSet_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Обучение займёт некоторое время. Запустить обучение?", "Внимание!", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                network.Train(network);
                UpdateAccuracyGraphic();
                UpdateLossGraphic();
                UpdateResultInfo();
            }
        }

        private void Button_UpdateStatistic_Click(object sender, EventArgs e)
        {
            UpdateAccuracyGraphic();
            UpdateLossGraphic();
            UpdateResultInfo();
        }

        private void UpdateAccuracyGraphic()
        {
            double[] accuracy = network.GetAccuracyArray();
            Chart_AccuracyGraphic.Series[0].Points.Clear();
            label_averageAccuracy.Text = accuracy.Average().ToString();
            for (int i = 0; i < accuracy.Length; i++)
            {
                Chart_AccuracyGraphic.Series[0].Points.AddXY(i, accuracy[i]);
                Chart_AccuracyGraphic.Series[0].Points[i].AxisLabel = NetworkParameters.ANSWER_SET[i];
            }
        }
        private void UpdateLossGraphic()
        {
            double[] lossToEpoch = network.GetLossArray();
            chart_lossToEpoch.Series[0].Points.Clear();
            for (int i = 0; i < lossToEpoch.Length; i++)
            {
                chart_lossToEpoch.Series[0].Points.AddXY(i + 1, lossToEpoch[i]);
            }
        }

        private void button_multipleTests_Click(object sender, EventArgs e)
        {
            if (File.Exists(NetworkParameters.testSymbolsPath))
            {
                network.AccuracyAssessment();
                UpdateAccuracyGraphic();
                UpdateResultInfo();
            }
            else
            {
                MessageBox.Show($"Файл {NetworkParameters.testSymbolsPath} в корневой папке проекта: {Path.Combine(Environment.CurrentDirectory, "DataSets")} не найден");
            }
        }
    }
}
