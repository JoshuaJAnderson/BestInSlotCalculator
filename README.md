# What is the purpose of this repo?
This repo is made as an example of a different program I made. It loops through every combination of modifiers and figures out which combination 
would give the best dps (damage per second), dph (damage per hit), hps (heals per second). Things to note is modifiers are added together before being 
multiplied into class modifiers to figure out total dps.

## How to use?
- First you need to make sure you can connect to an SQL Database with the specified formatting. Fill out the stats of the modifiers. Once you run the 
program it will tell you the format it desires which is: [Calculation_Type](string) [Class_Type](string) [Tier](int) [Slots](int) [Recoil](float)
- [Calculation_Type] Includes: "Dps"(Damage Per Second), "Hps"(Heals Per Second), "Dph"(Damage Per Hit)
- [Class_Type] Includes: "Tank", "Healer", "Assassin", "Rouge", "Artillary", "Commander", "AoE"
- [Tier] Has to be an integer greater than or equal to 0
- [Slots] Has to be an integer above 0.
- [Recoil] This is the base speed of reloading before the next shot. Has to be a float greater than 0. If you have rate of fire, just take the inverse of
that to get the recoil.


## The math behind each:
- Dps is: Damage * RateOfFire * (1 + BaseCrit * CritStrength * CritPercentage) * MultiShot
- Dph is: Damage * (1 + BaseCit * CritStrength) * MultiShot
- Hps is: Damage * RateOfFire * (1 + BaseCrit * CritStrength * CritPercentage) * HealingPower

## You will need a SQL Database to connect to
The connection string is in the "SQL_Reader.cs" file.
The formatting is:
- mod_name VARCHAR(255)
- damage FLOAT
- rate_of_fire FLOAT
- critical_strength FLOAT
- critical_chance FLOAT
- multi_shot FLOAT
- tech INT