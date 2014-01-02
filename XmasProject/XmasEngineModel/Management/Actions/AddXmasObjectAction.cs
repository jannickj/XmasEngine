using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmasEngineModel.EntityLib;

namespace XmasEngineModel.Management.Actions
{
	public class AddXmasObjectAction : EnvironmentAction
	{
		public XmasObject Object { get; private set; }

		public AddXmasObjectAction(XmasObject xobj)
		{
			this.Object = xobj;
		}

		protected override void Execute()
		{
            this.Engine.AddActor(Object);
		}
	}
}
