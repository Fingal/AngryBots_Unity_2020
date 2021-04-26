using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms {
	[UpdateAfter(typeof(BoidMovementSystem))]
	public class DirectionToRotationSystem : SystemBase {
		//[BurstCompile]
		//[RequireComponentTag(typeof(MoveForward))]
		//struct MoveForwardRotation : IJobForEach<Translation, Rotation, MoveSpeed>
		//{
		//	public float dt;

		//	public void Execute(ref Translation pos, [ReadOnly] ref Rotation rot, [ReadOnly] ref MoveSpeed speed)
		//	{
		//		pos.Value = pos.Value + (dt * speed.Value * math.forward(rot.Value));
		//	}
		//}

		protected override void OnUpdate()
		{
			var dt = Time.DeltaTime;
			Entities.WithAll<BoidTag>().WithBurst().ForEach((ref Rotation rot, in Direction direction) => {
				rot.Value = quaternion.LookRotationSafe(direction.Value, math.up());
			}).ScheduleParallel();

			//return moveForwardRotationJob.Schedule(this, inputDeps);
		}
	}
}