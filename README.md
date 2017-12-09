# SimpleElevator  
Emulate simple elevator    

User can set number of floors in a building, set elevator's speed, height of one floor and time interval between opening and closing doors.  
It is supposed that people can not prevent closing of doors. Also acceleration is not taken into account.  
There is only one button on each floor - to call the elevator. There is no ability to tell where are you going to move - up or down.  

User may enter command in format: [source][floor].  
[source] may take values 'e' (command was sent from inside of elevator) or 'f' (means that a button was pressed on a floor).  
[floor] is a number of required (or calling) floor.    

Example:  
e5 - means that inside of an elevator somebody has pressed button "5"  
f3 - means that on the third floor was pressed a button to call elevator
