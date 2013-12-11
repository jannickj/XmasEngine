using System.Collections.Generic;
using XmasEngineModel.EntityLib;
namespace XmasEngineModel.Management
{

    /// <summary>
    /// Action type meant to make changes that affect the entire environment, this actiontype is queued on the ActionManager
    /// </summary>
	public abstract class EnvironmentAction : XmasAction
	{
        private List<XmasUniversal> raiseActionEvtOn = new List<XmasUniversal>();

        internal IEnumerable<XmasUniversal> RaiseActionEvtOn
        {
            get { return raiseActionEvtOn; }
        }

        public void RaiseActionEventOn(XmasUniversal xuni)
        {
            this.raiseActionEvtOn.Add(xuni);
        }
	}
}