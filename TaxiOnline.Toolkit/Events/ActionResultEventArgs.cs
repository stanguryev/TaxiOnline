using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Toolkit.Events
{
    public class ActionResultEventArgs : EventArgs
    {
        private readonly ActionResult _result;

        public ActionResult Result
        {
            get { return _result; }
        } 

        public ActionResultEventArgs(ActionResult result)
        {
            _result = result; ;
        }
    }
}
