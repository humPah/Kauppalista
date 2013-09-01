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
            List<String> listPurchases = (App.Current as App).ListPurchases;
            List<String> listQuickPurchases = (App.Current as App).ListQuickPurchases;
            String text = TextBoxNew.Text;
            if(text.Equals("")) return;
            if(listPurchases.Contains(text))
            {
                MessageBox.Show("Ostos löytyy jo kauppalistalta");
                return;
            }
            listPurchases.Add(text);    
            if (!listQuickPurchases.Contains(text))
            {
                listQuickPurchases.Add(text);
            }
            

            TextBoxNew.ClearValue(TextBox.TextProperty);         
         
            
            UpdatePurchasePanel();
        }

        private void ApplicationBarIconButtonTyhjenna_Click(object sender, EventArgs e)
        {
            List<String> listPurchases = (App.Current as App).ListPurchases;
            listPurchases.Clear();
            UpdatePurchasePanel();
        }

        private void UpdatePurchasePanel()
        {
            List<String> listPurchases = (App.Current as App).ListPurchases;
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
            List<String> listPurchases = (App.Current as App).ListPurchases;
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdatePurchasePanel();
        }
    }
}