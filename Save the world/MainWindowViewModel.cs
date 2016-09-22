using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Save_the_world
{
    public class MainWindowViewModel : NotifyPropertyChangedViewModel
    {
        private string _text = "Game Over";
        private string _wintext = "You won!";
        public int Fortschritt { get; } = 0;

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string WinText
        {
            get { return _wintext; }
            set
            {
                if (value == _wintext) return;
                _wintext = value;
                OnPropertyChanged();
            }
        }
        public BitmapSource Panda => System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            Properties.Resources.panda.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        private void doSomething()
        {
            Text = "DoSomething";
        }
    }
}