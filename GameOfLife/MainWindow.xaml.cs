using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Frame _currentFrame;
        public MainWindow()
        {
            InitializeComponent();
            _currentFrame = new Frame(1800, 1000, PixelFormats.Bgr24);
            _currentFrame.FillPixels(_currentFrame.RandomizeLife);

            gameOfLifeBoard.Source = _currentFrame.GetBitmapSource();
        }
    }

    internal class Frame
    {
        private readonly PixelFormat _pixelFormat;
        private int BytesPerPixel => _pixelFormat.BitsPerPixel / 8;
        private int Stride => Width * BytesPerPixel;
        private readonly byte[] _pixelBytes;

        public int Width { get; }
        public int Height { get; }

        private readonly Pixel[,] _pixels;

        public Frame(int width, int height, PixelFormat pixelFormat)
        {
            _pixelFormat = pixelFormat;
            Width = width;
            Height = height;
            _pixels = new Pixel[width, height];
            _pixelBytes = new byte[BytesPerPixel * Width * Height];
        }

        public Pixel GetPixel(int posX, int posY)
        {
            return _pixels[posX, posY];
        }

        public void FillPixels(Func<int, int, Pixel> fillPixel)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    _pixels[x, y] = fillPixel(x, y);
                }
            }
        }

        public Pixel RandomizeLife(int x, int y)
        {
            var gen = new Random();

            return gen.Next(100) <= 2 ?
                Pixel.Black() :
                Pixel.White();
        }


        public BitmapSource GetBitmapSource()
        {
            for (var i = 0; i < _pixelBytes.Length; i += BytesPerPixel)
            {
                var posX = (i / BytesPerPixel) % Width;
                var posY = (int)Math.Floor((decimal)(i / BytesPerPixel) / Width);
                var pix = GetPixel(posX, posY);
                _pixelBytes[i + 0] = Convert.ToByte(pix.Blue);
                _pixelBytes[i + 1] = Convert.ToByte(pix.Green);
                _pixelBytes[i + 2] = Convert.ToByte(pix.Red);
            }

            return BitmapSource.Create(Width, Height, 96d, 96d, _pixelFormat, null, _pixelBytes, Stride);
        }

    }

    internal class Pixel
    {
        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }
        public int Alpha { get; }

        public Pixel(int red = 0, int green = 0, int blue = 0, int alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public static Pixel Black()
        {
            return new Pixel();
        }
        public static Pixel White()
        {
            return new Pixel(255, 255, 255);
        }
    }
}