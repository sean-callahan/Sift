using System.Collections.Generic;

using Sift.Common.Net;

namespace Sift.Client.Elements
{
    internal interface ISettingsElement
    {
        void Load(IDictionary<string, NetworkSetting> cache);
        void Save();
    }
}
