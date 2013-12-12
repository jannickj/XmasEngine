using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmasEngineModel.Management
{
    public class HandShake
    {
        public XmasAction Action { get; internal set; }

        internal HandShake( XmasAction action)
        {
            this.Action = action;
        }
        

        public void PerformHandShake()
        {
            this.Action.ActionManager.AcceptHandShake(this);
        }
    }
}
