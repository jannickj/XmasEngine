using JSLibrary.Data;
using XmasEngineExtensions.TileExtension.Events;
using XmasEngineModel.EntityLib;
using XmasEngineModel.Management;
using XmasEngineModel.Management.Events;
using XmasEngineExtensions.TileExtension.Modules;
using XmasEngineModel.Management.Actions;

namespace XmasEngineExtensions.TileExtension.Actions
{
	public class MoveUnitAction : EntityXmasAction<Agent>
	{
		private Vector direction;
		private double time;


		public MoveUnitAction(Vector direction)
		{
			this.direction = direction.Direction;
		}

		protected override void Execute()
		{
			TilePosition tile = World.GetEntityPosition(Source) as TilePosition;
			Point newloc = tile.Point + direction;
			UnitMovePreEvent before = new UnitMovePreEvent(newloc);

			Source.Raise(before);
			if (before.IsStopped)
			{
		
				return;
			}
			time = Source.Module<SpeedModule>().Speed;
			if (time == 0)
			{
				if (World.SetEntityPosition(Source, new TilePosition(newloc)))
					Source.Raise(new UnitMovePostEvent(newloc));
				else
					Source.Raise(new UnitMovePostEvent(tile.Point));
			
			}
			else
			{
				TimedAction gt = Factory.CreateTimer(() =>
					{
						if (World.SetEntityPosition(Source, new TilePosition(newloc)))
							Source.Raise(new UnitMovePostEvent(newloc));
						else
							Source.Raise(new UnitMovePostEvent(tile.Point));

						
					});

				
					gt.SetSingle(time);
                    this.RunAction(gt);
			}
		}
	}
}