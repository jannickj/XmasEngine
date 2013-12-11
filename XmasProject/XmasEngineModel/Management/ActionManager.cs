using System;
using System.Collections.Generic;
using System.Linq;
using JSLibrary.Data.GenericEvents;
using XmasEngineModel.Exceptions;
using XmasEngineModel.Management.Events;
using JSLibrary;

namespace XmasEngineModel.Management
{
    /// <summary>
    /// ActionManager controls the flow of actions being executed in the engine
    /// </summary>
	public class ActionManager
	{
        private HashSet<XmasAction> awaitingHandShake = new HashSet<XmasAction>();
		private Queue<XmasAction> awaitingActions = new Queue<XmasAction>();
		private HashSet<XmasAction> runningActions = new HashSet<XmasAction>();
		private EventManager evtman;

        /// <summary>
        /// Instantiates a new ActionManager
        /// </summary>
        /// <param name="evtman">The eventmanager used in the same engine instantiation</param>
		public ActionManager(EventManager evtman)
		{
			this.evtman = evtman;
		}

        /// <summary>
        /// Gets all actions currently running the action manager
        /// </summary>
		public ICollection<XmasAction> RunningActions
		{
			get { return runningActions.ToArray(); }
		}

        /// <summary>
        /// Gets all actions queued to the action manager
        /// </summary>
		public ICollection<XmasAction> QueuedActions
		{
			get { return awaitingActions.ToArray(); }
		}

        private void raiseEventForAction(XmasAction act, XmasEvent evt)
        {
            var envact = act as EnvironmentAction;
            var entact = act as EntityXmasAction;
            if (entact != null)
            {
                entact.Source.Raise(evt);
            }
            else
            {
                evtman.Raise(evt);
                if (envact != null)
                {
                    foreach (var xuni in envact.RaiseActionEvtOn)
                    {
                        xuni.RaiseNonRecursive(evt);
                    }
                }
            }
        }

		#region EVENTS

		private void action_Resolved(object sender, EventArgs e)
		{
			XmasAction ga = (XmasAction) sender;
			runningActions.Remove(ga);
			ga.Resolved -= action_Resolved;
            if (ga.ActionFailed)
            {
                var failedevent = (XmasEvent)Generics.InstantiateGenericClass(typeof(ActionFailedEvent<>), new Type[] { ga.GetType() }, ga.Clone(),ga.ActionFailedException);
                raiseEventForAction(ga, failedevent);
            }
            
		}

        private void action_Completed(object sender, EventArgs e)
        {
            XmasAction ga = (XmasAction)sender;
            ga.Completed -= action_Completed;

            var completedevent = (XmasEvent)Generics.InstantiateGenericClass(typeof(ActionCompletedEvent<>), new Type[] { ga.GetType() }, ga.Clone());

            raiseEventForAction(ga, completedevent);

        }

		#endregion

		internal event UnaryValueHandler<XmasAction> ActionQueuing;
		internal event UnaryValueHandler<XmasAction> ActionQueued;
        internal event UnaryValueHandler<XmasAction> PreActionExecution;

        /// <summary>
        /// Executes all actions queued to the action manager (This action is not threadsafe)
        /// </summary>
        /// <returns>the number of actions succesfully executed</returns>
		public int ExecuteActions()
		{
			int actionsExecuted = 0;
			List<XmasAction> actions;

			do
			{
				lock (this)
				{
					actions = awaitingActions.ToList();
					awaitingActions.Clear();
				}

				foreach (XmasAction action in actions)
				{
                    ExecuteAction(action);
                    actionsExecuted++;					
				}
				lock (this)
				{
					if (awaitingActions.Count == 0)
						break;
				}
			} while (true);
			return actionsExecuted;
		}

        internal void ExecuteAction(XmasAction action)
        {
            if (!action.HandShakeCompleted)
            {
                if (PreActionExecution != null)
                    PreActionExecution(this, new UnaryValueEvent<XmasAction>(action));
                runningActions.Add(action);
                action.Resolved += action_Resolved;
                action.Completed += action_Completed;

                var handshakeInquryevt = (XmasEvent)Generics.InstantiateGenericClass(typeof(ActionHandShakeInqueryEvent<>), new Type[] { action.GetType() }, action);
                raiseEventForAction(action, handshakeInquryevt);

                var startingevent = (ActionStartingEvent)Generics.InstantiateGenericClass(typeof(ActionStartingEvent<>), new Type[] { action.GetType() }, action.Clone());
                if (action.HandShakeRequired)
                {
                    startingevent.HandShakeNeeded = true;
                    startingevent.HandShake = new HandShake(this, action);
                }
                raiseEventForAction(action, startingevent);
            }

            if (!action.HandShakeRequired || action.HandShakeCompleted)
            {
                try
                {
                    action.Fire();
                
                }
                catch (ForceStopEngineException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    action.Fail(e);
                    this.evtman.Raise(new ActionFailedEvent(action,e));

                }
            }
        }

        


        /// <summary>
        /// Queues an action to the ActionManager. This method is threadsafe.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
		public void Queue(EnvironmentAction action)
		{
			QueueAction(action);
		}

		internal void QueueAction(XmasAction action)
		{
			lock (this)
			{
				if (ActionQueuing != null)
					ActionQueuing(this, new UnaryValueEvent<XmasAction>(action));
				awaitingActions.Enqueue(action);
				if (ActionQueued != null)
					ActionQueued(this, new UnaryValueEvent<XmasAction>(action));
			}
		}

        internal void AcceptHandShake(HandShake handShake)
        {
            lock (this)
            {
                if(this.awaitingHandShake.Contains(handShake.Action))
                {
                    handShake.Action.HandShakeCompleted = true;
                    this.awaitingHandShake.Remove(handShake.Action);
                    this.awaitingActions.Enqueue(handShake.Action);
                }
            }
        }
    }
}