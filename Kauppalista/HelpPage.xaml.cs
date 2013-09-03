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
    public partial class HelpPage : PhoneApplicationPage
    {

        /// <summary>
        /// Constructor, + the help text to be shown
        /// </summary>
        public HelpPage()
        {
            InitializeComponent();
            String ohje = "Lisätäksesi ostoksia kauppalistaan, kirjoita ostoksen nimi ylhäällä olevaan tekstilaatikkoon. Paina tämän jälkeen Enter tai \"Lisää\"-nappia.\n\n";
            ohje += "Mikäli haluat tallentaa ostokset helposti lisättäväksi seuraavaa kertaa varten, ne voi samalla lisätä pikalisäyslistaan. Tällöin paina valintaruutu \"Lisää pikalisäykseen\" pohjaan.\n\n";
            ohje += "Voit halutessasi poistaa ostoksia kauppalistalta tai pikalisäyslistalta painamalla roskakoria ostoksen vierestä. Mikäli haluat poistaa kaikki ostokset kauppalistalta, paina alavalikossa olevaa \"Tyhjennä\"-nappia.\n";
            TextBlockHelp.Text = ohje;
        }

        
        /// <summary>
        /// Navigate back to Mainpage
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventargs</param>         
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}