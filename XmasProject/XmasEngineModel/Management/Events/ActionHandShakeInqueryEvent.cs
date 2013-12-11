using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmasEngineModel.Management.Events
{
    /// <summary>
    /// Event that is fired before the action started event is fired to give an oppotunity to make the action require handshake
    /// </summary>
    /// <typeparam name="TAction"></typeparam>
    public class ActionHandShakeInqueryEvent<TAction> : XmasEvent where TAction : XmasAction
    {
        

        /// <summary>
        /// Gets the action that was that is in question
        /// </summary>
        public TAction Action { get; private set; }

        internal ActionHandShakeInqueryEvent(TAction action)
        {
            this.NotThreadSafe = true;
            this.Action = action;
        }

    }
}
