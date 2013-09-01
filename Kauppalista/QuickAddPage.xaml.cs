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
    public partial class QuickAddPage : PhoneApplicationPage
    {

        private List<PurchaseCheckboxControl> listPurchaseCheckBoxes = new List<PurchaseCheckboxControl>();

        public QuickAddPage()
        {
            InitializeComponent();
            UpdatePurchasePanel();
        }

        private void UpdatePurchasePanel()
        {
            List<String> listQuickPurchases = (App.Current as App).ListQuickPurchases;
            List<String> listPurchases = (App.Current as App).ListPurchases;
            List<String> listPurchasesToAdd = new List<String>();
            foreach (String purchase in listQuickPurchases)
            {
                if(!listPurchases.Contains(purchase))
                {
                    listPurchasesToAdd.Add(purchase);
                }
            }

            
            PurchasePanel.Children.Clear();
            PurchasePanel.RowDefinitions.Clear();
            RowDefinition rowDef = null;

            int currentRow = 0;
            foreach (String purchase in listPurchasesToAdd)
            {
                rowDef = new RowDefinition();
                rowDef.Height = GridLength.Auto;
                PurchasePanel.RowDefinitions.Add(rowDef);

                PurchaseCheckboxControl item = new PurchaseCheckboxControl();
                Grid.SetRow(item, currentRow);
                item.Purchase = purchase;
                item.TextBlockContent.Text = purchase;
                item.DeletePurchase += item_DeletePurchase;
                PurchasePanel.Children.Add(item);

                listPurchaseCheckBoxes.Add(item);
                currentRow++;
            }
        }


        private void item_DeletePurchase(object sender, EventArgs e)
        {
            List<String> listQuickPurchases = (App.Current as App).ListQuickPurchases;
            PurchaseCheckboxControl purchaseSender = (PurchaseCheckboxControl)sender;
            String purchase = purchaseSender.Purchase;
            foreach (String purchInList in listQuickPurchases)
            {
                if (purchase.Equals(purchInList))
                {
                    listQuickPurchases.Remove(purchInList);
                    break;
                }
            }
            UpdatePurchasePanel();

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                    NavigationService.Navigate( new Uri( "/MainPage.xaml", UriKind.RelativeOrAbsolute ) );                      
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdatePurchasePanel();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<String> listQuickPurchases = (App.Current as App).ListQuickPurchases;
            List<String> purchasesToDelete = new List<String>();
            foreach (PurchaseCheckboxControl item in listPurchaseCheckBoxes)
            {
                if (item.CheckBoxSelected.IsChecked == true)
                {
                    String purchase = item.Purchase;
                    foreach (String s in listQuickPurchases)
                    {
                        if(s.Equals(purchase)) 
                        {
                            purchasesToDelete.Add(s);
                        }
                    }                    
                }
            }

            foreach (String s in purchasesToDelete)
            {
                listQuickPurchases.Remove(s);
            }
            UpdatePurchasePanel();
        }
    }
}