using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using WSApp.DataModel;
using WSApp.ViewModel;

namespace WSApp
{

    #region Template Selector

    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class AboutTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Button
        {
            get;
            set;
        }

        public DataTemplate Prominent
        {
            get;
            set;
        }

        public DataTemplate Subtle
        {
            get;
            set;
        }

        public DataTemplate Picture
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            AboutItemViewModel aboutItem = item as AboutItemViewModel;
            if (aboutItem != null)
            {
                switch (aboutItem.Type)
                {
                    case AboutItemViewModel.AboutType.address:
                        return Button;
                    case AboutItemViewModel.AboutType.phone:
                        return Button;
                    case AboutItemViewModel.AboutType.sms:
                        return Button;
                    case AboutItemViewModel.AboutType.web:
                        return Button;
                    case AboutItemViewModel.AboutType.email:
                        return Button;
                    case AboutItemViewModel.AboutType.comments:
                        return Subtle;
                    case AboutItemViewModel.AboutType.picture:
                        return Picture;
                    case AboutItemViewModel.AboutType.general:
                        return Prominent;
                    default:
                        return Prominent;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
    
    #endregion

    public partial class HostPivotPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton pinButton = null;
        private ApplicationBarIconButton contactButton = null;
        private ApplicationBarIconButton feedbackButton = null;
//        private ApplicationBarIconButton searchButton = null;
        private ApplicationBarMenuItem viewHostMenu = null;
        private Uri pinnedUri;
        private Uri unpinnedUri;
        private int selectedUid;

        #region Initialization and Pivot Management

        // Constructor
        public HostPivotPage()
        {
            this.Resources.Add("HideEmptyStringConverter", new HideEmptyStringConverter());

            InitializeComponent();

            DataContext = App.ViewModelHost;

            setAppBar();
        }

        private void HostPivotPage_GotFocus(object sender, RoutedEventArgs e)
        {
            // Refresh state of pinned appbar button
            if (App.pinned.isPinned(selectedUid))
            {
                pinButton.IconUri = pinnedUri;
                pinButton.Text = WebResources.unpin;
            }
            else
            {
                pinButton.IconUri = unpinnedUri;
                pinButton.Text = WebResources.pin;
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String id = NavigationContext.QueryString["id"];
            int.TryParse(id, out selectedUid);

            int selectedIndex = App.nearby.selectedPage; // Remember last page viewed

            PinnedItemViewModel found = App.ViewModelMain.FindItemInPinnedList(selectedUid);
            if (null != found)
            {
                if (found.newMessageCount > 0)
                {   // Go directly to messages page if we have new messages
                    selectedIndex = 3;
                }
            }
            HostPivot.SelectedIndex = selectedIndex;
            App.ViewModelHost.username = App.nearby.getDecoratedUsername(selectedUid);
            App.ViewModelHost.uId = selectedUid;

            App.ViewModelHost.LoadData();   // Load as much info as we have before host requests return
        }

        private void setAppBar()
        {
            pinnedUri = new Uri("/Images/Appbar/appbar.pin.pinned.png", UriKind.Relative);
            unpinnedUri = new Uri("/Images/Appbar/appbar.pin.unpinned.png", UriKind.Relative);

            Uri initialPinnedUri;
            string initialPinnedText;
            // Create pin/unpin button
            if (App.pinned.isPinned())
            {
                initialPinnedUri = pinnedUri;
                initialPinnedText = WebResources.unpin;
            }
            else
            {
                initialPinnedUri = unpinnedUri;
                initialPinnedText = WebResources.pin;
            }
            pinButton = new ApplicationBarIconButton(initialPinnedUri);
            pinButton.Text = initialPinnedText;
            pinButton.Click += ApplicationBarIconButton_Click_Pin;
            ApplicationBar.Buttons.Add(pinButton);

            // Create contact button
            contactButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.reply.people.png", UriKind.Relative));
            contactButton.Text = "contact";
            contactButton.Click += ApplicationBarIconButton_Click_Contact;
            ApplicationBar.Buttons.Add(contactButton);

            // Create feedback button
            feedbackButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.feedback.png", UriKind.Relative));
            feedbackButton.Text = "feedback";
            feedbackButton.Click += ApplicationBarIconButton_Click_Feedback;
            ApplicationBar.Buttons.Add(feedbackButton);

            // Create map button
            //            searchButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.map.png", UriKind.Relative));
            //            searchButton.Text = "map";
            //            searchButton.Click += ApplicationBarIconButton_Click_Map;
            //            ApplicationBar.Buttons.Add(searchButton);

            if (null == viewHostMenu)
            {   // Create menu item to launch web site
                viewHostMenu = new ApplicationBarMenuItem();
                viewHostMenu.Text = WebResources.viewHostMenuText;
                viewHostMenu.Click += viewHostMenu_Click;
//                ApplicationBar.MenuItems.Add(viewHostMenu);  Todo:  figure out how to get cookies working with this
            }
        }

        private void viewHostMenu_Click(object sender, EventArgs e)
        {   // No way to get cookies to this so request fails.  Maybe user WebBrowser control, but don't see how to do it there either.
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.uriPrefix + WebResources.WarmShowersUri + "/user/" + selectedUid, UriKind.Absolute);
            task.Show();
        }

        private void Pivot_SelectionChanged(object sender, EventArgs e)
        {
            App.nearby.selectedPage = ((Pivot)sender).SelectedIndex;
        }

        #endregion

        #region ApplicationBar handlers

        private void ApplicationBarIconButton_Click_Contact(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/ContactForm.xaml?origin=HostPage&id=" + selectedUid, UriKind.Relative)); });
        }

        private void ApplicationBarIconButton_Click_Feedback(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/FeedbackForm.xaml?id=" + selectedUid, UriKind.Relative)); });
        }

        private void ApplicationBarIconButton_Click_Pin(object sender, EventArgs e)
        {   // Toggle pin
            if (App.pinned.isPinned())
            {
                if (App.pinned.unPin(selectedUid))
                {
                    pinButton.IconUri = unpinnedUri;
                    pinButton.Text = WebResources.pin;
                }
            }
            else
            {
                if (App.pinned.pin())
                {
                    pinButton.IconUri = pinnedUri;
                    pinButton.Text = WebResources.unpin;
                }
            }
        }

        private void ApplicationBarIconButton_Click_Map(object sender, EventArgs e)
        {
            BingMapsTask bingMapsTask = new BingMapsTask();
            bingMapsTask.SearchTerm = App.nearby.host.profile.user_Result.latitude.ToString() + " " + App.nearby.host.profile.user_Result.longitude.ToString();
            bingMapsTask.ZoomLevel = 15;
            bingMapsTask.Show();
        }

        #endregion

        #region List Management

        private void MessagesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != e && MessagesListBox.SelectedIndex > -1)
            {
                MessagesItemViewModel vm = (MessagesItemViewModel)e.AddedItems[0];
                if (null != vm)
                {
                    int tId = vm.threadID;

                    if (0 == tId)
                    {   // This happens if user clicks on "none"
                        return;
                    }

                    WebService.GetMessageThread(tId); // Begin user query

                    if (tId != App.nearby.selectedTid)
                    {   // User changed, saved data is invalid
                        App.nearby.messageThread.message_result = null;
                        App.nearby.selectedTid = tId;
                    }

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    { NavigationService.Navigate(new Uri("/MessagePage.xaml?id=" + selectedUid, UriKind.Relative)); });
                    MessagesListBox.SelectedIndex = -1; // Enable re-selection.  
                }
            }
        }

        private void AboutListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != e && AboutListBox.SelectedIndex > -1)
            {
                AboutItemViewModel vm = (AboutItemViewModel)e.AddedItems[0];

                switch (vm.Type)
                {
                    case AboutItemViewModel.AboutType.address:
                        BingMapsTask bingMapsTask = new BingMapsTask();
                        bingMapsTask.SearchTerm = vm.Line2 + " " + vm.Line3;
                        bingMapsTask.ZoomLevel = 15;
                        bingMapsTask.Show();
                        App.pinned.pin();
                        break;
                    case AboutItemViewModel.AboutType.phone:
                        var phoneCallTask = new PhoneCallTask
                        {
                            DisplayName = App.nearby.getFullName(selectedUid),
                            PhoneNumber = vm.Line2
                        };
                        phoneCallTask.Show();
                        App.pinned.pin();
                        break;
                    case AboutItemViewModel.AboutType.sms:
                        SmsComposeTask smsComposeTask = new SmsComposeTask();
                        smsComposeTask.To = vm.Line2;
                        smsComposeTask.Body = WebResources.Salutation + " " + App.nearby.getFullName(selectedUid) + ",\n";
                        smsComposeTask.Show();
                        App.pinned.pin();
                        break;
                    case AboutItemViewModel.AboutType.web:
                        WebBrowserTask task = new WebBrowserTask();
                        task.Uri = new Uri(vm.Line2);
                        task.Show();
                        break;
                    case AboutItemViewModel.AboutType.email:
                        EmailComposeTask emailComposeTask = new EmailComposeTask();
                        emailComposeTask.To = vm.Line2;
                        emailComposeTask.Show();
                        App.pinned.pin();
                        break;
                    case AboutItemViewModel.AboutType.comments:
                        break;
                    case AboutItemViewModel.AboutType.picture:
                        break;
                    case AboutItemViewModel.AboutType.general:
                        break;
                    default:
                        break;
                }
                AboutListBox.SelectedIndex = -1; // Enable re-selection.  
            }
        }

        #endregion

        public class HideEmptyStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
            {
                var input = (string)value;
                return string.IsNullOrWhiteSpace(input) ? Visibility.Collapsed : Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
            {
                throw new NotImplementedException();
            }
        }
    }
}