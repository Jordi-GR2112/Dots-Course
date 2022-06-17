using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

public class PersonCollisionSystem : SystemBase
{
    private BuildPhysicsWorld physicsWorld;
    private StepPhysicsWorld stepPhys;

    protected override void OnCreate()
    {
        physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhys = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct PersonCollisionJob : ITriggerEventsJob
    {
        [ReadOnly]public ComponentDataFromEntity<PersonTag> PersonGroup;
        public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
        public float seed;

        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
            bool isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);

            if(!isEntityAPerson || !isEntityBPerson) { return; }

            var random = new Random((uint)(triggerEvent.BodyIndexA * triggerEvent.BodyIndexB * (1 + seed)));

            random = ChangeMaterialColor(random, triggerEvent.EntityA);
            ChangeMaterialColor(random, triggerEvent.EntityB);

        }
        private Random ChangeMaterialColor(Random random, Entity entity)
        {
            if (ColorGroup.HasComponent(entity))
            {
                var colorComponent = ColorGroup[entity];
                colorComponent.Value = new float4(random.NextFloat(0,1), random.NextFloat(0,1), random.NextFloat(0,1), colorComponent.Value.w);

                ColorGroup[entity] = colorComponent; 
            }

            return random;
        }
    }

    

    protected override void OnUpdate()
    {
        Dependency = new PersonCollisionJob
        {
            PersonGroup = GetComponentDataFromEntity<PersonTag>(true),
            ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
            seed = System.DateTimeOffset.Now.Millisecond
        }.Schedule(stepPhys.Simulation, ref physicsWorld.PhysicsWorld, Dependency);
    }
}
