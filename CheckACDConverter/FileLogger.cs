using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CheckACDConverter
{
    public class FileLogger
    {
        private StorageFolder _folder;
        private StorageFile _file;
        private int _id = 0;

        public FileLogger(string filename)
        {
            OpenFileAsync(filename);
        }

        private async void OpenFileAsync(string filename)
        {
            _folder = ApplicationData.Current.LocalCacheFolder;
            _file = await _folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
        }

        public async Task WriteLogAsync(string s)
        {
            string now = DateTime.Now.ToString();
            try
            {
                await FileIO.AppendTextAsync(_file, (_id++) + " : " + now + " : " + s + '\r' + '\n');
            }
            catch { }
        }

        public void Close()
        {
            
        }
    }
}
