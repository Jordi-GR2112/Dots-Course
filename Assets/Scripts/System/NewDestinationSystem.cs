using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class NewDestinationSystem : SystemBase
{
    private RandomSystem randomSystem;

    protected override void OnCreate()
    {
        randomSystem = World.GetExistingSystem<RandomSystem>();
    }

    protected override void OnUpdate()
    {

        var randomArray = randomSystem.RandomArray;

        Entities.WithNativeDisableParallelForRestriction(randomArray).ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) => {
            
            float distance = math.abs(math.length(destination.Value - translation.Value));

            if (distance < 0.1f) 
            {               
                var randomSysAl = randomArray[nativeThreadIndex];

                float3 newDestination = new float3 { x = randomSysAl.NextFloat(0, 500), y = 0, z = randomSysAl.NextFloat(0, 500) };

                destination.Value = newDestination;

                randomArray[nativeThreadIndex] = randomSysAl;
            }

            
        }).ScheduleParallel();
    }
}
