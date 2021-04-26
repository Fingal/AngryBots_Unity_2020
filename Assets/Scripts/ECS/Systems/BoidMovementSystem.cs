using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms
{
	public class BoidMovementSystem : SystemBase {
		EntityQuery boidPositionQuery;
		EntityQuery boidDirectionQuery;
		protected override void OnCreate() {
			boidPositionQuery = GetEntityQuery(typeof(BoidTag), ComponentType.ReadOnly<Translation>());
			boidDirectionQuery = GetEntityQuery(typeof(BoidTag), ComponentType.ReadOnly<Direction>());
		}

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

		protected override void OnUpdate() {
			var _boidsPosition = boidPositionQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			var boidsPosition = _boidsPosition.AsReadOnly();
			var _boidsDirection = boidDirectionQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
			var boidsDirection = _boidsDirection.AsReadOnly();
			var dt = Time.DeltaTime;
			var separationDistance = 4f;
			var coherenceDistance = 25f;
			var aignmentDistance = 12f;

			var jobHandle = Entities.WithAll<BoidTag>().WithBurst().ForEach((ref Direction direction, in Translation pos) => {
				float3 coherence = 0, separation = 0, aignment = 0;
				int coherenceCounter = 0, separationCounter = 0, aligmentCounter = 0;
				for (int i = 0; i < boidsPosition.Length; i++) {
					var other = boidsPosition[i];
					var toOtherDirection = math.normalize(other.Value - pos.Value);
					var distance = math.length(pos.Value - other.Value);
					if (distance <= math.EPSILON) {
						continue;
					}
					if (distance < separationDistance) {
						separation -= toOtherDirection;
						separationCounter += 1;
					}
					else if (distance < coherenceDistance) {
						coherence += toOtherDirection;
						coherenceCounter += 1;
					}
					if (distance < aignmentDistance && math.dot(toOtherDirection,direction.Value)>0.7) {
						aignment += boidsDirection[i].Value;
						aligmentCounter += 1;
					}
				}
				coherence = (coherence) / math.max(1, coherenceCounter);
				separation = (separation) / math.max(1, separationCounter);
				aignment = (aignment) / math.max(1, aligmentCounter);
				direction.Value = math.normalize(11 * direction.Value + 1.2f * (separation) + 0.5f * (coherence) + 0.9f * (aignment));
				//pos.Value = pos.Value + (dt * 1 * direction.Value);
			}).ScheduleParallel(this.Dependency);

			//return moveForwardRotationJob.Schedule(this, inputDeps);

			var boidsHandle = _boidsPosition.Dispose(jobHandle);
			this.Dependency = JobHandle.CombineDependencies(boidsHandle,_boidsDirection.Dispose(jobHandle));


		}
		//protected override void OnUpdate() {
		//	var _boids = boidQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
		//	var boids = _boids.AsReadOnly();
		//	var dt = Time.DeltaTime;
		//	var separationDistance = 10f;
		//	var coherenceDistance = 15f;

		//	var jobHandle = Entities.WithAll<BoidTag>().WithoutBurst().ForEach((ref Direction direction, in Translation pos) => {
		//		float3 coherence = 0;
		//		float3 separation = 0;
		//		int coherenceCounter = 0, separationCounter = 0;
		//		for (int i = 0; i < boids.Length; i++) {
		//			var other = boids[i];
		//			var toOtherDirection = math.normalize(other.Value - pos.Value);
		//			var distance = math.length(pos.Value - other.Value);
		//			if (distance <= math.EPSILON) {
		//				continue;
		//			}
		//			if (distance < separationDistance) {
		//				separation -= toOtherDirection;
		//				coherenceCounter += 1;
		//			}
		//			if (distance < coherenceDistance) {
		//				coherence += toOtherDirection;
		//				separationCounter += 1;
		//			}
		//		}
		//		coherence = (coherence) / math.max(1, coherenceCounter);
		//		separation = (separation) / math.max(1, separationCounter);
		//		direction.Value = math.normalize(10 * direction.Value + 1.4f * (separation) + 0.5f * (coherence));
		//		//pos.Value = pos.Value + (dt * 1 * direction.Value);
		//	}).ScheduleParallel(this.Dependency);

		//	//return moveForwardRotationJob.Schedule(this, inputDeps);

		//	this.Dependency = _boids.Dispose(jobHandle);


		//}
	}
}