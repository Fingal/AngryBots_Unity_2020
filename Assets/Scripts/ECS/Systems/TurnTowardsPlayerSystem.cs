using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(MoveForwardSystem))]
public class TurnTowardsPlayerSystem : SystemBase
{
	[BurstCompile]
	[RequireComponentTag(typeof(EnemyTag))]
	struct TurnJob : IJobForEach<Translation, Rotation>
	{
		public float3 playerPosition; 

		public void Execute([ReadOnly] ref Translation pos, ref Rotation rot)
		{
			float3 heading = playerPosition - pos.Value;
			heading.y = 0f;
			rot.Value = quaternion.LookRotation(heading, math.up());
		}
	}

	protected override void OnUpdate()
	{
		if (!Settings.IsPlayerDead()) {


			float3 playerPosition = Settings.PlayerPosition;
			Entities.WithBurst().WithAll<EnemyTag>().ForEach((ref Translation pos, ref Rotation rot) => {
				float3 heading = playerPosition - pos.Value;
				heading.y = 0f;
				rot.Value = quaternion.LookRotation(heading, math.up());

			}).ScheduleParallel();
		}
	}
}

