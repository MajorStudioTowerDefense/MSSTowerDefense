Thanks for using Tutorial Factory.

Please check the updated documentation in the following link:
https://hsv-studio.gitbook.io/tutorial-factory/

If you have any question, feel free to contact me at:
hsvstudio1011@gmail.com

Forum link:
https://forum.unity.com/threads/new-release-discount-50-off-tutorial-factory.1271633/#post-8076980

Hope you enjoy it!

<<<IMPORTANT NOTICE>>>
Please remember to always backup projects before upgrading to new versions as certain upgrades may overwrite old settings.
**************************************************************************************************************************

There is currently a editor bug with [SerializedReference] attribute in Unity 2020.3 that would slow down editor if there are too many Tutorial Objects.
This issue is somehow solved in Unity 2021 with new SerializeReference. However, if you upgrade to Unity 2021, do not reopen project on Unity 2020 LTS again.
Otherwise all your added modules in Tutorial Factory would be gone. There is currently no issues to run in both versions other than editor slow down in Unity 2020 LTS.

Release Update:
Version 1.40:
New:
-Added Bound Module.
-Added global events for different state of Stage and Tutorial objects.
-Added option to initialize Tutorial Manager manually.
-Added font assets override for InfoDisplay and Popup module.
-Added runtime text change event for InfoDisplay and Popup module.
Bug Fix and Updates:
-Removed obsolete event system.
-Fixed StepBack API function call.
-Fixed Custom Module property drawing bug.

Version 1.30:
New:
-Added support for Unity new input system.
-Added custom raycast to handle scene objects clicking events.
-Added back button to PopupUIMask. Now it is able to go back to to previous Tutorial step with back button.
-Added API function 'StepbackTutorial()' in Tutorial Manager instance. Calling this would allow user to go back to previous Tutorial step.
-Added customizable option for close button, advance button and back button in PopupUIMask.
Updates:
-Removed Physics Raycaster component from camera. This component would not be added during runtime. It is replaced by HSVInputManger to handle scene objects detection.


Version 1.20:
New:
-Added trigger for individual Targets. Allows user to deactivate specified Target without stopping Tutorial Object.
-Added Pointer Type to Collider Config.
-Added new 'Transform' bound type to 'Target Bound Type'. Allows user to use Target transform bound not including its children transforms.
-Added new Event system to Stage and Tutorial Objects (Old event would be removed in version 1.3).
Bug Fix and Updates:
-Updated Expose Reference script to store scene object reference.
-Updated Editor layout for better visual.
-Updated Tutorial Manager state system.
-Updated Demo Scene to include the usage of Trigger Configuration in Targets.
-Fixed several minor editor bugs.
-Fixed Editor play/stop buttons bugs.

1.14
New:
-Added Distance Display option to the Position Module.
Bug Fix:
-Fixed minor Editor bug when duplicating/pasting object configuration.

1.13:
Bug Fix:
-Fixed Focus Dim Module canvas sorting order not correctly intialized when module starts.
-Fixed Popup Module scale in/out animation not correctly display.

1.12:
Bug Fix and Update:
-Fixed Arrow Module not correctly positioning around Target with 'Rect Offset' value.
-Fixed Arrow Module not fading in when 'Track OffScreen' is enable/disable repeatedly.

1.11:
Bug Fix and Update:
-Fixed PhysicsRaycaster component not getting removed correctly when Tutorial manager is removed
-Updated documentation to correctly indicate PhysicsRaycaster component being added to MainCamera.

1.1:
New:
-Added 'Position Module' to the built-in list. New module could spawn a display icon on top of target.
-Added effect prefab spawning for individual tutorial target. Every tutorial target could have extra prefab spawn on specified location.
-Added 'useRigidBodyTag' option for tag filtering in Trigger Config. If not use rigidbody for tag, it would use tag of collider/trigger's gameobject.
-Added new Position Module demo to the demo scene.
Bug Fix and Updates:
-Fixed Stage state and Tutorial state not being correctly initialized.
-Fixed Tutorial state name not updating on runtime if state changed.
-Fixed inspector error for module config.
-Updated inspector look for better displaying.