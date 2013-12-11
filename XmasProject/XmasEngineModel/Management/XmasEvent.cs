using System;

namespace XmasEngineModel.Management
{

    /// <summary>
    /// Extend this class to create new types of events for the engine
    /// </summary>
	public abstract class XmasEvent : EventArgs
	{
        /// <summary>
        /// Set true to prevent the event from being shared with the threadsafe evenmanager
        /// </summary>
        protected internal bool NotThreadSafe { get; set; }
	}
}