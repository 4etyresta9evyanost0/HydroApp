using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroApp
{
    public enum ServerListStatus
    {
        Updating = 1,
        Updated = 0,
        Failed = -1
    }

    public enum UserType
    {
        Guest = -1,
        Admin = 0,
        Constructor = 1,
        Common = 2
    }
}
