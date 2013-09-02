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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace Kauppalista
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        // Data context for the local database
        private PurchaseDataContext purchaseDB;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<PurchaseItem> _PurchaseItems;
        public ObservableCollection<PurchaseItem> PurchaseItems
        {
            get
            {
                return _PurchaseItems;
            }
            set
            {
                if (_PurchaseItems != value)
                {
                    _PurchaseItems = value;
                    NotifyPropertyChanged("PurchaseItems");
                }
            }
        }

        private QuickPurchaseDataContext quickPurchaseDB;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<QuickPurchaseItem> _QuickPurchaseItems;
        public ObservableCollection<QuickPurchaseItem> QuickPurchaseItems
        {
            get
            {
                return _QuickPurchaseItems;
            }
            set
            {
                if (_QuickPurchaseItems != value)
                {
                    _QuickPurchaseItems = value;
                    NotifyPropertyChanged("QuickPurchaseItems");
                }
            }
        }
        


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            purchaseDB = new PurchaseDataContext(PurchaseDataContext.DBConnectionString);
            quickPurchaseDB = new QuickPurchaseDataContext(QuickPurchaseDataContext.DBConnectionString);
            this.DataContext = this;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if(!settings.Contains("checkBoxQuickAdd"))
            {
                settings.Add("checkBoxQuickAdd", "checked");
                settings.Save();
            } else {
                String checkBoxQuickAddIsChecked = IsolatedStorageSettings.ApplicationSettings["checkBoxQuickAdd"] as String;
                if((checkBoxQuickAddIsChecked.Equals("checked")) == true) CheckBoxQuickAdd.IsChecked = true;
                else CheckBoxQuickAdd.IsChecked = false;
            }

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


        private void ApplicationBarMenuItemInfo_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxButton button = new MessageBoxButton();
            button = MessageBoxButton.OK;
            MessageBox.Show("© Matias Partanen\nmatias.partanen@jyu.fi", "Kauppalista v. 0.1", button);
        }

        private void BtnLisaa_Click(object sender, RoutedEventArgs e)
        {
            AddNewPurchase();
        }

        private void AddNewPurchase()
        {            
            String newItemText = TextBoxNew.Text;
            if (newItemText.Trim().Equals("")) return;
            foreach (PurchaseItem item in PurchaseItems)
            {
                if (item.ItemName.Equals(newItemText))
                {
                    MessageBox.Show("Ostos löytyy jo listalta.");
                    return;
                }
            }
            TextBoxNew.ClearValue(TextBox.TextProperty);
            PurchaseItem newItem = new PurchaseItem { ItemName = newItemText };
            PurchaseItems.Add(newItem);
            purchaseDB.PurchaseItems.InsertOnSubmit(newItem);
            List<String> listItemNames = new List<String>();
            if (CheckBoxQuickAdd.IsChecked == true)
            {
                foreach (QuickPurchaseItem quickPurchaseItem in QuickPurchaseItems)
                {
                    listItemNames.Add(quickPurchaseItem.ItemName);
                }
                if (!listItemNames.Contains(newItemText))
                {
                    QuickPurchaseItem newQuickItem = new QuickPurchaseItem { ItemName = newItemText };
                    quickPurchaseDB.QuickPurchaseItems.InsertOnSubmit(newQuickItem);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                PurchaseItem item = button.DataContext as PurchaseItem;
                PurchaseItems.Remove(item);
                purchaseDB.PurchaseItems.DeleteOnSubmit(item);
                purchaseDB.SubmitChanges();
                
            }
        }

        private void ApplicationBarIconButtonTyhjenna_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PurchaseItems.Count; i++)
            {
                PurchaseItem item = PurchaseItems[i];                
                purchaseDB.PurchaseItems.DeleteOnSubmit(item);
                purchaseDB.SubmitChanges();
            }
            PurchaseItems.Clear();
        }

        private void ApplicationBarMenuItemOhje_Click(object sender, EventArgs e)
        {

        }

        private void BtnQuickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/QuickAddPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var purchaseItemsInDB = from PurchaseItem purchase in purchaseDB.PurchaseItems                                    
                                    select purchase;

            // Execute the query and place the results into a collection.
            PurchaseItems = new ObservableCollection<PurchaseItem>(purchaseItemsInDB);

            var quickPurchaseItemsInDb = from QuickPurchaseItem quickPurchase in quickPurchaseDB.QuickPurchaseItems
                                         select quickPurchase;

            QuickPurchaseItems = new ObservableCollection<QuickPurchaseItem>(quickPurchaseItemsInDb);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);

            // Save changes to the database.
            purchaseDB.SubmitChanges();
            quickPurchaseDB.SubmitChanges();
        }

        private void CheckBoxQuickAdd_Tapped(object sender, RoutedEventArgs e)
        {
            if (CheckBoxQuickAdd == null) return;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;            
            if (CheckBoxQuickAdd.IsChecked == true)
            {
                if (!settings.Contains("checkBoxQuickAdd"))
                {
                    settings.Add("checkBoxQuickAdd", "checked");
                }
                else
                {
                    settings["checkBoxQuickAdd"] = "checked";
                }
            } else
            {
                if (!settings.Contains("checkBoxQuickAdd"))
                {
                    settings.Add("checkBoxQuickAdd", "notchecked");
                }
                else
                {
                    settings["checkBoxQuickAdd"] = "notchecked";
                }
            }
            settings.Save();
        }

        private void TextBoxNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewPurchase();
                this.Focus();
            }
        }

    }
}

//Database
[Table]
public class PurchaseItem : INotifyPropertyChanged, INotifyPropertyChanging
{
    // Define ID: private field, public property and database column.
    private int _PurchaseItemId;

    [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
    public int PurchaseItemId
    {
        get
        {
            return _PurchaseItemId;
        }
        set
        {
            if (_PurchaseItemId != value)
            {
                NotifyPropertyChanging("PurchaseItemId");
                _PurchaseItemId = value;
                NotifyPropertyChanged("PurchaseItemId");
            }
        }
    }

    // Define item name: private field, public property and database column.
    private string _itemName;

    [Column]
    public string ItemName
    {
        get
        {
            return _itemName;
        }
        set
        {
            if (_itemName != value)
            {
                NotifyPropertyChanging("ItemName");
                _itemName = value;
                NotifyPropertyChanged("ItemName");
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

public class PurchaseDataContext : DataContext
{
    // Specify the connection string as a static, used in main page and app.xaml.
    public static string DBConnectionString = "Data Source=isostore:/PurchaseDB.sdf";

    // Pass the connection string to the base class.
    public PurchaseDataContext(string connectionString)
        : base(connectionString)
    { }

    // Specify a single table for the to-do items.
    public Table<PurchaseItem> PurchaseItems;
}
