using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class LifetimeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

    protected override void OnCreate()
    {
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var ecb = endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifeTime) => {

            lifeTime.Value -= deltaTime;

            if (lifeTime.Value < 0.1f)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }

        }).ScheduleParallel();

        endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}
