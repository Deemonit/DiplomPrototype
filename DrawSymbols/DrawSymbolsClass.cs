using System.Drawing;
using System.Windows.Forms;

namespace DrawSymbols
{
    public class DrawSymbolsClass : Control
    {
        public DrawSymbolsClass(int rows, int columns, int standartWeight, int standartHeight, int drawWeight, int drawHeight) : base()
        {
            _matrixRows = rows;
            _matrixColumns = columns;

            _DATA_MATRIX = new int[_matrixRows, _matrixColumns];
            DATA_MATRIX = DATA_MATRIX;

            this._standartWeight = standartWeight;
            this._standartHeight = standartHeight;

            this.drawWeight = drawWeight;
            this.drawHeight = drawHeight;

            _currentWeight = this._standartWeight;
            _currentHeight = this._standartHeight;

            _isDrawMode = false;
            scaledWeight = _currentWeight / _DATA_MATRIX.GetLength(1);
            scaledHeight = _currentHeight / _DATA_MATRIX.GetLength(0);
        }

        //public DrawSymbolsClass() : base()
        //{
        //    _matrixRows = 1;
        //    _matrixColumns = 1;

        //    _DATA_MATRIX = new int[_matrixRows, _matrixColumns];
        //    DATA_MATRIX = DATA_MATRIX;

        //    this._standartWeight = 1;
        //    this._standartHeight = 1;

        //    this.drawWeight = 1;
        //    this.drawHeight = 1;

        //    _currentWeight = this._standartWeight;
        //    _currentHeight = this._standartHeight;

        //    _isDrawMode = false;
        //    scaledWeight = _currentWeight / _DATA_MATRIX.GetLength(1);
        //    scaledHeight = _currentHeight / _DATA_MATRIX.GetLength(0);
        //}

        public int scaledWeight;
        public int scaledHeight;

        private int _matrixRows;
        private int _matrixColumns;

        private int[,] _DATA_MATRIX;

        private bool _isDrawMode;

        public int _standartWeight;
        public int _standartHeight;

        public int drawWeight;
        public int drawHeight;

        public int _currentWeight;
        public int _currentHeight;

        public int[,] DATA_MATRIX
        {
            get { return _DATA_MATRIX; }
            set { _DATA_MATRIX = value;Invalidate(); }
        }


        public bool IsDrawMode
        {
            get
            {
                return _isDrawMode;
            }
            set
            {
                if (_isDrawMode != value)
                {
                    _isDrawMode = value;
                    if (IsDrawMode)
                    {
                        _currentWeight = drawWeight;
                        _currentHeight = drawHeight;

                        Height = drawHeight;
                        Width = drawWeight;
                    }
                    else
                    {
                        _currentWeight = _standartWeight;
                        _currentHeight = _standartHeight;

                        Height = _standartWeight;
                        Width = _standartWeight;
                    }
                    scaledWeight = _currentWeight / _DATA_MATRIX.GetLength(1);
                    scaledHeight = _currentHeight / _DATA_MATRIX.GetLength(0);
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush pixelColor=null;
            for (int i = 0; i < DATA_MATRIX.GetLength(0); i++)
            {
                for (int j = 0; j < DATA_MATRIX.GetLength(1); j++)
                {
                    pixelColor = new SolidBrush(Color.FromArgb(DATA_MATRIX[i, j], DATA_MATRIX[i, j], DATA_MATRIX[i, j]));
                    e.Graphics.FillRectangle(pixelColor, 0 + scaledWeight * j, 0 + scaledHeight * i, scaledWeight, scaledHeight);
                }
            }
            pixelColor.Dispose();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = 0x02000000;
                return cp;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            Width = width;
            Height = height;
            Invalidate();
        }

        public void DownloadSymbolToMatrix(string[] row)
        {
            int i, j;

            for (int k = 1; k < row.Length; k++)
            {
                i = (k - 1) / _DATA_MATRIX.GetLength(1);
                i%= _DATA_MATRIX.GetLength(0);

                j = (k - 1) - i * _DATA_MATRIX.GetLength(1);
                j %= _DATA_MATRIX.GetLength(1);

                _DATA_MATRIX[i, j] = int.Parse(row[k]);
            }
        }

        public void ErasePicture()
        {
            for (int i = 0; i < DATA_MATRIX.GetLength(0); i++)
            {
                for (int j = 0; j < DATA_MATRIX.GetLength(1); j++)
                {
                    DATA_MATRIX[i, j] = 0;
                }
            }
            Invalidate();
        }

        public void SetPixelColor(int row, int col, int tone)
        {
            if (row > DATA_MATRIX.GetLength(0) - 1)
            {
                row = DATA_MATRIX.GetLength(0) - 1;
            }
            if (row < 0)
            {
                row = 0;
            }
            if (col > DATA_MATRIX.GetLength(1) - 1)
            {
                col = DATA_MATRIX.GetLength(1) - 1;
            }
            if (col < 0)
            {
                col = 0;
            }
            DATA_MATRIX[row, col] = tone;
            Invalidate();
        }

        public void MNISTNormalizeMatrix()
        {
            HorizontalReflectMatrix();
            TurnLeftMatrix();
        }

        public void TurnRightMatrix()
        {
            int n = DATA_MATRIX.GetLength(0);
            int m = DATA_MATRIX.GetLength(1);
            for (int i = 0; i < n / 2; i++)
            {
                for (int j = i; j < m - i - 1; j++)
                {
                    int temp = DATA_MATRIX[i, j];
                    DATA_MATRIX[i, j] = DATA_MATRIX[m - j - 1, i];
                    DATA_MATRIX[m - j - 1, i] = DATA_MATRIX[n - i - 1, m - j - 1];
                    DATA_MATRIX[n - i - 1, m - j - 1] = DATA_MATRIX[j, n - i - 1];
                    DATA_MATRIX[j, n - i - 1] = temp;
                }
            }
            Invalidate();
        }
        public void TurnLeftMatrix()
        {
            int n = DATA_MATRIX.GetLength(0);
            int m = DATA_MATRIX.GetLength(1);
            for (int i = 0; i < n / 2; i++)
            {
                for (int j = i; j < m - i - 1; j++)
                {
                    int temp = DATA_MATRIX[i, j];
                    DATA_MATRIX[i, j] = DATA_MATRIX[j, n - i - 1];
                    DATA_MATRIX[j, n - i - 1] = DATA_MATRIX[n - i - 1, m - j - 1];
                    DATA_MATRIX[n - i - 1, m - j - 1] = DATA_MATRIX[m - j - 1, i];
                    DATA_MATRIX[m - j - 1, i] = temp;
                }
            }
            Invalidate();
        }
        public void HorizontalReflectMatrix()
        {
            int n = DATA_MATRIX.GetLength(0);
            int m = DATA_MATRIX.GetLength(1);
            int buf;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m / 2; j++)
                {
                    buf = DATA_MATRIX[i, j];
                    DATA_MATRIX[i, j] = DATA_MATRIX[i, m - 1 - j];
                    DATA_MATRIX[i, m - 1 - j] = buf;
                }
            }
            Invalidate();
        }
    }
}
