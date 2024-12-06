using Sisus.Init;
using SMTD.Grid;
using SMTD.BusPassengerGame;

namespace SMTD.LevelSystem
{
	/// <summary>
	/// Initializer for the <see cref="LevelManager"/> component.
	/// </summary>
	internal sealed class LevelManagerInitializer : Initializer<LevelManager, GridSystem, PassengerManager,ColoredGridObjectsController>
	{
		#if UNITY_EDITOR
		/// <summary>
		/// This section can be used to customize how the Init arguments will be drawn in the Inspector.
		/// <para>
		/// The Init argument names shown in the Inspector will match the names of members defined inside this section.
		/// </para>
		/// <para>
		/// Any PropertyAttributes attached to these members will also affect the Init arguments in the Inspector.
		/// </para>
		/// </summary>
		private sealed class Init
		{
			public GridSystem GridSystem = default;
			public PassengerManager PassengerManager = default;
			public ColoredGridObjectsController GridObjectsController = default;
		}
		#endif
	}
}
