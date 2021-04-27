using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms {
	[UpdateAfter(typeof(BoidMovementSystem))]
	public class DirectionToRotationSystem : SystemBase {

		protected override void OnUpdate()
		{
			var dt = Time.DeltaTime;
			Entities.WithAll<BoidTag>().WithBurst().ForEach((ref Rotation rot, in Direction direction) => {
				rot.Value = quaternion.LookRotationSafe(direction.Value, math.up());
			}).ScheduleParallel();

		}
	}
}