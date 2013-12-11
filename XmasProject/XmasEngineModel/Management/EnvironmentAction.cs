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

        /// <summary>
        /// Adds an XmasUniversal to a list of Universals that has an Action Failed/Started/Completed event raised on them
        /// </summary>
        /// <param name="xuni">XmasUniversal to have an ActionEvent raised on it</param>
        public void AddRaiseActionEvent(XmasUniversal xuni)
        {
            this.raiseActionEvtOn.Add(xuni);
        }
	}
}