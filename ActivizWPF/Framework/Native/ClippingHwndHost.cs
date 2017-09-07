using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;


namespace ActivizWPF.Framework.Native
{
    public class ClippingHwndHost : HwndHost
    {
        private HwndSource _source;

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(Visual), typeof(ClippingHwndHost),
            new PropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClippingHwndHost hwndHost = (ClippingHwndHost)d;

            if (e.OldValue != null)
            {
                if (hwndHost._source != null)
                    hwndHost._source.RootVisual = null;
                hwndHost.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                hwndHost.AddLogicalChild(e.NewValue);

                if (hwndHost._source != null)
                    hwndHost._source.RootVisual = (Visual)e.NewValue;
            }
        }

        public Visual Content
        {
            get { return (Visual)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public ClippingHwndHost()
        {
            //InteropFocusTracking.SetIsEnabled(this, true);
        }

        #region HwndHost


        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Content != null)
                    yield return Content;
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var param = new HwndSourceParameters("ClippingHwndHost", (int)Width, (int)Height)
            {
                ParentWindow = hwndParent.Handle,
                WindowStyle = Win32.WS_VISIBLE | Win32.WS_CHILD,
            };

            _source = new HwndSource(param)
            {
                RootVisual = Content
            };

            return new HandleRef(null, _source.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _source.Dispose();
            _source = null;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // If we don't do this, HwndHost doesn't seem to pick up on all size changes.
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);
        }

        #endregion
    }
}
