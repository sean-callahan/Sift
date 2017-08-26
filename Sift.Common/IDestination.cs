using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sift.Common
{
    public interface IDestination
    {
        string Id { get; set; }

        string EndPoint { get; }
    }
}
