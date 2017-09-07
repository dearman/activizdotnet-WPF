using System;
using System.Windows;
using System.Windows.Input;

namespace ActivizWPF.Windows
{
    /// <summary>
    /// Interaction logic for FloatingTextBox.xaml
    /// </summary>
    public partial class FloatingTextBox : Window
    {
        public event EventHandler DataEntered;

        public FloatingTextBox()
        {
            InitializeComponent();
        }

        protected virtual void OnDataEntered(EventArgs e)
        {
            EventHandler handler = DataEntered;
            if (handler != null)
            {
                handler(this, e);
            }

            this.Hide();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnDataEntered(e);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            OnDataEntered(e);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            OnDataEntered(e);
        }
    }
}
