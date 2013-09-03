using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Kauppalista
{
    public partial class QuickAddPage : PhoneApplicationPage, INotifyPropertyChanged
    {

        /// <summary>
        /// Dynamic list and DB link of quick purchase items
        /// </summary>
        private QuickPurchaseDataContext quickPurchaseDB;
        private PurchaseDataContext purchaseDB;

        private ObservableCollection<QuickPurchaseItem> _quickPurchaseItems;
        public ObservableCollection<QuickPurchaseItem> QuickPurchaseItems
        {
            get
            {
                return _quickPurchaseItems;
            }
            set
            {
                if (_quickPurchaseItems != value)
                {
                    _quickPurchaseItems = value;
                    NotifyPropertyChanged("QuickPurchaseItems");
                }
            }
        }

        /// <summary>
        /// Dynamic list and DB link of purchase items
        /// </summary>
        private ObservableCollection<PurchaseItem> _purchaseItems;
        public ObservableCollection<PurchaseItem> PurchaseItems
        {
            get
            {
                return _purchaseItems;
            }
            set
            {
                if (_purchaseItems != value)
                {
                    _purchaseItems = value;
                    NotifyPropertyChanged("PurchaseItems");
                }
            }
        }

        /// <summary>
        /// Constructor and setting DB connections
        /// </summary>
        public QuickAddPage()
        {
            InitializeComponent();
            quickPurchaseDB = new QuickPurchaseDataContext(QuickPurchaseDataContext.DBConnectionString);
            purchaseDB = new PurchaseDataContext(PurchaseDataContext.DBConnectionString);
            this.DataContext = this;
        }
        
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        
        /// <summary>
        /// Navigate back to MainPage
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventargs</param>
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


        /// <summary>        
        /// Define the query to gather all of the items, after execute the query and place the results into a collection
        /// Sort quickpurchases by itemname
        /// </summary>
        /// <param name="e">eventargs</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var purchaseItemsInDB = from PurchaseItem purchase in purchaseDB.PurchaseItems                                    
                                    select purchase;

            PurchaseItems = new ObservableCollection<PurchaseItem>(purchaseItemsInDB);


            // Define the query to gather all of the to-do items.
            var quickPurchaseItemsInDB = from QuickPurchaseItem quickpurchase in quickPurchaseDB.QuickPurchaseItems
                                         orderby quickpurchase.ItemName
                                         select quickpurchase;

            // Execute the query and place the results into a collection.
            QuickPurchaseItems = new ObservableCollection<QuickPurchaseItem>(quickPurchaseItemsInDB);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Delete the selected object associated through datacontext
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventargs</param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                QuickPurchaseItem itemToDelete = button.DataContext as QuickPurchaseItem;

                // Remove the to-do item from the observable collection.
                QuickPurchaseItems.Remove(itemToDelete);

                // Remove the to-do item from the local database.
                quickPurchaseDB.QuickPurchaseItems.DeleteOnSubmit(itemToDelete);                

                // Save changes to the database.
                quickPurchaseDB.SubmitChanges();
            }
        }
        
        /// <summary>
        /// Save changes to databases
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Save changes to the database.
            quickPurchaseDB.SubmitChanges();
            purchaseDB.SubmitChanges();
        }


        /// <summary>
        /// Check the selected items user wants to add, after which check and remove the multiple entries, and add the rest to database
        /// Once finished, navigate back to MainPage
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventargs</param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            List<String> itemsToBeAdded = new List<String>();
            foreach (QuickPurchaseItem item in QuickPurchaseItems)
            {
                if (item.IsChecked == true)
                {
                    itemsToBeAdded.Add(item.ItemName);                       
                }
            }
            
            List<String> currentItemNames = new List<String>();
            foreach (PurchaseItem item in PurchaseItems)
            {
                currentItemNames.Add(item.ItemName);
            }


            foreach (String itemString in itemsToBeAdded)
            {
                if (!currentItemNames.Contains(itemString))
                {
                    PurchaseItem newItem = new PurchaseItem();
                    newItem.ItemName = itemString;
                    PurchaseItems.Add(newItem);
                    purchaseDB.PurchaseItems.InsertOnSubmit(newItem);
                }
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute)); 
        }
    }
}

/// <summary>
/// Database-classes
/// </summary>
[Table]
public class QuickPurchaseItem : INotifyPropertyChanged, INotifyPropertyChanging
{
    // Define ID: private field, public property and database column.
    private int _QuickPurchaseItemId;

    [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
    public int QuickPurchaseItemId
    {
        get
        {
            return _QuickPurchaseItemId;
        }
        set
        {
            if (_QuickPurchaseItemId != value)
            {
                NotifyPropertyChanging("QuickPurchaseItemId");
                _QuickPurchaseItemId = value;
                NotifyPropertyChanged("QuickPurchaseItemId");
            }
        }
    }

    // Define item name: private field, public property and database column.
    private string _ItemName;

    [Column]
    public string ItemName
    {
        get
        {
            return _ItemName;
        }
        set
        {
            if (_ItemName != value)
            {
                NotifyPropertyChanging("ItemName");
                _ItemName = value;
                NotifyPropertyChanged("ItemName");
            }
        }
    }

    private bool _IsChecked;

    [Column]
    public bool IsChecked
    {
        get
        {
            return _IsChecked;
        }
        set
        {
            if (_IsChecked != value)
            {
                NotifyPropertyChanging("IsChecked");
                _IsChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }
    }

    // Version column aids update performance.
    [Column(IsVersion = true)]
    private Binary _version;

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    // Used to notify the page that a data context property changed
    private void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region INotifyPropertyChanging Members

    public event PropertyChangingEventHandler PropertyChanging;

    // Used to notify the data context that a data context property is about to change
    private void NotifyPropertyChanging(string propertyName)
    {
        if (PropertyChanging != null)
        {
            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
    }

    #endregion
}

public class QuickPurchaseDataContext : DataContext
{
    // Specify the connection string as a static, used in main page and app.xaml.
    public static string DBConnectionString = "Data Source=isostore:/QuickPurchaseDB.sdf";

    // Pass the connection string to the base class.
    public QuickPurchaseDataContext(string connectionString)
        : base(connectionString)
    { }

    // Specify a single table for the to-do items.
    public Table<QuickPurchaseItem> QuickPurchaseItems;
}