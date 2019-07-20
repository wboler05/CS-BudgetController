# Cities: Skylines - Budget Controller
This is a mod for Cities:Skylines that controls the water, sewage, electricity and education budgets automatically based on consumption rates. This mod only updates the budget by looking at the consumption vs production.  It is currently set to keep the budget at just above 5% of center, meaning you should always have "green" capacity.  There is an offset panel in the options to modify this, but it currently does not work well.

The mod handles the following budgets:
* Water (Water and Sewage)
* Electricity
* Education (Elementary, High School, and University)

With regards to combined budgets, the algorithm chooses the minimum producer as the benchmark. The best way to treat this is to ensure that all your producers for that particular budget are close to equal in production to consumption ratio.  For example, if you have a lot of water towers, but not enough sewage pumps, the budget will optimize strictly against sewage.  You will need to shut off some of the water towers until the water and sewage is equalized.  Another example is if you provide a university, you may need to plop a few more elementary schools to reduce the budget spent towards the university.

## Known Problems
* The budget panel does not update in real-time, but you can see the effects of the budget modification if you bring up either of the utility panels.  If you adjust the budget manually, the algorithm will immediately set that value and then try to optimized back to +5%. Keep that in mind when you open the budget panel to just leave those sliders alone.

## Disclaimer
This is my first code in C# and my first mod for Cities:Skylines (or any game).  It is currently in beta and users should apply it at their own risk.  It's very possible for the controller to become unstable at this time, until I can find time to do some actual system analysis and proper tuning adjustments. 
