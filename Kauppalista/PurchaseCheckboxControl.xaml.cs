using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Kauppalista
{
    public partial class PurchaseCheckboxControl : UserControl
    {
        public delegate void DeleteItem(object sender, RoutedEventArgs e);
        public event DeleteItem DeletePurchase;

        private String _Purchase;
        public String Purchase
        {
            get { return _Purchase; }
            set { _Purchase = value; }
        }

        public PurchaseCheckboxControl()
        {
            InitializeComponent();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeletePurchase != null)
            {
                DeletePurchase(this, e);
            }
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextAnimationTap.Begin();
        }

        
    }
}
