﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms {
	[UpdateAfter(typeof(BoidMovementSystem))]
	public class MoveBoidForwardSystem : SystemBase {
		protected override void OnUpdate() {
			float speed = Settings.getSpeed();
			float range = Settings.getRange();
			var dt = Time.DeltaTime;
			Entities.WithAll<BoidTag>().WithBurst().ForEach((ref Translation pos, in Direction direction) => {
				pos.Value = pos.Value + (dt * speed * direction.Value);
				if (pos.Value.x > range || pos.Value.x < -range) { pos.Value.x = ((pos.Value.x + 3 * range) % (2 * range)) - range; }
				if (pos.Value.y > range || pos.Value.y < -range) { pos.Value.y = ((pos.Value.y + 3 * range) % (2 * range)) - range; }
				if (pos.Value.z > range || pos.Value.z < -range) { pos.Value.z = ((pos.Value.z + 3 *range) % (2*range))-range; }
			}).ScheduleParallel();

			//return moveForwardRotationJob.Schedule(this, inputDeps);
		}
	}
}