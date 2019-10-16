# Bridge-timer
A timer for bridge matches. Highly configurable
Written to get a grip on the brand new .Net Core 3.0 API. The program should speak for itself. 
Beware of the French translations! S'il y a un confrère qui soit disposé(e) á ameliorer les traductions françaises je serai très reconnaissant.  

# Inno setup installation script
An Inno setup installation script is present in the Installer folder.
In the Installer\Installer folder a compiled installer can be found.

# Commandline parameters
The timings of the application (playing time, warning moment, change time) can be set using the Settings button in the application. These settings are remembered. These timimgs can be set as well using commandline parameters:
BridgeTimer.exe -p22 -w5 -c3 will set the playing time to 22 minutes, the warning moment to 5 minutes before the playing time runs out and the change time to 3 minutes. It is not necessary to specify all three parameters, values that are omitted will be taken from the previous values.
