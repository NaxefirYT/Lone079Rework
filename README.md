# Lone079Rework

[![downloads](https://img.shields.io/github/downloads/NaxefirYT/Lone079Rework/total?style=for-the-badge&logo=icloud&color=%233A6D8C)](https://github.com/NaxefirYT/Lone079Rework/releases/latest)
![Latest](https://img.shields.io/github/v/release/NaxefirYT/Lone079Rework?style=for-the-badge&label=Latest%20Release&color=%23D91656)
[![Exiled Version](https://img.shields.io/badge/Exiled-9.3.0+-blue?style=for-the-badge)](https://github.com/Exiled-Team/EXILED)

---

## 📖 Description

**Lone079Rework** is a **EXILED** plugin for **SCP: Secret Laboratory** that transforms SCP-079 into a random SCP if it becomes the last SCP alive. After transformation, SCP-079 spawns in the containment chamber of the selected SCP.

---

## 🚀 Features

- **Automatic Transformation**: SCP-079 transforms into a random SCP when it’s the last SCP alive.
- **Containment Chamber Spawn**: The transformed SCP spawns in its original containment chamber.
- **Customizable Health**: Health can scale with SCP-079’s level or remain fixed.
- **Configurable**: Fully customizable via the `config.yml` file.

---

## 🛠️ Installation

1. Download the latest release from the [Releases](https://github.com/NaxefirYT/Lone079Rework/releases) page.
2. Place the `Lone079Rework.dll` file in the `Plugins` folder of your SCP:SL server.
3. Restart the server.

---

## ⚙️ Configuration

The plugin can be configured via the `config.yml` file located in the `Configs` folder.

Example configuration:
```yaml
lone079_rework:
  # Whether the plugin is enabled or not.
  is_enabled: true
  # Whether debug messages should be shown in the server console.
  debug: false
  # Whether SCP-049-2 (zombies) should be counted as SCPs when checking if SCP-079 is the last SCP alive.
  count_zombies: false
  # Whether SCP-079's respawn health should scale with its level. If true, health increases by 5% per level.
  scale_with_level: false
  # The base health percentage SCP-079 will respawn with. Default is 50%.
  health_percent: 50
  # The broadcast message shown to SCP-079 when it is respawned as another SCP.
  broadcast_message: '<i>You have been respawned as a random SCP with half health because all other SCPs have died.</i>'
  # The duration (in seconds) of the broadcast message shown to SCP-079 when it is respawned.
  broadcast_duration: 10
  # The delay (in seconds) before SCP-079 is respawned after being the last SCP alive.
  respawn_delay: 3
  # Whether SCP-079 should transform into another SCP when recontainment.
  transform_on_recontain: false
