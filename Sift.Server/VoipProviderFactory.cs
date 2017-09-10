using System;
using System.Collections.Generic;

using IniParser.Model;

using Sift.Common;

namespace Sift.Server
{
    internal static class VoipProviderFactory
    {
        internal static readonly IReadOnlyDictionary<string, VoipProviders> TypeNames = new Dictionary<string, VoipProviders>
        {
            { "none", VoipProviders.None },
            { "asterisk", VoipProviders.Asterisk },
        };

        internal static readonly IReadOnlyDictionary<VoipProviders, string> PrettyNames = new Dictionary<VoipProviders, string>
        {
            { VoipProviders.None, "None" },
            { VoipProviders.Asterisk, "Asterisk" },
        };

        public static IVoipProvider Create(KeyDataCollection data)
        {
            string type = data["type"].Trim().ToLower();
            if (type == null)
                throw new Exception("missing type value in Provider collection");
            if (!TypeNames.ContainsKey(type))
                throw new Exception("unsupported type value in Provider collection");
            VoipProviders provider = TypeNames[type];
            switch (provider)
            {
                case VoipProviders.Asterisk:
                    return new Asterisk(data);
                default:
                    throw new NotImplementedException("VoipProviderFactory");
            }
        }
    }
}
