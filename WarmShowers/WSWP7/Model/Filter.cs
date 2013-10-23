using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using WSApp.ViewModel;

namespace WSApp.DataModel
{
    public class Filter
    {
        IsolatedStorageFile store;
        const string filterFilename = "WSFilter";

        private void Load()
        {
            store = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream stream = store.OpenFile(filterFilename, FileMode.OpenOrCreate);
            if (null != stream)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FilterViewModel));
                App.ViewModelFilter = (FilterViewModel)ser.ReadObject(stream);
                stream.Close();

                if (null == App.ViewModelFilter)
                {   // First time app was run since installation
                    App.ViewModelFilter = new FilterViewModel();
                }
            }
        }

        private void Save()
        {
            store = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream stream = store.OpenFile(filterFilename, FileMode.Truncate);
            if (null != stream)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FilterViewModel));
                ser.WriteObject(stream, App.ViewModelFilter);
                stream.Close();
            }
        }
    }
}
