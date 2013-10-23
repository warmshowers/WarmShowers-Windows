using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace WSApp
{
    public class RecentViewModel : INotifyPropertyChanged
    {
        public RecentViewModel()
        {
            this.Items = new ObservableCollection<NearbyItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<NearbyItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Items.Add(new NearbyItemViewModel() { Name = "Robert Fripp", Distance = "2.3 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Peter Hammill", Distance = "3.7 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Hugh Banton", Distance = "6.2 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Laurie Anderson", Distance = "6.6 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Adrian Belew", Distance = "8.2 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "David Lowery", Distance = "9.0 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Doug Martsch", Distance = "10.6 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Frank Black", Distance = "13.3 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Todd Rundgren", Distance = "18.1 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Ian Anderson", Distance = "19.2 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Steve Hackett", Distance = "21.3 miles from me" });
            this.Items.Add(new NearbyItemViewModel() { Name = "Robyn Hitchcock", Distance = "23.8 miles from me" });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}