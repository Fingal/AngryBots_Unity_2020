using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms
{
	public class MoveForwardSystem : SystemBase {
		[BurstCompile]
		[RequireComponentTag(typeof(MoveForward))]
		struct MoveForwardRotation : IJobForEach<Translation, Rotation, MoveSpeed>
		{
			public float dt;

			public void Execute(ref Translation pos, [ReadOnly] ref Rotation rot, [ReadOnly] ref MoveSpeed speed)
			{
				pos.Value = pos.Value + (dt * speed.Value * math.forward(rot.Value));
			}
		}

		protected override void OnUpdate()
		{
			var dt = Time.DeltaTime;
			Entities.WithAll<MoveForward>().WithBurst().ForEach((ref Translation pos, ref Rotation rot, ref MoveSpeed speed) => {
				pos.Value = pos.Value + (dt * speed.Value * math.forward(rot.Value));
			}).ScheduleParallel();

			//return moveForwardRotationJob.Schedule(this, inputDeps);
		}
	}
}