using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ActivizWPF.Windows
{
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window, INotifyPropertyChanged
    {
        private bool _resizeX;
        private bool _resizeY;

        public bool ResizeX
        {
            get { return _resizeX; }
            set
            {
                _resizeX = value;
                this.Height = _resizeX ? this.MaxHeight : 0;
            }
        }

        public bool ResizeY
        {
            get { return _resizeY; }
            set
            {
                _resizeY = value;
                this.Width = _resizeY ? this.MaxWidth : 0;
            }
        }

        public Point StartPoint { get; set; }

        public ResizeWindow()
        {
            InitializeComponent();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point point = MousePoint.GetCursorPosition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
