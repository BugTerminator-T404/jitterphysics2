# Jitter World

An instance of the Jitter `World` class contains all entities in the physics simulation and provides the `World.Step` method to advance the simulation by a single time step.

## Creating the World

In Jitter, the user must decide in advance the maximum number of physical entities to be added to the world.

```cs
var capacity = new World.Capacity
{
    BodyCount = 64_000,
    ContactCount = 64_000,
    ConstraintCount = 32_000,
    SmallConstraintCount = 32_000
};

var world = new World(capacity);
```

- **BodyCount**: The maximum number of bodies that can be added to the world.
- **ContactCount**: The maximum number of contacts which can be created. A single contact contains up to four contact points. In Jitter each
shape can generate a contact with each other shape.
- **ConstraintCount**: The maximum number of constraints.
- **SmallConstraintCount**: The maximum number of small constraints. These are constraints of smaller size which are used in soft body simulations for example.

:::info Initial allocation cost
The initial values are used to create fixed arrays of pointers which take a total size of:

$$$
\left(\mathrm{BodyCount}+\mathrm{ContactCount}+\mathrm{ConstraintCount}+\mathrm{SmallConstraintCount}\right)\times{}\mathrm{IntPtr.Size}
$$$

For a typical 64-bit system the example above allocates arrays with a total size of about $1.5\,\mathrm{MB}$.
:::

## World.Step

Forward the world by a single time step using

```cs
Step(float dt, bool multiThread = true)
```

### Time step size

:::info Units in Jitter
The unit system of Jitter is not explicitly defined.
The engine utilizes 32-bit floating-point arithmetic and is optimized for objects with a size of 1 [len_unit].
For example, the collision system uses length thresholds on the order of 1e-04 [len_unit].
It assumes a unit density of 1 [mass_unit/len_unit³] for the mass properties of shapes.
Consequently, the default mass of a unit cube is 1 [mass_unit].
The default value for gravity in Jitter is 9.81 [len_unit/time_unit²], which aligns with the gravitational acceleration on Earth in metric units (m/s²).
Therefore, it is reasonable to use metric units (kg, m, s) when conceptualizing these values.
:::

The smaller the time step size, the more stable the simulation.
Timesteps larger then $\mathrm{dt}=1/60\,\mathrm{s}$ are not adviced.
It is also recommend to use fixed time steps.
Typical code accumulates delta times and calls `world.Step` only at fixed time intervals, as shown in the following example.

```cs
private float accumulatedTime = 0.0f;

public void FixedTimeStep(float dt, int maxSteps = 4)
{
    const float fixedStep = 1.0f / 100.0f;

    int steps = 0;
    accumulatedTime += dt;

    while (accumulatedTime > fixedStep)
    {
        world.Step(fixedStep, multiThread);
        accumulatedTime -= fixedStep;

        // we can not keep up with the real time, i.e. the simulation
        // is running slower than the real time is passing.
        if (++steps >= maxSteps) return;
    }
}
```

### Multithreading

Jitter employs its own thread pool (`Parallelization.ThreadPool`) to distribute tasks across multiple threads, potentially processed by multiple cores.
Jitter utilizes the thread pool when the `world.Step` method is invoked with the multiThread argument set to true.
By default Jitter spawns `ThreadPool.ThreadCountSuggestion`$$$-1$$$ additional threads, where the suggestion is caculated by

```cs
public const float ThreadsPerProcessor = 0.9f;
public static int ThreadCountSuggestion => Math.Max((int)(Environment.ProcessorCount * ThreadsPerProcessor), 1);
```

The number of worker threads managed by the thread pool can be adjusted using `ChangeThreadCount(int numThreads)`.
A singleton pattern is used here, as demonstrated below:

```cs
ThreadPool.Instance.ChangeThreadCount(4);
```

This adjusts the number of additional (with respect to the main thread) worker threads to $$$4-1=3$$$.

The `world.ThreadModel` property may be used to keep the thread pool in a tight loop waiting for work to be processed after `world.Step` has been run (`ThreadModelType.Persistent`), or to yield threads afterwards (`ThreadModelType.Regular`).
The latter option is recommend to free processing power for other code, such as rendering.

## Solver Iterations

Jitter employs an iterative solver to solve contacts and constraints.
The number of iterations can be raised to improve simulation quality (`world.SolverIterations`).

```cs
world.SolverIterations = (solver: 6, relaxation: 4);
```

Jitter solves physical contacts (and constraints) on the velocity level ('solver iterations').
Jitter also adds velocities to rigid bodies to resolve unphysical interpenetrations of bodies.
These additional velocities add unwanted energy to the system which can be removed by an additional relaxation phase after integrating the new positions from these velocities.
The number of iterations in the relaxation phase ('relaxation iterations') is specified here as well.
The runtime for solving contacts and constraints scales linearly with the number of iterations.

## Substep Count

The time step can be divided into smaller steps, defined by `world.SubstepCount`.
These smaller time steps are solved similiar to a regular full steps, however collision information is not updated.
Each sub step is solved with the number of solver iterations specified in `world.SolverIterations`.
For example

```cs
world.SubstepCount = 4;
world.SolverIterations = (solver: 2, relaxation: 1);
```

does perform $$$12$$$ solver iterations in total for each call to `world.Step`.
The runtime is slower than a single regular step with $$$12$$$ iterations but this approach enhances the stability of the simulation.
Substepping is excellent for enhancing the overall quality of constraints, stabilizing large stacks of objects, and simulating large mass ratios (like heavy objects resting on light objects) with greater accuracy.

## Auxiliary Contacts

Jitter employs a technique termed 'auxiliary contacts', where additional contacts are generated for the general case where two flat surfaces of shapes are in contact.
These additional contacts are calculated within one frame, generating the full contact manifold in a 'single pass' and preventing jitter commonly encountered with incrementally constructed collision manifolds.
The `world.EnableAuxiliaryContactPoints` property can be used to enable or disable the usage of auxiliary contact point generation.

## Rigid Bodies

All rigid bodies registered with the world can be accessed using

```cs
world.RigidBodies
```

where `RigidBodies` is of type `ReadOnlyActiveList<RigidBody>`.
The bodies are in no particular order and maybe reordered during calls to `world.Step`.

## Raw Data

`RigidBodie`s, `Arbiter`s, and `(Small)Constraints` are regular C# classes that reside on the managed heap.
However, these objects are linked to their unmanaged counterparts: `RigidBodyData`, `ContactData`, and `(Small)ConstraintData` which can be accessed using:

```cs
world.RawData
```

Jitter relocates native structures so that active objects are stored in contiguous memory, enabling efficient access by the iterative solver.

:::danger Raw Memory Access
Accessing raw memory is generally not required when utilizing the standard functionalities of Jitter.
Although reading the raw data of objects is generally safe, modifying data can corrupt the internal state of the engine.
:::

:::warning Accessing Removed Entities
Instances of `RigidBody`, `Arbiter`, and `Constraint` store some of their data in unmanaged memory, which is automatically freed once the entities are removed (`world.Remove`) from the world.
These entities must not be used any longer, i.e., their functions and properties must not be called or accessed, otherwise, a `NullReferenceException` is thrown.
:::
