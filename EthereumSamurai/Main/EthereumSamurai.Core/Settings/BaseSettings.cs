using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Core.Settings
{
    public interface IBaseSettings
    {
    }

    public class BaseSettings : IBaseSettings
    {
    }

    public class SettingsWrapper
    {
        public IBaseSettings EthereumIndexerSettings { get; set; }
    }
}
