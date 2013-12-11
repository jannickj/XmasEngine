using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmasEngineModel.Management
{
    public class HandShake
    {
        private ActionManager manager;
        internal XmasAction Action { get; set; }

        public HandShake(ActionManager actionManager, XmasAction action)
        {
            // TODO: Complete member initialization
            this.manager = actionManager;
            this.Action = action;
        }
        

        public void PerformHandShake()
        {
            manager.AcceptHandShake(this);
        }
    }
}
