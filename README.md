**Features:**  
• Shows in the food name wheter it's raw (yellow) undercooked (yellow), cooked (green), overcooked (red) or burnt (red).
• Shows a percentage when cooking food. Between 0 and 100% the food is considered raw, between 100 and 150% the food is properly cooked, above 150% it's burnt. Stop cooking when the food name is green for the maximum effect;  
• Shows a "loading bar" type of thing when cooking food.   
• This mod does not change how much hunger you gain back when eating.  
• The mod can be configured from a dedicated config file. To do so after running the game with the mod once, go to *...\Sailwind\BepInEx\config\pr0skynesis.cookedinfo.cfg* and make the change you want. Currently you can disable the loading bar, the percentage and/or the colored text.

**Requirements: Requires BepInEx**  
**Installation:** Download CookedInfo.dll and move it into the *...\Sailwind\BepInEx\plugins* folder.  
  
**Game version:** *0.24.1*  
**Mod Version:** *1.0.2*  
**Warning:** I don't really know much about programming so this might be buggy. I tested it for a while and didn't have any issue, but please let me know if there are problems. I'd recommend making a backup save, just in case, but I don't think this would break a savegame.  
**Compatibility:** This mod affects the UpdateMaterial method in the CookableFood. If another mod changes that there will be issues, I suppose. If that method or class get changed in the future the mod might stop working, but I'd say for the foreseeable future it should work for a few updates.  
