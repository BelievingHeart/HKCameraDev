using System;
using System.Windows;
using ImageDebugger.Core.IoC;
using ImageDebugger.Core.IoC.Interface;

namespace UI.DataAccess
{
    public class SerializationManager : ISerializationManager
    {
        public string SerializationBaseDir
        {
            get { return Environment.CurrentDirectory; }
        }
    }
}