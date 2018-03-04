KinectTracker v1.00
======================================

KinectTracker is a software that allows to track all joints of the person in realtime 
by retrieving coordinate triples(x,y,z) of joints. It is programmed entirely in C#/.NET.
THE GUI is organized into 3-tabs. The first tab visualizes the RGB, IR and Depth camera streams.
The second tab visualizes the detected body joints (please ignore the small offset). The third tab is intended to track the body joint or custom objects using reflexive-tape. The intetions for this functionality was to track joints that are ocluded by any physical panel which are not detected by Kinect as a whole skeleton. The software segments the tape region and outputs the centroid coordinates of that region.
The tracked coordinates are sent to event server and can be accessed by any TCP client.

1. System Requirements:
32/64 bit processor
4GB RAM or higher
Physical dual core 3.1 Ghz or faster processor
USB 3.0 Controller 
DX11 graphics adapter

2. Installation Instructions:
Install  Visual Studio Express Edition 2012 or higher.
Install Kinect SDK 2.0
Define eventServer path at Websocket/WebsocketClient
Open Kinect Tracker solution in Visual Studio and build the project.
Start the eventServer
You can run the program in VS environment or create a release version with .exe installer.
The 3 tabs are just a visualization. The blobs and skeletons are sent to the eventServer continously. The only thing you can configure is the threshold for the blob, i.e. the area blob needs to have to count as such. Then the center of the blob is sent to the eventServer.
If you want to detect IR blobs, you need to switch to the corresponding tab in the GUI and define an appropriate IR threshold.

3. Generated Events
{"event":"onIRBlobDetected","id":144,"ts":1510321674361,"value":{"area":35393,"centroid":{"X":0.06998157,"Y":-0.121933341,"Z":0.572}},"tags":[]}
{"event":"onBodyDetected","id":306,"ts":1510321674365,"value":{"bodies":[{"id":"72057594037929899","joints":[{"name":"spinebase","x":-0.9939101,"y":0.427907765,"z":2.1797142},{"name":"spinemid","x":-1.03157043,"y":0.576595068,"z":2.11458564},{"name":"neck","x":-1.06548727,"y":0.721084356,"z":2.04363132},{"name":"head","x":-1.11879838,"y":0.7733406,"z":2.05824423},{"name":"shoulderleft","x":-1.15141428,"y":0.729372144,"z":2.0727632},{"name":"elbowleft","x":-1.24231076,"y":0.711663544,"z":2.11834741},{"name":"wristleft","x":-1.10983229,"y":0.674157858,"z":2.04833269},{"name":"handleft","x":-1.05848,"y":0.6675247,"z":2.01820874},{"name":"shoulderright","x":-1.03092146,"y":0.7519197,"z":2.10141778},{"name":"elbowright","x":-1.01737833,"y":0.599927366,"z":2.11786866},{"name":"wristright","x":-1.02578461,"y":0.4478839,"z":2.1078012},{"name":"handright","x":-1.01692808,"y":0.39819023,"z":2.11626172},{"name":"hipleft","x":-1.01199567,"y":0.408825219,"z":2.14521456},{"name":"kneeleft","x":-1.08706117,"y":0.230286434,"z":2.0930028},{"name":"ankleleft","x":-1.11115491,"y":0.04686943,"z":2.05755353},{"name":"footleft","x":-1.05037284,"y":0.0974350944,"z":2.057173},{"name":"hipright","x":-0.9582565,"y":0.4394957,"z":2.17626286},{"name":"kneeright","x":-1.03796482,"y":0.23413223,"z":2.11391425},{"name":"ankleright","x":-0.9975539,"y":0.0501568876,"z":2.10548472},{"name":"footright","x":-1.01767,"y":0.09159902,"z":2.06632876},{"name":"spineshoulder","x":-1.05767667,"y":0.685626447,"z":2.062513},{"name":"handtipleft","x":-1.049484,"y":0.654729664,"z":2.007773},{"name":"thumbleft","x":-1.044672,"y":0.6784231,"z":2.01899981},{"name":"handtipright","x":-1.01172674,"y":0.37465,"z":2.11208463},{"name":"thumbright","x":-0.9927631,"y":0.39999333,"z":2.11200023}]}]},"tags":[]}


4. What is contained in folders
-----------------------------------
The main directory is /Sensor which contains FrameProcessor to capture and process frames. It also contains KinectManager - a singelton class for kinect operations.
In directory /CVision you will find image-processing algorithms for drawing skeleton joints and detecting blobs.
TCP and object serialization operations ar located in /Websocket direcory.

5. Contributors
Furkat Kochkarov(furkat.kochkarov88@gmail.com)