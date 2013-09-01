using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Kauppalista
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<String> listPurchases = new List<String>();
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarMenuItemInfo_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxButton button = new MessageBoxButton();
            button = MessageBoxButton.OK;
            MessageBox.Show("© Matias Partanen\nmatias.partanen@jyu.fi", "Kauppalista v. 0.1", button);
        }

        private void BtnLisaa_Click(object sender, RoutedEventArgs e)
        {
            String text = TextBoxNew.Text;
            if(text.Equals("")) return;
            foreach (String purchase in listPurchases)
            {
                if (text.Equals(purchase))
                {
                    MessageBox.Show("Ostos löytyy jo kauppalistalta");
                    return;
                }
            }
            TextBoxNew.ClearValue(TextBox.TextProperty);
            listPurchases.Add(text);            
            UpdatePurchasePanel();
        }

        private void ApplicationBarIconButtonTyhjenna_Click(object sender, EventArgs e)
        {
            listPurchases.Clear();
            UpdatePurchasePanel();
        }

        private void UpdatePurchasePanel()
        {
            listPurchases.Sort();
            PurchasePanel.Children.Clear();
            PurchasePanel.RowDefinitions.Clear();
            RowDefinition rowDef = null;

            int currentRow = 0;
            foreach (String purchase in listPurchases)
            {
                rowDef = new RowDefinition();
                rowDef.Height = GridLength.Auto;
                PurchasePanel.RowDefinitions.Add(rowDef);

                PurchaseControl item = new PurchaseControl();                
                item.Purchase = purchase;
                item.TextBlockContent.Text = purchase;
                item.DeletePurchase += item_DeletePurchase;
                Grid.SetRow(item, currentRow);
                PurchasePanel.Children.Add(item);

                currentRow++;
            }
        }

        private void item_DeletePurchase(object sender, EventArgs e)
        {
            PurchaseControl purchaseSender = (PurchaseControl)sender;
            String purchase = purchaseSender.Purchase;
            foreach (String purchInList in listPurchases)
            {
                if (purchase.Equals(purchInList))
                {
                    listPurchases.Remove(purchInList);
                    break;
                }
            }
            UpdatePurchasePanel();
        }

        private void BtnQuickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/QuickAddPage.xaml", UriKind.Relative));
        }
    }
}