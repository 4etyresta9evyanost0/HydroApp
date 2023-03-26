using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroApp
{
    internal class CommonPageViewModel : ViewModel
    {
        public HydropressDbContext Context { get ; set; }
        public CommonPageViewModel(HydropressDbContext context)
        {
            Context = context;
        }
    }
}
