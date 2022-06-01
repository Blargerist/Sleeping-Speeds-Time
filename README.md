# Sleeping-Speeds-Time
A V Rising server mod that makes time pass faster while sleeping in a coffin

### Dedicated Server Installation
- Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/) to your server folder
- Place SleepingSpeedsTime.dll into _(VRising server folder)/BepInEx/plugins_

### Singleplayer Installation
- Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/) to your <Steam Location>/steamapps/common/VRising/VRising_Server folder
- In the /VRising_Server folder, edit doorstop_config.ini to include the line _ignoreDisableSwitch=true_ in the [UnityDoorstop] section
- In the /VRising_server/BepInEx/Config folder, open BepInEx.cfg and change the Enabled value in the [Logging.Console] section to be false
- Place SleepingSpeedsTime.dll into /VRising/VRising_Server/BepInEx/plugins
