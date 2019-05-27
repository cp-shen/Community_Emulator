## 1. Composition of the system

A number of spheres will be put into one container.

The spheres represent people.

The container represents the world.

Each sphere will be spawned at the center of the container and have a constant move speed and a random move direction.

When a sphere hits the boundary of the container or another sphere,
the velocity of the sphere will change direction to the normal direction of the hit point. 

## 2. Properties of a person

Each person will hava an equal initial amount of resources.

Each person has two abilities, which are work ability and communicating ability.

The strength of abilities is decided randomly for each person.

And the stonger one ability is, the weaker another ability will be.

## 3. Production loop

When two spheres collides with each other, the production action will happen, which is assumed not to take any time.

After producing, each person will have a new amount of resources, which is decieded by their original resources and their abilities.

There is an upper limit of times of production for each person.

## 4. Joint production

When production happens, the person with weaker work ability can lend part of his resources to the person with stronger work ability.

The ratio of lend amount is decided by average of two people's communicating ability.

By lending resources, an amount of benefit will be generated.

And each of two persons will get part of the benefit.

The stronger a person's communicating ability is, the larger part of benefit he will get.

```c#
resources_lent = resources_lender * average(ability_communicate_lender, ability_communicate_borrower);

benefit = resources_lent * (ability_work_borrower - ability_work_lender);

resources_new_A = resources_old_A * ability_work_A + benefit * ability_com_A / (ability_com_A + ability_com_B);
```

## 5. End of the loop

When the given total time runs out, the system loop will end.

The more resources a person has, the better evaluation a person will be given.

## 6. Survive or die

When the loop ends, a peron whose resources is more than average level will survive, otherwise he will die.
