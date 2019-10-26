using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using ImageDebugger.Core.ViewModels;

namespace ImageDebugger.Core.Models
{
    public class AutoSerializableBase<T> : ViewModelBase
    {
        private void Serialize(object sender, PropertyChangedEventArgs e)
        {
            var serializePath = GetSerializationPath();   
            using (var fs = new FileStream(serializePath, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, this);
            }
        }

        protected virtual string GetSerializationPath()
        {
            throw new System.NotImplementedException();
        }

        public void ResumeAutoSerialization()
        {
            PropertyChanged += Serialize;
        }
        
        public void StopAutoSerialization()
        {
            PropertyChanged -= Serialize;
        }
    }
}