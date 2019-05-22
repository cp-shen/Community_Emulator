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

After the time of one cycle, the production action will happen, which is assumed not to take any time.

After producing, each person will have a new amount of resources, which is decieded by his original resources and his work ability.

```resources_new = resources_input * ability_work```

## 4. Joint production

During the interval of two production actions,  
a person can sign joint productipn plans with the persons he meets,  
and only one best plan will be choosed.

When joint production happens, the person with weaker work ability can lend part of his resources to the person with stronger work ability.

The ratio of lend amount is decided by maximum of two people's communicating ability.

After joint production, The borrower first returns the amount of resources which equals to the output which the lender will get if using the lent part in producton.

Then the borrower returns half of what he gets additionally by doing this joint producion (half of the benefit).

```c#
resources_lent = resources_lender * max(ability_communicate_lender, ability_communicate_borrower);

benefit = resources_lent * (ability_work_borrower - ability_work_lender);
```

## 5. End of the loop

When the given total time runs out, the system loop will end.

The more resources a person has, the better evaluation a person will be given.

## 6. Survive or die

When the loop ends, a peron whose resources is more than average level will survive, otherwise he will die.
