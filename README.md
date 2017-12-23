# MSControlSuite

Developed in the Noble-Gas Lab at Lamont-Doherty-Earth-Observatory/Columbia University
New York City, this software package drives a custom-build sample-preparation system
for noble gas measurements. It also retrieves data from an HelixMC mass-spectrometer
(through QTegra).

It's designed in LabView and utilized object oriented programming.

The architecture of the software package is designed to not only drive this one system but can be extended to drive any system.

## MSControl

MSControl's task is to control the hardware, retrieve data and save data.

Sample preparation and analysis can be fully automated, since MSControl uses so called run-scripts to run sequences of pre-defined actions (e.g. opening and closing valves, set temperatures, retrieve data from a mass-spectrometer etc etc.)


## MSEval

MSEval is a data-viewer and data-reduction tool for data collected with MSControl.

Checkout the wiki for full information.