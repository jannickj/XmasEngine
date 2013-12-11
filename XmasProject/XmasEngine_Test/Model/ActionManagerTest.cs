using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using XmasEngineModel.Management;
using XmasEngineModel.Management.Actions;
using XmasEngineModel.Management.Events;

namespace XmasEngine_Test.Model
{
	[TestFixture]
	public class ActionManagerTest
	{
		private EventManager EventManager { get; set; }
		private ActionManager ActionManager { get; set; }

		public ActionManagerTest()
		{
			this.EventManager = new EventManager();
			this.ActionManager = new ActionManager(this.EventManager);
		}

		[Test]
		public void ExecuteAction_ActionFailsWhileRunning_RaiseActionFailedEvent()
		{
			bool evtRaised = false;
			this.EventManager.Register(new Trigger<ActionFailedEvent<SimpleAction>>(evt => evtRaised = true));
			this.ActionManager.ExecuteAction(new SimpleAction(_ => { throw new Exception("FAIL"); }));
			Assert.IsTrue(evtRaised);

		}

		

		[Test]
		public void ExecuteActionWithHandShake_HandShakeGiven_ActionExecuted()
		{
			HandShake handshake = null;
			bool actionExecuted = false;
			var action = new SimpleAction(sa => actionExecuted = true);
			action.HandShakeRequired = true;
			this.EventManager.Register(new Trigger<ActionStartingEvent<SimpleAction>>(evt => handshake = evt.HandShake));
			this.ActionManager.Queue(action);
			this.ActionManager.ExecuteActions();
			Assert.IsFalse(actionExecuted);

			handshake.PerformHandShake();
			this.ActionManager.ExecuteActions();

			Assert.IsTrue(actionExecuted);


		}

		[Test]
		public void ExecuteActionWithoutHandShake_HandShakeRequirementInjected_ActionExecutedOnHandShake()
		{
			HandShake handshake = null;
			bool actionExecuted = false;
			var action = new SimpleAction(sa => actionExecuted = true);
			this.EventManager.Register(new Trigger<ActionHandShakeInqueryEvent<SimpleAction>>(evt => evt.Action.HandShakeRequired = true));
			this.EventManager.Register(new Trigger<ActionStartingEvent<SimpleAction>>(evt => handshake = evt.HandShake));
			this.ActionManager.Queue(action);
			this.ActionManager.ExecuteActions();
			Assert.IsFalse(actionExecuted);

			handshake.PerformHandShake();
			this.ActionManager.ExecuteActions();

			Assert.IsTrue(actionExecuted);
		}

	}
}
