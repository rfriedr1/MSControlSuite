# MSControlSuite

Developed in the Noble-Gas Lab at Lamont-Doherty-Earth-Observatory/Columbia University New York City,
this software package drives a custom-build sample-preparation system for noble gas measurements.
It also retrieves data from an HelixMC mass-spectrometer (through QTegra). Sample preparation and analysis
can be fully automated, since MSControl uses so called run-scripts to run sequences of pre-defined actions
(e.g. opening and closing valves, set temperatures, retrieve data from a mass-spectrometer etc etc.)

It's designed in LabView and utilized object oriented programming.

## MSControl
The following hardware components are supported:

* Agilent 34980A Data Acquisition/Switch System
* Pfeiffer Vacuum MaxiGauge
* Lakeshore Temperature Controller
* Controlling the mass-spectrometer through Thermo scientific's QTegra software package

MSControl uses so called run-scripts to run sequences of pre-defined actions that the user
want's to perform, e.g. opening and closing valves, set temperatures, retrieve data from a 
mass-spectrometer etc etc.

The architecture of the software package is designed to not only drive this one system but can
be extended to drive any system.

## MSEval
MSEval is a data-viewer and data-reduction tool for data collected with MSControl.
