After importing this sample to your project, you need to :

	- Hit on the top toolbar button : Tools/Setup Project (Jichaels's VR SDK)
	
	- Change the values of these prefabs :

		- On JVRPlayer/JVRHeadSet/TrackingSpace/Camera		
			- Change the "Environment/Volume mask" to PostProcessing.
			
		- On JVRHandLeft and JVRHandRight :
			- Change the value of JVRController/Layer mask to "JVRInteract"
			
		- On Player/Controllers/Modules/Drawer/JVRDrawableMain
		    - Change layer to "JVRInteract"