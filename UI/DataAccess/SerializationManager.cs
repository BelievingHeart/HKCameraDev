using System;
using System.Windows;
using HKCameraDev.Core.IoC.Interface;

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